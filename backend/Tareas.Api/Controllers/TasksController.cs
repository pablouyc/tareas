using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Text.Json;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public TasksController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskResponse>>> GetAll()
    {
        var tasks = await _dbContext.TaskInstances
            .AsNoTracking()
            .OrderBy(t => t.DueDateUtc)
            .Select(ToResponseExpression())
            .ToListAsync();

        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> GetById(Guid id)
    {
        var task = await _dbContext.TaskInstances.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        return Ok(ToResponse(task));
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskCreateRequest request)
    {
        await EnsureTaskDependenciesAsync(request.TaskTemplateId, request.ClientId, request.SectorId);

        if (request.WorkType == TaskWorkType.Rectificativa && request.OriginalTaskId is null)
        {
            throw new DomainValidationException("OriginalTaskId is required for rectificativa tasks.");
        }

        if (request.WorkType == TaskWorkType.Normal && request.OriginalTaskId is not null)
        {
            throw new DomainValidationException("OriginalTaskId is only allowed for rectificativa tasks.");
        }

        TaskInstance? originalTask = null;
        if (request.OriginalTaskId is not null)
        {
            originalTask = await _dbContext.TaskInstances.FindAsync(request.OriginalTaskId.Value);
            if (originalTask is null)
            {
                throw new NotFoundException($"Original task '{request.OriginalTaskId}' not found.");
            }
        }

        var task = new TaskInstance
        {
            TaskTemplateId = request.TaskTemplateId,
            ClientId = request.ClientId,
            SectorId = request.SectorId,
            Title = request.Title,
            DueDateUtc = request.DueDateUtc,
            EstimatedHours = request.EstimatedHours,
            Link = request.Link,
            Comments = request.Comments,
            WorkType = request.WorkType,
            OriginalTaskId = request.OriginalTaskId,
            Status = WorkItemStatus.Pending
        };

        _dbContext.TaskInstances.Add(task);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = task.Id }, ToResponse(task));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskResponse>> Update(Guid id, TaskUpdateRequest request)
    {
        var task = await _dbContext.TaskInstances.FindAsync(id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        if (request.WorkType == TaskWorkType.Rectificativa && request.OriginalTaskId is null)
        {
            throw new DomainValidationException("OriginalTaskId is required for rectificativa tasks.");
        }

        if (request.WorkType == TaskWorkType.Normal && request.OriginalTaskId is not null)
        {
            throw new DomainValidationException("OriginalTaskId is only allowed for rectificativa tasks.");
        }

        if (request.OriginalTaskId is not null)
        {
            var originalExists = await _dbContext.TaskInstances.AnyAsync(t => t.Id == request.OriginalTaskId.Value);
            if (!originalExists)
            {
                throw new NotFoundException($"Original task '{request.OriginalTaskId}' not found.");
            }
        }

        task.Title = request.Title;
        task.DueDateUtc = request.DueDateUtc;
        task.EstimatedHours = request.EstimatedHours;
        task.Link = request.Link;
        task.Comments = request.Comments;
        task.Reason = request.Reason;
        task.WorkType = request.WorkType;
        task.OriginalTaskId = request.OriginalTaskId;
        task.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(ToResponse(task));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var task = await _dbContext.TaskInstances.FindAsync(id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        _dbContext.TaskInstances.Remove(task);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/start")]
    public async Task<ActionResult<TaskResponse>> Start(Guid id)
    {
        var task = await GetTaskForTransition(id);

        EnsureStatus(task, WorkItemStatus.Pending, "Only pending tasks can be started.");

        await UpdateStatusAsync(task, WorkItemStatus.Started, new { from = task.Status, to = WorkItemStatus.Started });

        return Ok(ToResponse(task));
    }

    [HttpPost("{id:guid}/send-to-review")]
    public async Task<ActionResult<TaskResponse>> SendToReview(Guid id)
    {
        var task = await GetTaskForTransition(id);

        EnsureStatus(task, WorkItemStatus.Started, "Only started tasks can be sent to review.");

        await UpdateStatusAsync(task, WorkItemStatus.InReview, new { from = task.Status, to = WorkItemStatus.InReview });

        return Ok(ToResponse(task));
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<ActionResult<TaskResponse>> Approve(Guid id)
    {
        var task = await GetTaskForTransition(id);

        EnsureStatus(task, WorkItemStatus.InReview, "Only tasks in review can be approved.");

        var blockingTasks = await _dbContext.TaskDependencies
            .Where(d => d.TaskId == id)
            .Include(d => d.DependsOnTask)
            .Select(d => d.DependsOnTask)
            .Where(t => t.Status != WorkItemStatus.Approved && t.Status != WorkItemStatus.Delivered)
            .Select(t => t.Id)
            .ToListAsync();

        if (blockingTasks.Count > 0)
        {
            throw new DependencyBlockedException("Task has pending dependencies.", blockingTasks);
        }

        task.ApprovedAtUtc = DateTimeOffset.UtcNow;
        await UpdateStatusAsync(task, WorkItemStatus.Approved, new { from = task.Status, to = WorkItemStatus.Approved });

        return Ok(ToResponse(task));
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<ActionResult<TaskResponse>> Reject(Guid id, TaskRejectRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new DomainValidationException("Reason is required to reject a task.");
        }

        var task = await GetTaskForTransition(id);

        EnsureStatus(task, WorkItemStatus.InReview, "Only tasks in review can be rejected.");

        task.Reason = request.Reason;
        await UpdateStatusAsync(task, WorkItemStatus.Rejected, new { from = task.Status, to = WorkItemStatus.Rejected, reason = request.Reason });

        return Ok(ToResponse(task));
    }

    [HttpPost("{id:guid}/deliver")]
    public async Task<ActionResult<TaskResponse>> Deliver(Guid id)
    {
        var task = await GetTaskForTransition(id);

        EnsureStatus(task, WorkItemStatus.Approved, "Only approved tasks can be delivered.");

        task.DeliveredAtUtc = DateTimeOffset.UtcNow;
        await UpdateStatusAsync(task, WorkItemStatus.Delivered, new { from = task.Status, to = WorkItemStatus.Delivered });

        return Ok(ToResponse(task));
    }

    [HttpPost("{id:guid}/rectify")]
    public async Task<ActionResult<TaskResponse>> Rectify(Guid id)
    {
        var original = await _dbContext.TaskInstances.FindAsync(id);
        if (original is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        var rectificativa = new TaskInstance
        {
            TaskTemplateId = original.TaskTemplateId,
            ClientId = original.ClientId,
            SectorId = original.SectorId,
            Title = $"Rectificativa - {original.Title}",
            Status = WorkItemStatus.Pending,
            WorkType = TaskWorkType.Rectificativa,
            OriginalTaskId = original.Id,
            DueDateUtc = original.DueDateUtc.AddDays(7),
            EstimatedHours = original.EstimatedHours
        };

        _dbContext.TaskInstances.Add(rectificativa);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = rectificativa.Id }, ToResponse(rectificativa));
    }

    [HttpPost("{id:guid}/assignees")]
    public async Task<IActionResult> AddAssignee(Guid id, TaskAssigneeRequest request)
    {
        var task = await _dbContext.TaskInstances.FindAsync(id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            throw new NotFoundException($"User '{request.UserId}' not found.");
        }

        var existing = await _dbContext.TaskAssignees.FindAsync(id, request.UserId);
        if (existing is not null)
        {
            return Ok();
        }

        _dbContext.TaskAssignees.Add(new TaskAssignee { TaskId = id, UserId = request.UserId });
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:guid}/assignees/{userId:guid}")]
    public async Task<IActionResult> RemoveAssignee(Guid id, Guid userId)
    {
        var assignee = await _dbContext.TaskAssignees.FindAsync(id, userId);
        if (assignee is null)
        {
            throw new NotFoundException($"Assignee '{userId}' not found for task '{id}'.");
        }

        _dbContext.TaskAssignees.Remove(assignee);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:guid}/reviewers")]
    public async Task<IActionResult> AddReviewer(Guid id, TaskReviewerRequest request)
    {
        var task = await _dbContext.TaskInstances.FindAsync(id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        var userExists = await _dbContext.Users.AnyAsync(u => u.Id == request.UserId);
        if (!userExists)
        {
            throw new NotFoundException($"User '{request.UserId}' not found.");
        }

        var existing = await _dbContext.TaskReviewers.FindAsync(id, request.UserId);
        if (existing is not null)
        {
            return Ok();
        }

        _dbContext.TaskReviewers.Add(new TaskReviewer { TaskId = id, UserId = request.UserId });
        await _dbContext.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id:guid}/reviewers/{userId:guid}")]
    public async Task<IActionResult> RemoveReviewer(Guid id, Guid userId)
    {
        var reviewer = await _dbContext.TaskReviewers.FindAsync(id, userId);
        if (reviewer is null)
        {
            throw new NotFoundException($"Reviewer '{userId}' not found for task '{id}'.");
        }

        _dbContext.TaskReviewers.Remove(reviewer);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static TaskResponse ToResponse(TaskInstance task)
    {
        return new TaskResponse(
            task.Id,
            task.TaskTemplateId,
            task.ClientId,
            task.SectorId,
            task.Title,
            task.Status,
            task.WorkType,
            task.OriginalTaskId,
            task.Reason,
            task.DueDateUtc,
            task.ApprovedAtUtc,
            task.DeliveredAtUtc,
            task.EstimatedHours,
            task.Link,
            task.Comments,
            task.IsActive);
    }

    private static Expression<Func<TaskInstance, TaskResponse>> ToResponseExpression()
    {
        return task => new TaskResponse(
            task.Id,
            task.TaskTemplateId,
            task.ClientId,
            task.SectorId,
            task.Title,
            task.Status,
            task.WorkType,
            task.OriginalTaskId,
            task.Reason,
            task.DueDateUtc,
            task.ApprovedAtUtc,
            task.DeliveredAtUtc,
            task.EstimatedHours,
            task.Link,
            task.Comments,
            task.IsActive);
    }

    private async Task EnsureTaskDependenciesAsync(Guid templateId, Guid clientId, Guid sectorId)
    {
        var existsTemplate = await _dbContext.TaskTemplates.AnyAsync(t => t.Id == templateId);
        if (!existsTemplate)
        {
            throw new NotFoundException($"Task template '{templateId}' not found.");
        }

        var existsClient = await _dbContext.Clients.AnyAsync(c => c.Id == clientId);
        if (!existsClient)
        {
            throw new NotFoundException($"Client '{clientId}' not found.");
        }

        var existsSector = await _dbContext.Sectors.AnyAsync(s => s.Id == sectorId);
        if (!existsSector)
        {
            throw new NotFoundException($"Sector '{sectorId}' not found.");
        }
    }

    private static void EnsureStatus(TaskInstance task, WorkItemStatus requiredStatus, string message)
    {
        if (task.Status != requiredStatus)
        {
            throw new DomainValidationException(message, new { task.Status, requiredStatus });
        }
    }

    private async Task<TaskInstance> GetTaskForTransition(Guid id)
    {
        var task = await _dbContext.TaskInstances.FindAsync(id);
        if (task is null)
        {
            throw new NotFoundException($"Task '{id}' not found.");
        }

        return task;
    }

    private async Task UpdateStatusAsync(TaskInstance task, WorkItemStatus newStatus, object payload)
    {
        task.Status = newStatus;
        task.UpdatedAtUtc = DateTimeOffset.UtcNow;

        _dbContext.TaskEvents.Add(new TaskEvent
        {
            TaskId = task.Id,
            EventType = TaskEventType.StatusChanged,
            Payload = JsonSerializer.Serialize(payload)
        });

        await _dbContext.SaveChangesAsync();
    }
}

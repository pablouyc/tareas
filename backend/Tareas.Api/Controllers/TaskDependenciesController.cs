using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/task-dependencies")]
public class TaskDependenciesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public TaskDependenciesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskDependencyResponse>>> GetAll()
    {
        var dependencies = await _dbContext.TaskDependencies
            .AsNoTracking()
            .Select(d => new TaskDependencyResponse(d.Id, d.TaskId, d.DependsOnTaskId))
            .ToListAsync();

        return Ok(dependencies);
    }

    [HttpPost]
    public async Task<ActionResult<TaskDependencyResponse>> Create(TaskDependencyCreateRequest request)
    {
        var taskExists = await _dbContext.TaskInstances.AnyAsync(t => t.Id == request.TaskId);
        if (!taskExists)
        {
            throw new NotFoundException($"Task '{request.TaskId}' not found.");
        }

        var dependencyExists = await _dbContext.TaskInstances.AnyAsync(t => t.Id == request.DependsOnTaskId);
        if (!dependencyExists)
        {
            throw new NotFoundException($"Dependency task '{request.DependsOnTaskId}' not found.");
        }

        var dependency = new TaskDependency
        {
            TaskId = request.TaskId,
            DependsOnTaskId = request.DependsOnTaskId
        };

        _dbContext.TaskDependencies.Add(dependency);
        _dbContext.TaskEvents.Add(new TaskEvent
        {
            TaskId = request.TaskId,
            EventType = TaskEventType.DependencyAdded,
            Payload = JsonSerializer.Serialize(new { request.TaskId, request.DependsOnTaskId })
        });

        await _dbContext.SaveChangesAsync();

        var response = new TaskDependencyResponse(dependency.Id, dependency.TaskId, dependency.DependsOnTaskId);
        return CreatedAtAction(nameof(GetAll), new { }, response);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var dependency = await _dbContext.TaskDependencies.FindAsync(id);
        if (dependency is null)
        {
            throw new NotFoundException($"Task dependency '{id}' not found.");
        }

        _dbContext.TaskDependencies.Remove(dependency);
        _dbContext.TaskEvents.Add(new TaskEvent
        {
            TaskId = dependency.TaskId,
            EventType = TaskEventType.DependencyRemoved,
            Payload = JsonSerializer.Serialize(new { dependency.TaskId, dependency.DependsOnTaskId })
        });

        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

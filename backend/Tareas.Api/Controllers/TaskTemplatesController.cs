using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/task-templates")]
public class TaskTemplatesController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public TaskTemplatesController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskTemplateResponse>>> GetAll()
    {
        var templates = await _dbContext.TaskTemplates
            .OrderBy(t => t.Name)
            .Select(t => new TaskTemplateResponse(t.Id, t.Name, t.SectorId, t.Periodicity, t.DefaultEstimatedHours, t.IsActive))
            .ToListAsync();

        return Ok(templates);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskTemplateResponse>> GetById(Guid id)
    {
        var template = await _dbContext.TaskTemplates.FindAsync(id);
        if (template is null)
        {
            throw new NotFoundException($"Task template '{id}' not found.");
        }

        return Ok(new TaskTemplateResponse(template.Id, template.Name, template.SectorId, template.Periodicity, template.DefaultEstimatedHours, template.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<TaskTemplateResponse>> Create(TaskTemplateCreateRequest request)
    {
        var template = new TaskTemplate
        {
            Name = request.Name,
            SectorId = request.SectorId,
            Periodicity = request.Periodicity,
            DefaultEstimatedHours = request.DefaultEstimatedHours,
            IsActive = request.IsActive
        };

        _dbContext.TaskTemplates.Add(template);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = template.Id },
            new TaskTemplateResponse(template.Id, template.Name, template.SectorId, template.Periodicity, template.DefaultEstimatedHours, template.IsActive));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskTemplateResponse>> Update(Guid id, TaskTemplateUpdateRequest request)
    {
        var template = await _dbContext.TaskTemplates.FindAsync(id);
        if (template is null)
        {
            throw new NotFoundException($"Task template '{id}' not found.");
        }

        template.Name = request.Name;
        template.SectorId = request.SectorId;
        template.Periodicity = request.Periodicity;
        template.DefaultEstimatedHours = request.DefaultEstimatedHours;
        template.IsActive = request.IsActive;
        template.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new TaskTemplateResponse(template.Id, template.Name, template.SectorId, template.Periodicity, template.DefaultEstimatedHours, template.IsActive));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var template = await _dbContext.TaskTemplates.FindAsync(id);
        if (template is null)
        {
            throw new NotFoundException($"Task template '{id}' not found.");
        }

        _dbContext.TaskTemplates.Remove(template);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

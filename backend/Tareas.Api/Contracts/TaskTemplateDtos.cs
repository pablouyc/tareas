using System.ComponentModel.DataAnnotations;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Contracts;

public record TaskTemplateResponse(Guid Id, string Name, Guid SectorId, TaskPeriodicity Periodicity, decimal DefaultEstimatedHours, bool IsActive);

public class TaskTemplateCreateRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    public TaskPeriodicity Periodicity { get; set; } = TaskPeriodicity.Monthly;

    [Range(0.25, 1000)]
    public decimal DefaultEstimatedHours { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}

public class TaskTemplateUpdateRequest
{
    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    public TaskPeriodicity Periodicity { get; set; } = TaskPeriodicity.Monthly;

    [Range(0.25, 1000)]
    public decimal DefaultEstimatedHours { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}

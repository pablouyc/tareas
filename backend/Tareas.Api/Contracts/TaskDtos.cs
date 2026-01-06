using System.ComponentModel.DataAnnotations;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Contracts;

public record TaskResponse(
    Guid Id,
    Guid TaskTemplateId,
    Guid ClientId,
    Guid SectorId,
    string Title,
    WorkItemStatus Status,
    TaskWorkType WorkType,
    Guid? OriginalTaskId,
    string? Reason,
    DateTimeOffset DueDateUtc,
    DateTimeOffset? ApprovedAtUtc,
    DateTimeOffset? DeliveredAtUtc,
    decimal EstimatedHours,
    string? Link,
    string? Comments,
    bool IsActive);

public class TaskCreateRequest
{
    [Required]
    public Guid TaskTemplateId { get; set; }

    [Required]
    public Guid ClientId { get; set; }

    [Required]
    public Guid SectorId { get; set; }

    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset DueDateUtc { get; set; }

    [Range(0.25, 1000)]
    public decimal EstimatedHours { get; set; } = 1;

    [StringLength(1000)]
    public string? Link { get; set; }

    [StringLength(4000)]
    public string? Comments { get; set; }

    public TaskWorkType WorkType { get; set; } = TaskWorkType.Normal;

    public Guid? OriginalTaskId { get; set; }
}

public class TaskUpdateRequest
{
    [Required]
    [StringLength(250)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public DateTimeOffset DueDateUtc { get; set; }

    [Range(0.25, 1000)]
    public decimal EstimatedHours { get; set; } = 1;

    [StringLength(1000)]
    public string? Link { get; set; }

    [StringLength(4000)]
    public string? Comments { get; set; }

    [StringLength(2000)]
    public string? Reason { get; set; }

    public TaskWorkType WorkType { get; set; } = TaskWorkType.Normal;

    public Guid? OriginalTaskId { get; set; }
}

public class TaskRejectRequest
{
    [Required]
    [StringLength(2000)]
    public string Reason { get; set; } = string.Empty;
}

public class TaskAssigneeRequest
{
    [Required]
    public Guid UserId { get; set; }
}

public class TaskReviewerRequest
{
    [Required]
    public Guid UserId { get; set; }
}

using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Domain.Entities;

public class TimeEntry : BaseEntity
{
    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public TimeEntryType Type { get; set; } = TimeEntryType.Task;

    public Guid? TaskId { get; set; }
    public TaskInstance? Task { get; set; }

    public DateOnly Date { get; set; }

    public decimal Hours { get; set; }

    public string? Notes { get; set; }
}

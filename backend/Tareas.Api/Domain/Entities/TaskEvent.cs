using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Domain.Entities;

public class TaskEvent : BaseEntity
{
    public Guid TaskId { get; set; }
    public TaskInstance Task { get; set; } = null!;

    public TaskEventType EventType { get; set; }

    public string? Payload { get; set; }
}

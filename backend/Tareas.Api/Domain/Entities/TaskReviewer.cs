namespace Tareas.Api.Domain.Entities;

public class TaskReviewer
{
    public Guid TaskId { get; set; }
    public TaskInstance Task { get; set; } = null!;

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}

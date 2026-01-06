namespace Tareas.Api.Domain.Entities;

// Una TaskInstance puede depender de muchas TaskInstances
public class TaskDependency : BaseEntity
{
    public Guid TaskId { get; set; }
    public TaskInstance Task { get; set; } = null!;

    public Guid DependsOnTaskId { get; set; }
    public TaskInstance DependsOnTask { get; set; } = null!;
}

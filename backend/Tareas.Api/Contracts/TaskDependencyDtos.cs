using System.ComponentModel.DataAnnotations;

namespace Tareas.Api.Contracts;

public record TaskDependencyResponse(Guid Id, Guid TaskId, Guid DependsOnTaskId);

public class TaskDependencyCreateRequest
{
    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public Guid DependsOnTaskId { get; set; }
}

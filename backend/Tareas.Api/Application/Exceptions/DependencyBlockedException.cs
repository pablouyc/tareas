namespace Tareas.Api.Application.Exceptions;

public class DependencyBlockedException : Exception
{
    public DependencyBlockedException(string message, IReadOnlyCollection<Guid> blockingTaskIds) : base(message)
    {
        BlockingTaskIds = blockingTaskIds;
    }

    public IReadOnlyCollection<Guid> BlockingTaskIds { get; }
}

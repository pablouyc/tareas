namespace Tareas.Api.Domain.Enums;

public enum TaskEventType
{
    StatusChanged = 0,
    CommentAdded = 1,
    Approved = 2,
    Rejected = 3,
    Delivered = 4,
    DependencyAdded = 5,
    DependencyRemoved = 6
}

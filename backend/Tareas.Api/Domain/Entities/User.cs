namespace Tareas.Api.Domain.Entities;

using Tareas.Api.Domain.Enums;

public class User : BaseEntity
{
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.Assignee;

    // Relación principal (sector “home”)
    public Guid? PrimarySectorId { get; set; }
    public Sector? PrimarySector { get; set; }

    public bool IsActive { get; set; } = true;
}

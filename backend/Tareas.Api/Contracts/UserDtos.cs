using System.ComponentModel.DataAnnotations;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Contracts;

public record UserResponse(Guid Id, string DisplayName, string Email, UserRole Role, Guid? PrimarySectorId, bool IsActive);

public class UserCreateRequest
{
    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Assignee;

    public Guid? PrimarySectorId { get; set; }

    public bool IsActive { get; set; } = true;
}

public class UserUpdateRequest
{
    [Required]
    [StringLength(200)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; } = UserRole.Assignee;

    public Guid? PrimarySectorId { get; set; }

    public bool IsActive { get; set; } = true;
}

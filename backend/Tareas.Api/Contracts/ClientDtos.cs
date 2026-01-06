using System.ComponentModel.DataAnnotations;

namespace Tareas.Api.Contracts;

public record ClientResponse(Guid Id, string Code, string Name, bool IsActive);

public class ClientCreateRequest
{
    [StringLength(50)]
    public string? Code { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

public class ClientUpdateRequest
{
    [StringLength(50)]
    public string? Code { get; set; }

    [Required]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

using System.ComponentModel.DataAnnotations;

namespace Tareas.Api.Contracts;

public record SectorResponse(Guid Id, string Name);

public class SectorCreateRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
}

public class SectorUpdateRequest
{
    [Required]
    [StringLength(150)]
    public string Name { get; set; } = string.Empty;
}

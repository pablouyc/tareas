namespace Tareas.Api.Domain.Entities;

public class Sector : BaseEntity
{
    public string Name { get; set; } = string.Empty; // "Impuestos indirectos", etc
    public bool IsActive { get; set; } = true;
}

namespace Tareas.Api.Domain.Entities;

public class Client : BaseEntity
{
    public string Code { get; set; } = string.Empty;   // opcional: "CLI-001"
    public string Name { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}

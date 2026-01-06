using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Domain.Entities;

public class TaskTemplate : BaseEntity
{
    // Ej: "IVA Mensual", "Cierre Contable", "Asientos", etc
    public string Name { get; set; } = string.Empty;

    // Sector dueño “principal”
    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;

    // Periodicidad base (para auto-crear)
    public TaskPeriodicity Periodicity { get; set; } = TaskPeriodicity.Monthly;

    // Estimación base (se recalcula mensualmente, pero esto es fallback)
    public decimal DefaultEstimatedHours { get; set; } = 1;

    public bool IsActive { get; set; } = true;
}

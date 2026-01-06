using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Domain.Entities;

public class TaskInstance : BaseEntity
{
    // Relación con plantilla (obligatoria)
    public Guid TaskTemplateId { get; set; }
    public TaskTemplate TaskTemplate { get; set; } = null!;

    // Cliente obligatorio
    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    // Sector “ejecutor” (puede diferir del template si colabora otro sector)
    public Guid SectorId { get; set; }
    public Sector Sector { get; set; } = null!;

    // Título editable (por defecto viene del template)
    public string Title { get; set; } = string.Empty;

    // Estado
    public WorkItemStatus Status { get; set; } = WorkItemStatus.Pending;

    // Tipo de trabajo (normal/rectificativa)
    public TaskWorkType WorkType { get; set; } = TaskWorkType.Normal;

    // Motivo solo si es rectificativa o si hay rechazo (texto libre)
    public string? Reason { get; set; }

    // Fechas KPI
    public DateTimeOffset DueDateUtc { get; set; }
    public DateTimeOffset? ApprovedAtUtc { get; set; }
    public DateTimeOffset? DeliveredAtUtc { get; set; }

    // Estimación (se recalcula cada mes por cliente/template, pero se “congela” en la tarea)
    public decimal EstimatedHours { get; set; } = 1;

    // Adjuntos/links simples por ahora (luego lo normalizamos)
    public string? Link { get; set; }
    public string? Comments { get; set; }

    public bool IsActive { get; set; } = true;
}

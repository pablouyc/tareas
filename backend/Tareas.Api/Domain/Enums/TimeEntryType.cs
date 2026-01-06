namespace Tareas.Api.Domain.Enums;

public enum TimeEntryType
{
    Task = 0,
    Operational = 1,   // automatizaciones, soporte interno, etc
    Meeting = 2,
    Admin = 3,
    Vacation = 4,
    Overtime = 5,
    Deduction = 6
}

# DB Schema (EF Core)

## Tablas principales

### Clients
- `Id` (PK)
- `Code`, `Name`, `IsActive`, `CreatedAtUtc`, `UpdatedAtUtc`

### Sectors
- `Id` (PK)
- `Name`, `IsActive`, `CreatedAtUtc`, `UpdatedAtUtc`

### Users
- `Id` (PK)
- `DisplayName`, `Email`, `Role`, `PrimarySectorId`, `IsActive`
- FK: `PrimarySectorId` → `Sectors.Id` (SetNull)

### TaskTemplates
- `Id` (PK)
- `Name`, `SectorId`, `Periodicity`, `DefaultEstimatedHours`, `IsActive`
- FK: `SectorId` → `Sectors.Id` (Restrict)

### TaskInstances
- `Id` (PK)
- `TaskTemplateId`, `ClientId`, `SectorId`, `Title`, `Status`, `WorkType`, `OriginalTaskId`
- `DueDateUtc`, `ApprovedAtUtc`, `DeliveredAtUtc`, `EstimatedHours`, `Reason`, `Link`, `Comments`
- FK: `TaskTemplateId` → `TaskTemplates.Id` (Restrict)
- FK: `ClientId` → `Clients.Id` (Restrict)
- FK: `SectorId` → `Sectors.Id` (Restrict)
- FK: `OriginalTaskId` → `TaskInstances.Id` (Restrict)

### TaskDependencies
- `Id` (PK)
- `TaskId`, `DependsOnTaskId`
- FK: `TaskId` → `TaskInstances.Id` (Cascade)
- FK: `DependsOnTaskId` → `TaskInstances.Id` (Restrict)

### TaskEvents
- `Id` (PK)
- `TaskId`, `EventType`, `Payload`, `CreatedAtUtc`
- FK: `TaskId` → `TaskInstances.Id` (Cascade)

### TaskAssignees
- PK compuesto: `TaskId`, `UserId`
- FK: `TaskId` → `TaskInstances.Id` (Cascade)
- FK: `UserId` → `Users.Id` (Cascade)

### TaskReviewers
- PK compuesto: `TaskId`, `UserId`
- FK: `TaskId` → `TaskInstances.Id` (Cascade)
- FK: `UserId` → `Users.Id` (Cascade)

### TimeEntries
- `Id` (PK)
- `UserId`, `TaskId` (nullable), `Type`, `Date`, `Hours`, `Notes`, `CreatedAtUtc`
- FK: `UserId` → `Users.Id` (Restrict)
- FK: `TaskId` → `TaskInstances.Id` (SetNull)

## Relaciones principales
- Un `Client` tiene muchas `TaskInstances`.
- Un `Sector` tiene muchas `TaskTemplates` y `TaskInstances`.
- Un `TaskTemplate` tiene muchas `TaskInstances`.
- Una `TaskInstance` puede depender de otras `TaskInstances` vía `TaskDependencies`.
- `TaskAssignees` y `TaskReviewers` conectan tareas con usuarios.
- `TaskEvents` registra cambios de estado y dependencias.
- `TimeEntries` registra horas por usuario y opcionalmente por tarea.

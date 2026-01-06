# API Contracts

## Formato de error
```json
{
  "error": "ValidationError",
  "message": "One or more validation errors occurred.",
  "details": {
    "field": ["error message"]
  }
}
```

## Health
### GET /health
Response **200**:
```json
{ "ok": true, "service": "Tareas.Api", "timestamp": "2026-01-06T00:00:00Z" }
```

## Version
### GET /api/version
Response **200**:
```json
{ "name": "Tareas.Api", "version": "0.1.0", "environment": "Development" }
```

## Debug
### GET /api/debug/ping-db
Response **200**:
```json
{ "ok": true }
```

## Clients
### POST /api/clients
Request:
```json
{ "code": "CLI-100", "name": "Cliente Demo", "isActive": true }
```
Response **201**:
```json
{ "id": "...", "code": "CLI-100", "name": "Cliente Demo", "isActive": true }
```

### GET /api/clients
Response **200**:
```json
[{ "id": "...", "code": "CLI-100", "name": "Cliente Demo", "isActive": true }]
```

### PUT /api/clients/{id}
Request:
```json
{ "code": "CLI-101", "name": "Cliente Actualizado", "isActive": true }
```

### DELETE /api/clients/{id}
Response **204**

## Sectors
### POST /api/sectors
```json
{ "name": "Finanzas" }
```

### GET /api/sectors
```json
[{ "id": "...", "name": "Finanzas" }]
```

## Users
### POST /api/users
```json
{ "displayName": "Carlos Ruiz", "email": "carlos@empresa.com", "role": 1, "primarySectorId": null, "isActive": true }
```

### GET /api/users
```json
[{ "id": "...", "displayName": "Carlos Ruiz", "email": "carlos@empresa.com", "role": 1, "primarySectorId": null, "isActive": true }]
```

## Task Templates
### POST /api/task-templates
```json
{ "name": "Balance mensual", "sectorId": "...", "periodicity": 1, "defaultEstimatedHours": 4, "isActive": true }
```

### GET /api/task-templates
```json
[{ "id": "...", "name": "Balance mensual", "sectorId": "...", "periodicity": 1, "defaultEstimatedHours": 4, "isActive": true }]
```

## Tasks
### POST /api/tasks
```json
{
  "taskTemplateId": "...",
  "clientId": "...",
  "sectorId": "...",
  "title": "Balance Abril",
  "dueDateUtc": "2026-01-20T00:00:00Z",
  "estimatedHours": 6,
  "workType": 0
}
```

### GET /api/tasks
```json
[
  {
    "id": "...",
    "taskTemplateId": "...",
    "clientId": "...",
    "sectorId": "...",
    "title": "Balance Abril",
    "status": 0,
    "workType": 0,
    "originalTaskId": null,
    "reason": null,
    "dueDateUtc": "2026-01-20T00:00:00Z",
    "approvedAtUtc": null,
    "deliveredAtUtc": null,
    "estimatedHours": 6,
    "link": null,
    "comments": null,
    "isActive": true
  }
]
```

### Transiciones
- **POST /api/tasks/{id}/start** → **200** con `TaskResponse`
- **POST /api/tasks/{id}/send-to-review** → **200** con `TaskResponse`
- **POST /api/tasks/{id}/approve** → **200** o **409**
  - Error **409**:
  ```json
  {
    "error": "DependencyBlocked",
    "message": "Task has pending dependencies.",
    "details": { "blockingTaskIds": ["..."] }
  }
  ```
- **POST /api/tasks/{id}/reject**
  - Request:
  ```json
  { "reason": "Falta documentación" }
  ```
- **POST /api/tasks/{id}/deliver** → **200**
- **POST /api/tasks/{id}/rectify** → **201**

### Assignees / Reviewers
- **POST /api/tasks/{id}/assignees**
  ```json
  { "userId": "..." }
  ```
- **DELETE /api/tasks/{id}/assignees/{userId}**
- **POST /api/tasks/{id}/reviewers**
  ```json
  { "userId": "..." }
  ```
- **DELETE /api/tasks/{id}/reviewers/{userId}**

## Task Dependencies
### POST /api/task-dependencies
```json
{ "taskId": "...", "dependsOnTaskId": "..." }
```

### DELETE /api/task-dependencies/{id}
Response **204**

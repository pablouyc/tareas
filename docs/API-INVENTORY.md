# API Inventory

> Base URL: `http://localhost:5013`

## Health
- `GET /health`
  - **200 OK**
  - Response:
    ```json
    {
      "ok": true,
      "service": "Tareas.Api",
      "timestamp": "2026-01-06T00:00:00Z"
    }
    ```

## Version
- `GET /api/version`
  - **200 OK**
  - Response:
    ```json
    {
      "name": "Tareas.Api",
      "version": "0.1.0",
      "environment": "Development"
    }
    ```

## Debug
- `GET /api/debug/ping-db`
  - **200 OK** cuando la conexión es válida.
  - **500** cuando falla la conexión.
  - Response (OK):
    ```json
    { "ok": true }
    ```

## Clients
- `GET /api/clients` → **200 OK**
- `GET /api/clients/{id}` → **200 OK / 404**
- `POST /api/clients` → **201 Created**
- `PUT /api/clients/{id}` → **200 OK / 404**
- `DELETE /api/clients/{id}` → **204 No Content / 404**

Example request:
```json
{
  "code": "CLI-100",
  "name": "Cliente Demo",
  "isActive": true
}
```

Example response:
```json
{
  "id": "00000000-0000-0000-0000-000000000000",
  "code": "CLI-100",
  "name": "Cliente Demo",
  "isActive": true
}
```

## Sectors
- `GET /api/sectors` → **200 OK**
- `GET /api/sectors/{id}` → **200 OK / 404**
- `POST /api/sectors` → **201 Created**
- `PUT /api/sectors/{id}` → **200 OK / 404**
- `DELETE /api/sectors/{id}` → **204 No Content / 404**

## Users
- `GET /api/users` → **200 OK**
- `GET /api/users/{id}` → **200 OK / 404**
- `POST /api/users` → **201 Created**
- `PUT /api/users/{id}` → **200 OK / 404**
- `DELETE /api/users/{id}` → **204 No Content / 404**

## Task Templates
- `GET /api/task-templates` → **200 OK**
- `GET /api/task-templates/{id}` → **200 OK / 404**
- `POST /api/task-templates` → **201 Created**
- `PUT /api/task-templates/{id}` → **200 OK / 404**
- `DELETE /api/task-templates/{id}` → **204 No Content / 404**

## Tasks
- `GET /api/tasks` → **200 OK**
- `GET /api/tasks/{id}` → **200 OK / 404**
- `POST /api/tasks` → **201 Created**
- `PUT /api/tasks/{id}` → **200 OK / 404**
- `DELETE /api/tasks/{id}` → **204 No Content / 404**

### Transiciones
- `POST /api/tasks/{id}/start`
- `POST /api/tasks/{id}/send-to-review`
- `POST /api/tasks/{id}/approve` → **409 Conflict** si hay dependencias bloqueantes
- `POST /api/tasks/{id}/reject` → requiere `reason`
- `POST /api/tasks/{id}/deliver`
- `POST /api/tasks/{id}/rectify`

## Task Dependencies
- `GET /api/task-dependencies` → **200 OK**
- `POST /api/task-dependencies` → **201 Created**
- `DELETE /api/task-dependencies/{id}` → **204 No Content / 404**

## Assignees & Reviewers
- `POST /api/tasks/{id}/assignees`
- `DELETE /api/tasks/{id}/assignees/{userId}`
- `POST /api/tasks/{id}/reviewers`
- `DELETE /api/tasks/{id}/reviewers/{userId}`

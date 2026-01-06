# Ejecutar localmente

## Base de datos (SQL Server 2022)
Desde la raíz del repo:

```bash
export MSSQL_SA_PASSWORD='TuPasswordFuerte!'
docker compose -f infra/docker-compose.yml up -d
```

## Migraciones
Desde la raíz del repo:

```bash
dotnet build backend/Tareas.Api
ConnectionStrings__Default='Server=localhost,14333;Database=TareasDb;User Id=sa;Password=TuPasswordFuerte!;TrustServerCertificate=True' \
  dotnet tool run dotnet-ef database update --project backend/Tareas.Api --startup-project backend/Tareas.Api
```

## Iniciar la API
Desde `backend/Tareas.Api`:

```bash
ConnectionStrings__Default='Server=localhost,14333;Database=TareasDb;User Id=sa;Password=TuPasswordFuerte!;TrustServerCertificate=True' \
  dotnet run
```

## Seeds de desarrollo
- Se ejecutan automáticamente **solo** en `Development`.
- Cargan sectores, clientes, usuarios, plantillas, tareas, dependencias, rectificativas y time entries.

## Tests
Desde la raíz del repo:

```bash
dotnet test backend/Tareas.Tests
```

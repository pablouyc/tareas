# State of Repo

## Árbol de carpetas (resumen)
```
.
├── backend/
│   ├── Tareas.Api/
│   │   ├── Application/
│   │   ├── Contracts/
│   │   ├── Controllers/
│   │   ├── Data/
│   │   ├── Domain/
│   │   ├── Migrations/
│   │   ├── Properties/
│   │   ├── Program.cs
│   │   ├── Tareas.Api.csproj
│   │   └── Tareas.Api.http
│   └── Tareas.Tests/
├── docs/
└── infra/
    └── docker-compose.yml
```

## Base de datos (SQL Server 2022)
Desde la raíz del repo:

```bash
docker compose -f infra/docker-compose.yml up -d
```

> Nota: configurar la variable `MSSQL_SA_PASSWORD` en el entorno antes de levantar el contenedor.

## Ejecutar la API
Desde `backend/Tareas.Api`:

```bash
dotnet run
```

## Manifiesto de herramientas
El manifiesto local de herramientas se encuentra en `.config/dotnet-tools.json` e incluye `dotnet-ef` para migraciones.

using Microsoft.EntityFrameworkCore;
using Tareas.Api.Domain.Entities;
using Tareas.Api.Domain.Enums;

namespace Tareas.Api.Data;

public static class DevelopmentSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext)
    {
        if (dbContext.Database.IsRelational())
        {
            await dbContext.Database.MigrateAsync();
        }
        else
        {
            await dbContext.Database.EnsureCreatedAsync();
        }

        if (await dbContext.Clients.AnyAsync())
        {
            return;
        }

        var sectors = new List<Sector>
        {
            new() { Name = "Contabilidad" },
            new() { Name = "Impuestos" },
            new() { Name = "Legal" }
        };

        var clients = new List<Client>
        {
            new() { Code = "CLI-001", Name = "Acme SA" },
            new() { Code = "CLI-002", Name = "Globex" },
            new() { Code = "CLI-003", Name = "Umbrella" },
            new() { Code = "CLI-004", Name = "Wayne Enterprises" }
        };

        var users = new List<User>
        {
            new() { DisplayName = "Ana Torres", Email = "ana@empresa.com", Role = UserRole.Admin },
            new() { DisplayName = "Luis Pérez", Email = "luis@empresa.com", Role = UserRole.Manager },
            new() { DisplayName = "Marta Díaz", Email = "marta@empresa.com", Role = UserRole.Reviewer },
            new() { DisplayName = "Juan Soto", Email = "juan@empresa.com", Role = UserRole.Assignee },
            new() { DisplayName = "Sofía Ramos", Email = "sofia@empresa.com", Role = UserRole.Assignee }
        };

        users[0].PrimarySector = sectors[0];
        users[1].PrimarySector = sectors[1];
        users[2].PrimarySector = sectors[2];

        var templates = new List<TaskTemplate>
        {
            new() { Name = "IVA Mensual", Sector = sectors[1], Periodicity = TaskPeriodicity.Monthly, DefaultEstimatedHours = 4 },
            new() { Name = "Cierre Contable", Sector = sectors[0], Periodicity = TaskPeriodicity.Monthly, DefaultEstimatedHours = 10 },
            new() { Name = "Revisión Legal", Sector = sectors[2], Periodicity = TaskPeriodicity.Quarterly, DefaultEstimatedHours = 6 }
        };

        var now = DateTimeOffset.UtcNow;
        var tasks = new List<TaskInstance>();
        for (var i = 0; i < 18; i++)
        {
            var template = templates[i % templates.Count];
            var client = clients[i % clients.Count];
            var sector = template.Sector;

            tasks.Add(new TaskInstance
            {
                TaskTemplate = template,
                Client = client,
                Sector = sector,
                Title = $"{template.Name} - {client.Name} #{i + 1}",
                Status = (WorkItemStatus)(i % 6),
                DueDateUtc = now.AddDays(i - 5),
                EstimatedHours = template.DefaultEstimatedHours + (i % 3),
                Comments = "Generado para desarrollo"
            });
        }

        var rectificativa = new TaskInstance
        {
            TaskTemplate = templates[0],
            Client = clients[0],
            Sector = sectors[1],
            Title = "Rectificativa IVA - Acme",
            Status = WorkItemStatus.Pending,
            WorkType = TaskWorkType.Rectificativa,
            DueDateUtc = now.AddDays(10),
            EstimatedHours = 2
        };

        tasks.Add(rectificativa);

        var dependencies = new List<TaskDependency>
        {
            new() { Task = tasks[3], DependsOnTask = tasks[1] },
            new() { Task = tasks[5], DependsOnTask = tasks[2] }
        };

        rectificativa.OriginalTask = tasks[0];

        var timeEntries = new List<TimeEntry>();
        for (var i = 0; i < 20; i++)
        {
            timeEntries.Add(new TimeEntry
            {
                User = users[i % users.Count],
                Task = tasks[i % tasks.Count],
                Type = TimeEntryType.Task,
                Date = DateOnly.FromDateTime(now.AddDays(-i).DateTime),
                Hours = 0.5m + (i % 6) * 0.25m,
                Notes = "Carga inicial de horas"
            });
        }

        await dbContext.AddRangeAsync(sectors);
        await dbContext.AddRangeAsync(clients);
        await dbContext.AddRangeAsync(users);
        await dbContext.AddRangeAsync(templates);
        await dbContext.AddRangeAsync(tasks);
        await dbContext.AddRangeAsync(dependencies);
        await dbContext.AddRangeAsync(timeEntries);

        await dbContext.SaveChangesAsync();
    }
}

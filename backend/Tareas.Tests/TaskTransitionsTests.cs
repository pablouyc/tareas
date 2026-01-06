using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Controllers;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;
using Tareas.Api.Domain.Enums;
using Xunit;

namespace Tareas.Tests;

public class TaskTransitionsTests
{
    [Fact]
    public async Task Reject_RequiresReason()
    {
        var dbContext = CreateDbContext();
        var task = new TaskInstance
        {
            Title = "Test",
            TaskTemplateId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            SectorId = Guid.NewGuid(),
            DueDateUtc = DateTimeOffset.UtcNow
        };
        dbContext.TaskInstances.Add(task);
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext);

        await Assert.ThrowsAsync<DomainValidationException>(() => controller.Reject(task.Id, new TaskRejectRequest { Reason = "" }));
    }

    [Fact]
    public async Task Approve_Fails_WhenDependenciesNotApproved()
    {
        var dbContext = CreateDbContext();
        var dependency = new TaskInstance
        {
            Title = "Dependency",
            TaskTemplateId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            SectorId = Guid.NewGuid(),
            DueDateUtc = DateTimeOffset.UtcNow,
            Status = WorkItemStatus.Started
        };
        var task = new TaskInstance
        {
            Title = "Main",
            TaskTemplateId = Guid.NewGuid(),
            ClientId = Guid.NewGuid(),
            SectorId = Guid.NewGuid(),
            DueDateUtc = DateTimeOffset.UtcNow,
            Status = WorkItemStatus.InReview
        };
        dbContext.TaskInstances.AddRange(task, dependency);
        dbContext.TaskDependencies.Add(new TaskDependency { TaskId = task.Id, DependsOnTaskId = dependency.Id });
        await dbContext.SaveChangesAsync();

        var controller = new TasksController(dbContext);

        await Assert.ThrowsAsync<DependencyBlockedException>(() => controller.Approve(task.Id));
    }

    private static AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"unit-{Guid.NewGuid()}")
            .Options;

        return new AppDbContext(options);
    }
}

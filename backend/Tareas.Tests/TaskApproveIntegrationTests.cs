using System.Net;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;
using Tareas.Api.Domain.Enums;
using Xunit;

namespace Tareas.Tests;

public class TaskApproveIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public TaskApproveIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Approve_ReturnsConflict_WhenDependenciesBlocked()
    {
        var client = _factory.CreateClient();

        Guid taskId;

        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

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

            taskId = task.Id;
        }

        var response = await client.PostAsync($"/api/tasks/{taskId}/approve", null);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);

        var json = await response.Content.ReadAsStringAsync();
        var doc = JsonDocument.Parse(json);
        Assert.Equal("DependencyBlocked", doc.RootElement.GetProperty("error").GetString());
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Tareas.Api.Application.Middleware;
using Tareas.Api.Application.Models;
using Tareas.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(entry => entry.Value?.Errors.Count > 0)
                .ToDictionary(
                    entry => entry.Key,
                    entry => entry.Value!.Errors.Select(error => error.ErrorMessage).ToArray());

            var response = new ErrorResponse
            {
                Error = "ValidationError",
                Message = "One or more validation errors occurred.",
                Details = errors
            };

            return new BadRequestObjectResult(response);
        };
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

var app = builder.Build();

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger (always in this project for now)
app.UseSwagger();
app.UseSwaggerUI();

// Dev: avoid HTTPS redirect issues (we'll enable HTTPS later when hosting)
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

// Basic endpoints
app.MapGet("/health", () => Results.Ok(new
{
    ok = true,
    service = "Tareas.Api",
    timestamp = DateTimeOffset.UtcNow
}));

app.MapGet("/api/version", () => Results.Ok(new
{
    name = "Tareas.Api",
    version = "0.1.0",
    environment = app.Environment.EnvironmentName
}));

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DevelopmentSeeder.SeedAsync(dbContext);
}

app.Run();

public partial class Program { }

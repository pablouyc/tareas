var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

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

app.Run();

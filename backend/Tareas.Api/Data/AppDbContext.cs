using Microsoft.EntityFrameworkCore;

namespace Tareas.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
}

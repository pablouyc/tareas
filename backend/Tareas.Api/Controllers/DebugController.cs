using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Data;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/debug")]
public class DebugController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public DebugController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet("ping-db")]
    public async Task<IActionResult> PingDb()
    {
        await _dbContext.Database.ExecuteSqlRawAsync("SELECT 1");
        return Ok(new { ok = true });
    }
}

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/sectors")]
public class SectorsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public SectorsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<SectorResponse>>> GetAll()
    {
        var sectors = await _dbContext.Sectors
            .OrderBy(s => s.Name)
            .Select(s => new SectorResponse(s.Id, s.Name))
            .ToListAsync();

        return Ok(sectors);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<SectorResponse>> GetById(Guid id)
    {
        var sector = await _dbContext.Sectors.FindAsync(id);
        if (sector is null)
        {
            throw new NotFoundException($"Sector '{id}' not found.");
        }

        return Ok(new SectorResponse(sector.Id, sector.Name));
    }

    [HttpPost]
    public async Task<ActionResult<SectorResponse>> Create(SectorCreateRequest request)
    {
        var sector = new Sector { Name = request.Name };
        _dbContext.Sectors.Add(sector);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = sector.Id }, new SectorResponse(sector.Id, sector.Name));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<SectorResponse>> Update(Guid id, SectorUpdateRequest request)
    {
        var sector = await _dbContext.Sectors.FindAsync(id);
        if (sector is null)
        {
            throw new NotFoundException($"Sector '{id}' not found.");
        }

        sector.Name = request.Name;
        sector.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new SectorResponse(sector.Id, sector.Name));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var sector = await _dbContext.Sectors.FindAsync(id);
        if (sector is null)
        {
            throw new NotFoundException($"Sector '{id}' not found.");
        }

        _dbContext.Sectors.Remove(sector);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

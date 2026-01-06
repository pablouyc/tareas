using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/clients")]
public class ClientsController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public ClientsController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ClientResponse>>> GetAll()
    {
        var clients = await _dbContext.Clients
            .OrderBy(c => c.Name)
            .Select(c => new ClientResponse(c.Id, c.Code, c.Name, c.IsActive))
            .ToListAsync();

        return Ok(clients);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> GetById(Guid id)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client is null)
        {
            throw new NotFoundException($"Client '{id}' not found.");
        }

        return Ok(new ClientResponse(client.Id, client.Code, client.Name, client.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<ClientResponse>> Create(ClientCreateRequest request)
    {
        var client = new Client
        {
            Code = request.Code ?? string.Empty,
            Name = request.Name,
            IsActive = request.IsActive
        };

        _dbContext.Clients.Add(client);
        await _dbContext.SaveChangesAsync();

        var response = new ClientResponse(client.Id, client.Code, client.Name, client.IsActive);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ClientResponse>> Update(Guid id, ClientUpdateRequest request)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client is null)
        {
            throw new NotFoundException($"Client '{id}' not found.");
        }

        client.Code = request.Code ?? string.Empty;
        client.Name = request.Name;
        client.IsActive = request.IsActive;
        client.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new ClientResponse(client.Id, client.Code, client.Name, client.IsActive));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var client = await _dbContext.Clients.FindAsync(id);
        if (client is null)
        {
            throw new NotFoundException($"Client '{id}' not found.");
        }

        _dbContext.Clients.Remove(client);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

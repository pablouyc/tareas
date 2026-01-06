using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tareas.Api.Application.Exceptions;
using Tareas.Api.Contracts;
using Tareas.Api.Data;
using Tareas.Api.Domain.Entities;

namespace Tareas.Api.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public UsersController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        var users = await _dbContext.Users
            .OrderBy(u => u.DisplayName)
            .Select(u => new UserResponse(u.Id, u.DisplayName, u.Email, u.Role, u.PrimarySectorId, u.IsActive))
            .ToListAsync();

        return Ok(users);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetById(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user is null)
        {
            throw new NotFoundException($"User '{id}' not found.");
        }

        return Ok(new UserResponse(user.Id, user.DisplayName, user.Email, user.Role, user.PrimarySectorId, user.IsActive));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(UserCreateRequest request)
    {
        var user = new User
        {
            DisplayName = request.DisplayName,
            Email = request.Email,
            Role = request.Role,
            PrimarySectorId = request.PrimarySectorId,
            IsActive = request.IsActive
        };

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = user.Id },
            new UserResponse(user.Id, user.DisplayName, user.Email, user.Role, user.PrimarySectorId, user.IsActive));
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<UserResponse>> Update(Guid id, UserUpdateRequest request)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user is null)
        {
            throw new NotFoundException($"User '{id}' not found.");
        }

        user.DisplayName = request.DisplayName;
        user.Email = request.Email;
        user.Role = request.Role;
        user.PrimarySectorId = request.PrimarySectorId;
        user.IsActive = request.IsActive;
        user.UpdatedAtUtc = DateTimeOffset.UtcNow;

        await _dbContext.SaveChangesAsync();

        return Ok(new UserResponse(user.Id, user.DisplayName, user.Email, user.Role, user.PrimarySectorId, user.IsActive));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user is null)
        {
            throw new NotFoundException($"User '{id}' not found.");
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }
}

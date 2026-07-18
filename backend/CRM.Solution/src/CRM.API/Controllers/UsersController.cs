using CRM.Application.Interfaces;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CRM.API.Authorization;
using CRM.Application.Common.Security;
using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UsersController(
        ApplicationDbContext context,
        IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    [RequireRole(Roles.SuperAdmin, Roles.Admin)]
    [Authorize(Policy = "Users.Create")]
    [HttpPost]
    public async Task<IActionResult> Create(UserRequest request)
    {
        var userExists = await _context.Users
            .AnyAsync(x => x.Email == request.Email);

        if (userExists)
        {
            return BadRequest(new
            {
                message = "User already exists"
            });
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            OrganizationId = request.OrganizationId,
            Name = request.Name,
            Email = request.Email,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "User created successfully",
            user.Id,
            user.Email,
            user.Role
        });
    }
}

public class UserRequest
{
    public Guid OrganizationId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string Role { get; set; } = Roles.User;
}
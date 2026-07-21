using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public RolesController(ApplicationDbContext context)
    {
        _context = context;
    }


    // GET: api/Roles
    [HttpGet]
    public async Task<IActionResult> GetRoles()
    {
        var roles = await _context.Roles
            .ToListAsync();

        return Ok(roles);
    }


    // GET: api/Roles/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetRole(Guid id)
    {
        var role = await _context.Roles
            .FirstOrDefaultAsync(x => x.Id == id);

        if (role == null)
            return NotFound();

        return Ok(role);
    }


    // POST: api/Roles
    [HttpPost]
    public async Task<IActionResult> CreateRole(Role role)
    {
        role.Id = Guid.NewGuid();
        role.CreatedAt = DateTime.UtcNow;

        await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();

        return Ok(role);
    }
}
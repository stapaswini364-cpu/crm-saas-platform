using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class PermissionsController : ControllerBase
{
    private readonly ApplicationDbContext _context;


    public PermissionsController(ApplicationDbContext context)
    {
        _context = context;
    }



    // GET: api/Permissions

    [HttpGet]
    public async Task<IActionResult> GetPermissions()
    {
        var permissions = await _context.Permissions
            .ToListAsync();

        return Ok(permissions);
    }



    // POST: api/Permissions

    [HttpPost]
    public async Task<IActionResult> CreatePermission(
        Permission permission)
    {
        permission.Id = Guid.NewGuid();
        permission.CreatedAt = DateTime.UtcNow;


        await _context.Permissions.AddAsync(permission);

        await _context.SaveChangesAsync();


        return Ok(permission);
    }
}
using CRM.API.Authorization;
using CRM.Application.Common.Security;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public OrganizationsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [Authorize(Policy = "Reports.View")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var organizations = await _context.Organizations.ToListAsync();
        return Ok(organizations);
    }

    [RequireRole(Roles.SuperAdmin, Roles.Admin)]
    [Authorize(Policy = "Organizations.Create")]
    [HttpPost]
    public async Task<IActionResult> Create(Organization organization)
    {
        _context.Organizations.Add(organization);
        await _context.SaveChangesAsync();

        return Ok(organization);
    }
}
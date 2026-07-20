using CRM.Application.Common.Security;
using CRM.Application.Interfaces;

using CRM.Domain.Entities;

using CRM.Infrastructure.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace CRM.API.Controllers;


[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{

    private readonly ApplicationDbContext _db;

    private readonly IPasswordHasher _passwordHasher;



    public UsersController(
        ApplicationDbContext db,
        IPasswordHasher passwordHasher)
    {
        _db = db;

        _passwordHasher = passwordHasher;
    }



    // =========================
    // GET ALL USERS
    // =========================

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetUsers()
    {
        var users =
            await _db.Users
            .ToListAsync();


        return Ok(users);
    }



    // =========================
    // CREATE USER
    // Requires Users.Create Permission
    // =========================

    [HttpPost]
    [Authorize(Policy = "Users.Create")]
    public async Task<IActionResult> CreateUser(
        UserRequest request)
    {


        var existingUser =
            await _db.Users
            .FirstOrDefaultAsync(
                x => x.Email == request.Email);



        if(existingUser != null)
        {
            return BadRequest(
                "Email already exists");
        }



        var user = new User
        {

            Id = Guid.NewGuid(),


            OrganizationId =
                request.OrganizationId,


            Name =
                request.Name,


            Email =
                request.Email,


            PasswordHash =
                _passwordHasher.Hash(
                    request.Password),


            Role =
                request.Role ?? "User"

        };



        _db.Users.Add(user);


        await _db.SaveChangesAsync();



        return Ok(new
        {
            message = "User created successfully",

            user.Id,

            user.Name,

            user.Email,

            user.Role
        });

    }


}



// =========================
// Request DTO
// =========================

public class UserRequest
{

    public Guid OrganizationId { get; set; }



    public string Name { get; set; } = "";



    public string Email { get; set; } = "";



    public string Password { get; set; } = "";



    public string? Role { get; set; }

}
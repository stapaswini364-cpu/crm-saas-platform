using CRM.Application.Interfaces;
using CRM.Infrastructure.Data;
using CRM.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    private readonly IRedisCacheService _cache;


    public TasksController(
        ApplicationDbContext db,
        IRedisCacheService cache)
    {
        _db = db;
        _cache = cache;
    }


    [HttpGet]
    public async Task<IActionResult> GetTasks()
    {
        var tasks = await _db.Tasks
            .Include(t => t.AssignedToUser)
            .ToListAsync();

        return Ok(tasks);
    }


    [HttpPost]
    public async Task<IActionResult> CreateTask(TaskItem task)
    {
        task.Id = Guid.NewGuid();

        _db.Tasks.Add(task);

        await _db.SaveChangesAsync();


        await _cache.RemoveAsync(
            "dashboard:tasks-overview");


        return Ok(task);
    }
}
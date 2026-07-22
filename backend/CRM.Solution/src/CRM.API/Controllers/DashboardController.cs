using CRM.Application.DTOs.Dashboard;
using CRM.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashboardService _dashboardService;
    private readonly IRedisCacheService _cache;

    public DashboardController(
        IDashboardService dashboardService,
        IRedisCacheService cache)
    {
        _dashboardService = dashboardService;
        _cache = cache;
    }


    [HttpGet("tasks-overview")]
    public async Task<IActionResult> TasksOverview()
    {
        var key = "dashboard:tasks-overview";

        var cached = await _cache.GetAsync(key);

        if (cached != null)
        {
            return Ok(
                JsonSerializer.Deserialize<TasksOverviewDto>(cached));
        }


        var result =
            await _dashboardService.GetTasksOverviewAsync();


        await _cache.SetAsync(
            key,
            JsonSerializer.Serialize(result),
            TimeSpan.FromMinutes(5));


        return Ok(result);
    }



    [HttpGet("followups-due")]
    public async Task<IActionResult> FollowUpsDue()
    {
        var key = "dashboard:followups-due";

        var cached = await _cache.GetAsync(key);

        if (cached != null)
        {
            return Ok(
                JsonSerializer.Deserialize<List<FollowUpDto>>(cached));
        }


        var result =
            await _dashboardService.GetFollowUpsDueAsync();


        await _cache.SetAsync(
            key,
            JsonSerializer.Serialize(result),
            TimeSpan.FromMinutes(5));


        return Ok(result);
    }
}
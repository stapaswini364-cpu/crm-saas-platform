using CRM.Application.DTOs.Dashboard;
using CRM.Application.Interfaces;
using CRM.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly ApplicationDbContext _db;

    public DashboardService(ApplicationDbContext db)
    {
        _db = db;
    }


    public async Task<TasksOverviewDto> GetTasksOverviewAsync()
    {
        var today = DateTime.UtcNow;

        return new TasksOverviewDto
        {
            Total = await _db.Tasks.CountAsync(),

            Pending = await _db.Tasks.CountAsync(t =>
                t.Status == "Pending"),

            InProgress = await _db.Tasks.CountAsync(t =>
                t.Status == "InProgress"),

            Completed = await _db.Tasks.CountAsync(t =>
                t.Status == "Completed"),

            Overdue = await _db.Tasks.CountAsync(t =>
                t.DueDate < today &&
                t.Status != "Completed")
        };
    }


    public async Task<List<FollowUpDto>> GetFollowUpsDueAsync()
    {
        var today = DateTime.UtcNow.Date;

        return await _db.Enquiries
            .Where(e => e.FollowUpDate <= today)
            .OrderBy(e => e.FollowUpDate)
            .Select(e => new FollowUpDto
            {
                Id = e.Id,
                Name = e.Name,
                FollowUpDate = e.FollowUpDate,
                Status = e.Status,
                Source = e.Source
            })
            .ToListAsync();
    }
}
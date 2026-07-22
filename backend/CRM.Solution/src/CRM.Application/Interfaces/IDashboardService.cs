using CRM.Application.DTOs.Dashboard;

namespace CRM.Application.Interfaces;

public interface IDashboardService
{
    Task<TasksOverviewDto> GetTasksOverviewAsync();

    Task<List<FollowUpDto>> GetFollowUpsDueAsync();
}
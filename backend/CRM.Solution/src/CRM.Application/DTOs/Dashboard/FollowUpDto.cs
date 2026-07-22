namespace CRM.Application.DTOs.Dashboard;

public class FollowUpDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime FollowUpDate { get; set; }

    public string Status { get; set; } = string.Empty;

    public string Source { get; set; } = string.Empty;
}
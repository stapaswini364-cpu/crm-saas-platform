namespace CRM.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Status { get; set; } = "Pending";

    public DateTime DueDate { get; set; }

    public Guid? AssignedToUserId { get; set; }

    public User? AssignedToUser { get; set; }
}
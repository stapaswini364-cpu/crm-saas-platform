namespace CRM.Domain.Entities;

public class Enquiry
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime FollowUpDate { get; set; }

    public string Status { get; set; } = "Pending";

    public string Source { get; set; } = string.Empty;
}
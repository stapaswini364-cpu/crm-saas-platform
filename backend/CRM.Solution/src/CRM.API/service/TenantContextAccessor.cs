namespace CRM.API.Services;

public class TenantContextAccessor : ITenantContextAccessor
{
    public string? TenantId { get; set; }
}
namespace CRM.API.Services;

public interface ITenantContextAccessor
{
    string? TenantId { get; set; }
}
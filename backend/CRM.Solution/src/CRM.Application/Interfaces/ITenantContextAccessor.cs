namespace CRM.Application.Interfaces;

public interface ITenantContextAccessor
{
    Guid? TenantId { get; set; }

    void SetTenantId(Guid tenantId);
}
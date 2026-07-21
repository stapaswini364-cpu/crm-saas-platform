namespace CRM.Application.Common.Security;

public interface ITenantContextAccessor
{
    Guid? TenantId { get; }

    void SetTenantId(Guid tenantId);
}
using CRM.Application.Interfaces;

namespace CRM.Infrastructure.Security;

public class TenantContextAccessor : ITenantContextAccessor
{
    public Guid? TenantId { get; set; }


    public void SetTenantId(Guid tenantId)
    {
        TenantId = tenantId;
    }
}
using CRM.Application.Common.Security;

namespace CRM.Infrastructure.Security;

public class TenantContextAccessor : ITenantContextAccessor
{
    private Guid? _tenantId;

    public Guid? TenantId => _tenantId;


    public void SetTenantId(Guid tenantId)
    {
        _tenantId = tenantId;
    }
}
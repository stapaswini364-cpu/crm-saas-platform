using CRM.Application.Interfaces;

namespace CRM.Infrastructure.Security;

public class TenantContextAccessor : ITenantContextAccessor
{
    public Guid? TenantId { get; set; }
}
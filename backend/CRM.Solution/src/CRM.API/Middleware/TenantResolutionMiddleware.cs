using CRM.Application.Interfaces;

namespace CRM.API.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;


    public TenantResolutionMiddleware(
        RequestDelegate next)
    {
        _next = next;
    }


    public async Task InvokeAsync(
        HttpContext context,
        ITenantContextAccessor tenantContextAccessor)
    {
        Guid? tenantId = null;


        // Read Tenant Id from Header
        if (context.Request.Headers.TryGetValue(
            "X-Tenant-Id",
            out var headerValue))
        {
            if(Guid.TryParse(
                headerValue.ToString(),
                out var parsedTenantId))
            {
                tenantId = parsedTenantId;
            }
        }


        // Read Tenant Id from JWT Claim
        if(tenantId == null)
        {
            var claimValue =
                context.User
                    .FindFirst("tenant_id")
                    ?.Value;


            if(Guid.TryParse(
                claimValue,
                out var claimTenantId))
            {
                tenantId = claimTenantId;
            }
        }


        if(tenantId.HasValue)
        {
            context.Items["TenantId"] = tenantId.Value;

            tenantContextAccessor.TenantId = tenantId.Value;
        }


        await _next(context);
    }
}
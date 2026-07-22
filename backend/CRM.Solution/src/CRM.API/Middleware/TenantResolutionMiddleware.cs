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
        string? tenantId = null;


        // Read Tenant Id from Header
        if (context.Request.Headers.TryGetValue(
            "X-Tenant-Id",
            out var headerValue))
        {
            tenantId = headerValue.ToString();
        }


        // If Header not found, read from JWT Claim
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            tenantId = context.User
                .FindFirst("tenant_id")
                ?.Value;
        }


        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            context.Items["TenantId"] = tenantId;


            if (Guid.TryParse(
                tenantId,
                out var parsedTenantId))
            {
                tenantContextAccessor
                    .SetTenantId(parsedTenantId);
            }
        }


        await _next(context);
    }
}
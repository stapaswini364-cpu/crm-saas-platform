using Microsoft.AspNetCore.Authorization;

namespace CRM.API.Authorization;

public class RequireRoleAttribute : AuthorizeAttribute
{
    public RequireRoleAttribute(params string[] roles)
    {
        Roles = string.Join(",", roles);
    }
}
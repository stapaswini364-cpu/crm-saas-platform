using System.Collections.Generic;

namespace CRM.Application.Common.Security;

public static class RolePermissions
{
    public static readonly Dictionary<string, List<string>> Map = new()
    {
        [Roles.SuperAdmin] = new()
        {
            Permissions.UsersView,
            Permissions.UsersCreate,
            Permissions.UsersUpdate,
            Permissions.UsersDelete,

            Permissions.OrganizationsView,
            Permissions.OrganizationsCreate,
            Permissions.OrganizationsUpdate,
            Permissions.OrganizationsDelete,

            Permissions.ReportsView
        },

        [Roles.Admin] = new()
        {
            Permissions.UsersView,
            Permissions.UsersCreate,
            Permissions.UsersUpdate,

            Permissions.OrganizationsView,
            Permissions.OrganizationsUpdate,

            Permissions.ReportsView
        },

        [Roles.User] = new()
        {
            Permissions.UsersView
        }
    };
}
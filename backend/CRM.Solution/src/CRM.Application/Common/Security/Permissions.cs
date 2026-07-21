namespace CRM.Application.Common.Security;

public static class Permissions
{
    // Users
    public const string UsersView = "users.view";
    public const string UsersCreate = "users.create";
    public const string UsersUpdate = "users.update";
    public const string UsersDelete = "users.delete";

    // Organizations
    public const string OrganizationsView = "organizations.view";
    public const string OrganizationsCreate = "organizations.create";
    public const string OrganizationsUpdate = "organizations.update";
    public const string OrganizationsDelete = "organizations.delete";

    // Reports
    public const string ReportsView = "reports.view";
}
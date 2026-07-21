using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await context.Database.MigrateAsync();


        // Roles
        if (!await context.Roles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "SuperAdmin",
                    Description = "Full system access"
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrative access"
                },

                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "User",
                    Description = "Normal user access"
                }
            };

            await context.Roles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }



        // Permissions
        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Users.Create",
                    Description = "Create users"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Users.Update",
                    Description = "Update users"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Users.Delete",
                    Description = "Delete users"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Organizations.Create",
                    Description = "Create organizations"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Organizations.Update",
                    Description = "Update organizations"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Organizations.Delete",
                    Description = "Delete organizations"
                },

                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "Reports.View",
                    Description = "View reports"
                }
            };


            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }



        // Role Permission Mapping

        if (!await context.RolePermissions.AnyAsync())
        {
            var superAdmin =
                await context.Roles
                .FirstAsync(x => x.Name == "SuperAdmin");


            var admin =
                await context.Roles
                .FirstAsync(x => x.Name == "Admin");


            var user =
                await context.Roles
                .FirstAsync(x => x.Name == "User");


            var permissions =
                await context.Permissions.ToListAsync();



            var mappings = new List<RolePermission>();


            // SuperAdmin gets all permissions

            foreach(var permission in permissions)
            {
                mappings.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = superAdmin.Id,
                    PermissionId = permission.Id
                });
            }



            // Admin permissions

            foreach(var permission in permissions
                .Where(x =>
                    x.Name != "Organizations.Delete"))
            {
                mappings.Add(new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = admin.Id,
                    PermissionId = permission.Id
                });
            }



            // User only reports

            var reportPermission =
                permissions.First(x =>
                    x.Name == "Reports.View");


            mappings.Add(new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = user.Id,
                PermissionId = reportPermission.Id
            });



            await context.RolePermissions.AddRangeAsync(mappings);
            await context.SaveChangesAsync();
        }
    }
}
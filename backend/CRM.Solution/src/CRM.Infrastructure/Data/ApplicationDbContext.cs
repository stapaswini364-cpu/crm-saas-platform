using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }


    public DbSet<Organization> Organizations => Set<Organization>();

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();


    // RBAC Tables

    public DbSet<Role> Roles => Set<Role>();

    public DbSet<Permission> Permissions => Set<Permission>();

    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);



        // =========================
        // Role Configuration
        // =========================

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(x => x.Id);


            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(100);


            entity.Property(x => x.Description)
                .HasMaxLength(250);


            entity.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Role)
                .HasForeignKey(x => x.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
        });



        // =========================
        // Permission Configuration
        // =========================

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.HasKey(x => x.Id);


            entity.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(150);


            entity.Property(x => x.Description)
                .HasMaxLength(250);


            entity.HasMany(x => x.RolePermissions)
                .WithOne(x => x.Permission)
                .HasForeignKey(x => x.PermissionId)
                .OnDelete(DeleteBehavior.Cascade);
        });



        // =========================
        // Role Permission Mapping
        // =========================

        modelBuilder.Entity<RolePermission>(entity =>
        {
            entity.HasKey(x => x.Id);


            entity.HasIndex(x => new
            {
                x.RoleId,
                x.PermissionId
            })
            .IsUnique();
        });



        // =========================
        // Refresh Token Configuration
        // =========================

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(x => x.Id);


            entity.Property(x => x.Token)
                .IsRequired();


            entity.HasIndex(x => x.Token)
                .IsUnique();


            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
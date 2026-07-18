using Xunit;
using CRM.Infrastructure.Security;
using CRM.Infrastructure.Services;
using CRM.Infrastructure.Data;
using CRM.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRM.Tests;

public class AuthFlowTests
{
    [Fact]
    public void Password_Hash_And_Verify_Should_Work()
    {
        var hasher = new BCryptPasswordHasher();

        var password = "Admin@123";

        var hash = hasher.Hash(password);

        var result = hasher.Verify(password, hash);

        Assert.True(result);
    }


    [Fact]
    public void Wrong_Password_Should_Fail()
    {
        var hasher = new BCryptPasswordHasher();

        var hash = hasher.Hash("Admin@123");

        var result = hasher.Verify("WrongPassword", hash);

        Assert.False(result);
    }


    [Fact]
    public void Refresh_Token_Should_Be_Generated()
    {
        var service = new RefreshTokenService();

        var token = service.GenerateRefreshToken();

        Assert.NotNull(token);
        Assert.NotEmpty(token);
    }


    [Fact]
    public async Task Refresh_Token_Should_Save_And_Revoke()
    {
        // InMemory Database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase("AuthTestDb")
            .Options;


        using var context = new ApplicationDbContext(options);


        var userId = Guid.NewGuid();

        var refreshToken = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Token = "old-refresh-token",
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };


        // Save token
        context.RefreshTokens.Add(refreshToken);

        await context.SaveChangesAsync();


        // Verify saved
        var savedToken = await context.RefreshTokens
            .FirstAsync(x => x.Token == "old-refresh-token");


        Assert.NotNull(savedToken);
        Assert.False(savedToken.IsRevoked);


        // Revoke old token
        savedToken.IsRevoked = true;
        savedToken.RevokedAt = DateTime.UtcNow;


        await context.SaveChangesAsync();


        // Verify revoked
        var revokedToken = await context.RefreshTokens
            .FirstAsync(x => x.Token == "old-refresh-token");


        Assert.True(revokedToken.IsRevoked);
        Assert.NotNull(revokedToken.RevokedAt);
    }
}
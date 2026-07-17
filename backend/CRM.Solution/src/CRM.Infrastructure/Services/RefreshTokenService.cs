using System.Security.Cryptography;

namespace CRM.Infrastructure.Services;

public class RefreshTokenService
{
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(64);

        return Convert.ToBase64String(randomBytes);
    }
}
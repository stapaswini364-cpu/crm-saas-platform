using CRM.Application.Interfaces;
using CRM.Application.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CRM.Infrastructure.Services;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _configuration;

    public JwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public string GenerateToken(
        Guid userId,
        string email,
        string role)
    {
        var claims = new List<Claim>
        {
            new Claim(
                JwtRegisteredClaimNames.Sub,
                userId.ToString()
            ),

            new Claim(
                JwtRegisteredClaimNames.Email,
                email
            ),

            new Claim(
                ClaimTypes.Role,
                role
            )
        };


        // Add Role Permissions into JWT Claims
        if (RolePermissions.Map.TryGetValue(
            role,
            out var permissions))
        {
            foreach (var permission in permissions)
            {
                claims.Add(
                    new Claim(
                        "permission",
                        permission
                    )
                );
            }
        }


        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!
            )
        );


        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256
        );


        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],

            audience: _configuration["Jwt:Audience"],

            claims: claims,

            expires: DateTime.UtcNow.AddHours(1),

            signingCredentials: credentials
        );


        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }
}
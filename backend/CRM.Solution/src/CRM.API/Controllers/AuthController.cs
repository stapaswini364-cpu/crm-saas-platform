using CRM.Application.DTOs;
using CRM.Application.Interfaces;
using CRM.Application.Services;
using CRM.Domain.Entities;
using CRM.Infrastructure.Data;
using CRM.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRM.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;
    private readonly RefreshTokenService _refreshTokenService;

    public AuthController(
        ApplicationDbContext context,
        IJwtTokenService jwtTokenService,
        IPasswordHasher passwordHasher,
        RefreshTokenService refreshTokenService)
    {
        _context = context;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
        _refreshTokenService = refreshTokenService;
    }


    // LOGIN
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == request.Email);


        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password"
            });
        }


        var passwordValid = _passwordHasher.Verify(
            request.Password,
            user.PasswordHash);


        if (!passwordValid)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password"
            });
        }


        var accessToken = _jwtTokenService.GenerateToken(user);


        var refreshToken = _refreshTokenService.GenerateRefreshToken();


        var refreshTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = refreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };


        _context.RefreshTokens.Add(refreshTokenEntity);

        await _context.SaveChangesAsync();


        return Ok(new LoginResponse
        {
            Token = accessToken,
            RefreshToken = refreshToken
        });
    }



    // REFRESH TOKEN ROTATION
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshRequest request)
    {
        var oldToken = await _context.RefreshTokens
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);



        if (oldToken == null ||
            oldToken.IsRevoked ||
            oldToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized(new
            {
                message = "Invalid refresh token"
            });
        }



        // revoke old token
        oldToken.IsRevoked = true;
        oldToken.RevokedAt = DateTime.UtcNow;



        // generate new refresh token
        var newRefreshToken =
            _refreshTokenService.GenerateRefreshToken();



        oldToken.ReplacedByToken = newRefreshToken;



        var newTokenEntity = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = oldToken.UserId,
            Token = newRefreshToken,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };



        _context.RefreshTokens.Add(newTokenEntity);



        var newAccessToken =
            _jwtTokenService.GenerateToken(oldToken.User);



        await _context.SaveChangesAsync();



        return Ok(new RefreshTokenResponse
        {
            Token = newAccessToken,
            RefreshToken = newRefreshToken
        });
    }



    // LOGOUT
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshRequest request)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken);



        if (refreshToken == null)
        {
            return NotFound(new
            {
                message = "Refresh token not found"
            });
        }



        refreshToken.IsRevoked = true;
        refreshToken.RevokedAt = DateTime.UtcNow;



        await _context.SaveChangesAsync();



        return Ok(new
        {
            message = "Logged out successfully"
        });
    }
}
using CRM.Domain.Entities;

namespace CRM.Application.Services;

public interface IJwtTokenService
{
    string GenerateToken(User user);
}
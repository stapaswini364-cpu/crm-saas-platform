namespace CRM.Application.Interfaces;

public interface IRedisCacheService
{
    Task<string?> GetAsync(string key);

    Task SetAsync(
        string key,
        string value,
        TimeSpan expiry);

    Task RemoveAsync(string key);
}
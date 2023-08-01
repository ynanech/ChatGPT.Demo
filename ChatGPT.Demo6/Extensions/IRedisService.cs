using StackExchange.Redis;

namespace ChatGPT.Demo6.Extensions
{
public interface IRedisService
{
    Task<IDatabase> GetDatabaseAsync();
}
}
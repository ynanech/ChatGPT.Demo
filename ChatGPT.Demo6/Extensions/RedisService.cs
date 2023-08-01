using StackExchange.Redis;

namespace ChatGPT.Demo6.Extensions
{
public class RedisService : IDisposable, IRedisService
{
    private volatile ConnectionMultiplexer _connection;
    private IDatabase _cache;
    private readonly string _configuration;
    private readonly SemaphoreSlim _connectionLock = new SemaphoreSlim(initialCount: 1, maxCount: 1);

    public RedisService(string configuration)
    {
        _configuration = configuration;
    }

    public async Task<IDatabase> GetDatabaseAsync()
    {
        await ConnectAsync();
        return _cache;
    }

    private async Task ConnectAsync(CancellationToken token = default)
    {
        if (_cache != null) return;
        await _connectionLock.WaitAsync(token);
        try
        {
            if (_cache == null)
            {
                token.ThrowIfCancellationRequested();
                _connection = await ConnectionMultiplexer.ConnectAsync(_configuration);
                _cache = _connection.GetDatabase();
            }
        }
        finally
        {
            _connectionLock.Release();
        }
    }

    public void Dispose() => _connection?.Close();
}
}

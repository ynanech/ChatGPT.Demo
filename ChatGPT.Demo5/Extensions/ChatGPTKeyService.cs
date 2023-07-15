using StackExchange.Redis;

namespace ChatGPT.Demo5.Extensions
{
    public class ChatGPTKeyService : IChatGPTKeyService
    {
        private ConnectionMultiplexer? _connection;
        private IDatabase? _cache;
        private readonly string _configuration;
        private const string _redisKey = "ChatGPTKey";

        public ChatGPTKeyService(string configuration)
        {
            _configuration = configuration;
        }

        private async Task ConnectAsync()
        {
            if (_cache != null) return;
            _connection = await ConnectionMultiplexer.ConnectAsync(_configuration);
            _cache = _connection.GetDatabase();
        }
        public async Task InitAsync()
        {
            await ConnectAsync();
            //使用Set对象存储密钥
            await _cache!.SetAddAsync(_redisKey, new RedisValue[] {
        "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1",
        "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx2",
        });
        }
        public async Task<string> GetRandomAsync()
        {
            await ConnectAsync();
            //使用Set随机返回一个密钥
            var redisValue = await _cache!.SetRandomMemberAsync(_redisKey);
            return redisValue.ToString();
        }

        public async Task<string[]> GetAllAsync()
        {
            await ConnectAsync();
            //读取所有密钥
            var redisValues = await _cache!.SetMembersAsync(_redisKey);
            return redisValues.Select(m => m.ToString()).ToArray();
        }

        public async Task RemoveAsync(string apiKey)
        {
            await ConnectAsync();
            await _cache!.SetRemoveAsync(_redisKey, apiKey);
        }
    }
}

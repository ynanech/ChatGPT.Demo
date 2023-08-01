using StackExchange.Redis;

namespace ChatGPT.Demo6.Extensions
{
    public class ChatGPTKeyService : IChatGPTKeyService
    {
        private readonly IRedisService _redisService;
        private const string _redisKey = "ChatGPTKey";

        public ChatGPTKeyService(IRedisService redisService)
        {
            _redisService = redisService;
        }


        public async Task InitAsync()
        {
            var _cache = await _redisService.GetDatabaseAsync();
            //使用Set对象存储密钥
            await _cache!.SetAddAsync(_redisKey, new RedisValue[] {
        "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx1",
        "sk-xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx2",
        });
        }
        public async Task<string> GetRandomAsync()
        {
            var _cache = await _redisService.GetDatabaseAsync();
            //使用Set随机返回一个密钥
            var redisValue = await _cache!.SetRandomMemberAsync(_redisKey);
            return redisValue.ToString();
        }

        public async Task<string[]> GetAllAsync()
        {
            var _cache = await _redisService.GetDatabaseAsync();
            //读取所有密钥
            var redisValues = await _cache!.SetMembersAsync(_redisKey);
            return redisValues.Select(m => m.ToString()).ToArray();
        }

        public async Task RemoveAsync(string apiKey)
        {
            var _cache = await _redisService.GetDatabaseAsync();
            await _cache!.SetRemoveAsync(_redisKey, apiKey);
        }
    }
}

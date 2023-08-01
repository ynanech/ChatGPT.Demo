namespace ChatGPT.Demo6.Extensions
{
public interface IChatGPTKeyService
{
    //初始话密钥
    public Task InitAsync();

    //随机获取密钥KEY
    public Task<string> GetRandomAsync();

    //获取所有密钥
    Task<string[]> GetAllAsync();

    //移除密钥
    Task RemoveAsync(string apiKey);
}
}

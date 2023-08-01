using NRedisStack;
using NRedisStack.Search;
using NRedisStack.Search.DataTypes;
using NRedisStack.Search.Literals.Enums;
using StackExchange.Redis;
using static NRedisStack.Search.Schema;

namespace ChatGPT.Demo6.Extensions
{
public class RedisVectorSearchService : IRedisVectorSearchService
{
    private readonly IRedisService _redisService;
    private SearchCommands _searchCommands;
    public RedisVectorSearchService(IRedisService redisService)
    {
        _redisService = redisService;
    }

    //获取Redis向量搜索对象
    private async Task<SearchCommands> GetSearchCommandsAsync()
    {
        if (_searchCommands != null) return _searchCommands;

        var db = await _redisService.GetDatabaseAsync();
        _searchCommands = new SearchCommands(db, null);
        return _searchCommands;
    }


    public async Task CreateIndexAsync(string indexName, string indexPrefix, int vectorSize)
    {
        var ft = await GetSearchCommandsAsync();
        await ft.CreateAsync(indexName,
            new FTCreateParams()
                        .On(IndexDataType.HASH)
                        .Prefix(indexPrefix),
            new Schema()
                        .AddTextField("content")
                        .AddVectorField("vector",
                            VectorField.VectorAlgo.HNSW,
                            new Dictionary<string, object>()
                            {
                                ["TYPE"] = "FLOAT32",
                                ["DIM"] = vectorSize,
                                ["DISTANCE_METRIC"] = "COSINE"
                            }));
    }

    public async Task SetAsync(string indexPrefix, string docId, string content, float[] vector)
    {
        var db = await _redisService.GetDatabaseAsync();
        await db.HashSetAsync($"{indexPrefix}{docId}", new HashEntry[] {
            new HashEntry ("content", content),
            new HashEntry ("vector", vector.SelectMany(BitConverter.GetBytes).ToArray())
        });
    }

    public async Task DeleteAsync(string indexPrefix, string docId)
    {
        var db = await _redisService.GetDatabaseAsync();
        await db.KeyDeleteAsync($"{indexPrefix}{docId}");
    }

    public async Task DropIndexAsync(string indexName)
    {
        var ft = await GetSearchCommandsAsync();
        await ft.DropIndexAsync(indexName, true);
    }

    public async Task<InfoResult> InfoAsync(string indexName)
    {
        var ft = await GetSearchCommandsAsync();
        return await ft.InfoAsync(indexName);
    }

    public async IAsyncEnumerable<(string Content, double Score)> SearchAsync(string indexName, float[] vector, int limit)
    {
        var query = new Query($"*=>[KNN {limit} @vector $vector AS score]");

        query = query.AddParam("vector", vector.SelectMany(BitConverter.GetBytes).ToArray())
            .SetSortBy("score")
            .ReturnFields("content", "score")
            .Limit(0, limit)
            .Dialect(2);

        var ft = await GetSearchCommandsAsync();
        var result = await ft.SearchAsync(indexName, query).ConfigureAwait(false);
        foreach (var document in result.Documents)
        {
            yield return (document["content"], Convert.ToDouble(document["score"]));
        }
    }
}
}

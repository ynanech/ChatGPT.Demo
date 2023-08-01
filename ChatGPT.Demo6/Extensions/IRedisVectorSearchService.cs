using NRedisStack.Search.DataTypes;

namespace ChatGPT.Demo6.Extensions
{
public interface IRedisVectorSearchService
{
    //创建索引
    Task CreateIndexAsync(string indexName, string indexPrefix, int vectorSize);
    //删除索引
    Task DropIndexAsync(string indexName);
    //查看索引信息
    Task<InfoResult> InfoAsync(string indexName);
    //向量搜索
    IAsyncEnumerable<(string Content, double Score)> SearchAsync(string indexName, float[] vector, int limit);

    //删除索引数据
    Task DeleteAsync(string indexPrefix, string docId);
    //添加或修改索引数据
    Task SetAsync(string indexPrefix, string docId, string content, float[] vector);
}
}
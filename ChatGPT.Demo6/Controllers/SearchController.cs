using ChatGPT.Demo6.Extensions;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using StackExchange.Redis;

namespace ChatGPT.Demo6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly IOpenAIService _openAIService;
        private readonly IRedisVectorSearchService _redisVectorSearchService;
        //定义索引库名字
        private readonly string _indexName = "VectorSearchIndex";
        //定义索引数据前缀
        private readonly string _indexPrefix = "VectorSearchItem";
        public SearchController(IOpenAIService openAIService, IRedisVectorSearchService redisVectorSearchService)
        {
            _openAIService = openAIService;
            _redisVectorSearchService = redisVectorSearchService;
        }

        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpGet]
        public async Task<IActionResult> IndexAsync(string message, CancellationToken cancellationToken)
        {
            var embeddingResult = await _openAIService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest()
            {
                Input = message,
                Model = OpenAI.ObjectModels.Models.TextSearchAdaDocV1
            }, cancellationToken);


            if (!embeddingResult.Successful)
            {
                if (embeddingResult.Error == null)
                    throw new Exception("Unknown Error");
                return Content($"{embeddingResult.Error.Code}: {embeddingResult.Error.Message}");
            }

            var embeddingResponse = embeddingResult.Data.FirstOrDefault();

            int size = 10;
            var searchResponse = _redisVectorSearchService.SearchAsync(
                _indexName, embeddingResponse.Embedding.Select(m => Convert.ToSingle(m)).ToArray(), size);

            var searchResult = new List<object>(size);
            await foreach ((string Content, double Score) in searchResponse)
            {
                searchResult.Add(new
                {
                    Content,
                    Score
                });
            }
            return Ok(searchResult);
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(InitAsync))]
        public async Task<string> InitAsync()
        {
            var inputAsList = new List<string> { "我喜欢吃苹果", "我讨厌吃香蕉", "我爱吃瓜", "我喜欢喝茶", "我不爱喝咖啡", "我讨厌喝饮料", "我爱喝酒" };
            var embeddingResult = await _openAIService.Embeddings.CreateEmbedding(new EmbeddingCreateRequest()
            {
                InputAsList = inputAsList,
                Model = OpenAI.ObjectModels.Models.TextSearchAdaDocV1
            });

            if (!embeddingResult.Successful) return $"{embeddingResult.Error.Code}: {embeddingResult.Error.Message}";

            try
            {
                await _redisVectorSearchService.InfoAsync(_indexName).ConfigureAwait(false);
                await _redisVectorSearchService.DropIndexAsync(_indexName);
            }
            catch (RedisServerException ex) when (ex.Message == "Unknown Index name")
            {
                //索引不存在
            }

            await _redisVectorSearchService.CreateIndexAsync(_indexName, _indexPrefix, 1024);

            int i = 0;
            foreach (var item in inputAsList)
            {
                await _redisVectorSearchService.SetAsync(_indexPrefix, i.ToString(), item, embeddingResult.Data[i].Embedding.Select(m => Convert.ToSingle(m)).ToArray());
                ++i;
            }
            return "初始化成功";
        }
    }
}
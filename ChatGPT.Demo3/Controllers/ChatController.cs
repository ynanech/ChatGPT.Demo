using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Tokenizers;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Tokenizer.GPT3;

namespace ChatGPT.Demo3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IOpenAIService _openAiService;
        public ChatController(IOpenAIService openAiService)
        {
            _openAiService = openAiService;
        }

        [HttpPost]
        public async Task Input([FromForm] string message, CancellationToken cancellationToken)
        {
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");
            var completionResult = _openAiService.ChatCompletion.CreateCompletionAsStream(
                new ChatCompletionCreateRequest
                {
                    Messages = new List<ChatMessage>
                    {
                ChatMessage.FromUser(message)
                    },
                    Stream = true,
                    MaxTokens = 500,
                    Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo,
                }, cancellationToken: cancellationToken);

            await foreach (var completion in completionResult)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (completion.Successful)
                {
                    await Response.WriteAsync(completion.Choices.FirstOrDefault()?.Message.Content ?? "", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
                else
                {
                    if (completion.Error == null)
                        throw new Exception("Unknown Error");

                    await Response.WriteAsync($"{completion.Error.Code}: {completion.Error.Message}", cancellationToken);
                    await Response.Body.FlushAsync(cancellationToken);
                }
            }
        }


        [HttpGet(nameof(TokenCountOne))]
        public IActionResult TokenCountOne()
        {
            // 定义输入字符串
            // 文档 https://github.com/betalgo/openai/wiki/Tokenizer
            var input = "the brown fox jumped over the lazy dog!";
            var tokenCount = TokenizerGpt3.TokenCount(input, true);
            return Content($"Token数量为：{tokenCount}");
        }


        [HttpGet(nameof(TokenCountTwo))]
        public IActionResult TokenCountTwo()
        {
            // 定义词汇表文件和合并规则文件的路径
            //下载地址：https://huggingface.co/gpt2/tree/main
            var vocabFilePath = @"vocab.json";
            var mergeFilePath = @"merges.txt";

            // 创建分词器对象，传入BPE算法的参数
            var tokenizer = new Tokenizer(new Bpe(vocabFilePath, mergeFilePath));

            // 定义输入字符串
            var input = "the brown fox jumped over the lazy dog!";

            // 对输入字符串进行编码，得到编码结果对象
            var tokenizerEncodedResult = tokenizer.Encode(input);
            // 获取单词数量，并返回该值
            var tokenCount = tokenizerEncodedResult.Tokens.Count;
            return Content($"Token数量为：{tokenCount}");
        }
    }
}

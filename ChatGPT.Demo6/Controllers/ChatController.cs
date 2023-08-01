using System.Text.Json;
using ChatGPT.Demo6.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Tokenizers;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;
using OpenAI.Tokenizer.GPT3;

namespace ChatGPT.Demo6.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IOpenAIService _openAiService;
        //private readonly IChatGPTKeyService _chatGPTKeyService;

        public ChatController(IOpenAIService openAiService /* , IChatGPTKeyService chatGPTKeyService*/)
        {
            _openAiService = openAiService;
            //_chatGPTKeyService = chatGPTKeyService;
        }

        [HttpPost]
        public async Task Input([FromForm] string[] messages, CancellationToken cancellationToken)
        {
            int i = 0;
            Response.Headers.Add("Content-Type", "text/event-stream");
            Response.Headers.Add("Cache-Control", "no-cache");

            //string apiKey = await _chatGPTKeyService.GetRandomAsync();
            //IOpenAIService _openAiService = new OpenAIService(new OpenAiOptions
            //{
            //    ApiKey = apiKey
            //});
            var chatCompletionCreateRequest =
                new ChatCompletionCreateRequest
                {
                    Messages = messages.Select(m => i++ % 2 == 0 ? ChatMessage.FromUser(m) : ChatMessage.FromAssistant(m)).ToList(),
                    Stream = true,
                    MaxTokens = 500,
                    Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_0613,
                    Functions = ChatGPTFunctionCalling.Functions
                };
            var completionResult = _openAiService.ChatCompletion.CreateCompletionAsStream(
                chatCompletionCreateRequest, cancellationToken: cancellationToken);

            await foreach (var completion in completionResult)
            {
                if (cancellationToken.IsCancellationRequested)
                    break;

                if (completion.Successful)
                {
                    var responseMessage = completion.Choices.First().Message;
                    if (responseMessage.FunctionCall != null)
                    {
                        var functionName = responseMessage.FunctionCall.Name;
                        var functionArgs = JsonSerializer.Deserialize<Dictionary<string, string>>(responseMessage.FunctionCall.Arguments);
                        var functionToCall = ChatGPTFunctionCalling.AvailableFunctions[functionName];

                        var registrationTime = functionArgs.GetValueOrDefault("registrationTime") switch
                        {
                            "后天" => DateOnly.FromDateTime(DateTime.Now.AddDays(2)),
                            "明天" => DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                            _ => DateOnly.FromDateTime(DateTime.Now.Date),
                        };
                        var functionResponse = functionToCall(functionArgs.GetValueOrDefault("phoneNumber"), registrationTime);

                        chatCompletionCreateRequest.Messages.Add(ChatMessage.FromFunction(functionResponse, functionName));

                        var completionTwo = await _openAiService.ChatCompletion.CreateCompletion(
                            new ChatCompletionCreateRequest
                            {
                                Messages = chatCompletionCreateRequest.Messages,
                                Model = OpenAI.ObjectModels.Models.Gpt_3_5_Turbo_0613
                            }, cancellationToken: cancellationToken);


                        if (completionTwo.Successful)
                        {
                            await Response.WriteAsync(completionTwo.Choices.First().Message.Content ?? "", cancellationToken);
                            await Response.Body.FlushAsync(cancellationToken);
                        }
                        else
                        {
                            if (completionTwo.Error == null)
                                throw new Exception("Unknown Error");

                            await Response.WriteAsync($"{completionTwo.Error.Code}: {completionTwo.Error.Message}");
                            await Response.Body.FlushAsync();
                        }

                    }
                    else
                    {
                        await Response.WriteAsync(responseMessage.Content ?? "");
                        await Response.Body.FlushAsync();
                    }
                }
                else
                {
                    if (completion.Error == null)
                        throw new Exception("Unknown Error");

                    await Response.WriteAsync($"{completion.Error.Code}: {completion.Error.Message}");
                    await Response.Body.FlushAsync();
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

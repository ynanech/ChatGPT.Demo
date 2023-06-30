using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace ChatGPT.Demo1.Controllers
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
        public async Task<string> Input([FromForm] string message)
        {
            //访方法实现了ChatGT的/v1/chat/completions接口
            var completionResult = await _openAiService.ChatCompletion.CreateCompletion(
                new ChatCompletionCreateRequest
                {
                    //聊天信息
                    Messages = new List<ChatMessage>
                    {
                        ChatMessage.FromUser(message)
                    },
                    //指定模型
                    Model = OpenAI.ObjectModels.Models.ChatGpt3_5Turbo,
                    //设置Tokens限制数量
                    MaxTokens = 50
                });
            if (!completionResult.Successful)
                return "请求失败";
            return $"ChatGPT：{completionResult.Choices.First().Message.Content}";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using OpenAI.Interfaces;
using OpenAI.ObjectModels.RequestModels;

namespace ChatGPT.Demo2.Controllers
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
    }
}

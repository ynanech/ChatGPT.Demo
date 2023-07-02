namespace ChatGPT.Demo4.Extensions
{
    public class ChatGPTHttpMessageHandler : DelegatingHandler
    {
        private readonly IChatGPTKeyService _chatGPTKeyService;

        public ChatGPTHttpMessageHandler(IChatGPTKeyService chatGPTKeyService)
        {
            _chatGPTKeyService = chatGPTKeyService;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiKey = await _chatGPTKeyService.GetRandomAsync();

            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            return await base.SendAsync(request, cancellationToken);
        }

        protected override HttpResponseMessage Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var apiKey = _chatGPTKeyService.GetRandomAsync().Result;
            request.Headers.Remove("Authorization");
            request.Headers.Add("Authorization", $"Bearer {apiKey}");
            return base.Send(request, cancellationToken);
        }
    }
}

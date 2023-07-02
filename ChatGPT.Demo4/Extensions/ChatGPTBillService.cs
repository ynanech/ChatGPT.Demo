namespace ChatGPT.Demo4.Extensions
{
public class ChatGPTBillService : IChatGPTBillService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ChatGPTBillService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<ChatGPTBillModel?> QueryAsync(string apiKey)
    {
        string url = "https://api.openai.com/v1/dashboard/billing/subscription";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        var response = await client.GetFromJsonAsync<ChatGPTBillModel>(url);
        return response;
    }

    public async Task<ChatGPTBillDetailsModel?> QueryDetailsAsync(string apiKey, DateTimeOffset startTime, DateTimeOffset endTime)
    {
        string url = $"https://api.openai.com/dashboard/billing/usage?start_date={startTime:yyyy-MM-dd}&end_date={endTime:yyyy-MM-dd}";
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        var response = await client.GetFromJsonAsync<ChatGPTBillDetailsModel>(url);
        return response;
    }
}
}

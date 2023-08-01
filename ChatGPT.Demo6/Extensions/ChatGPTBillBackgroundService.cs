namespace ChatGPT.Demo6.Extensions
{
public class ChatGPTBillBackgroundService : BackgroundService
{
    private readonly IChatGPTKeyService _chatGPTKeyService;
    private readonly IChatGPTBillService _chatGPTBillService;

    public ChatGPTBillBackgroundService(IChatGPTKeyService chatGPTKeyService, IChatGPTBillService chatGPTBillService)
    {
        _chatGPTKeyService = chatGPTKeyService;
        _chatGPTBillService = chatGPTBillService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var apiKeys = await _chatGPTKeyService.GetAllAsync();
            foreach (var apiKey in apiKeys)
            {
                var bill = await _chatGPTBillService.QueryAsync(apiKey);
                if (bill == null) continue;

                var dt = DateTimeOffset.Now;
                //判断key是否到期或是否有额度
                if (bill.AccessUntil < dt.ToUnixTimeSeconds() || bill.HardLimitUsd == 0)
                {
                    await _chatGPTKeyService.RemoveAsync(apiKey);
                    continue;
                }
                //查询99天以内的账单明细
                var billDetails = await _chatGPTBillService.QueryDetailsAsync(
                    apiKey, dt.AddDays(-99), dt.AddDays(1));

                if (billDetails == null) continue;

                //判断已使用额度大于等于总额度
                if (billDetails.TotalUsage >= bill.HardLimitUsd)
                {
                    await _chatGPTKeyService.RemoveAsync(apiKey);
                    continue;
                }
            }

            // 每隔1分钟轮询刷新
            await Task.Delay(1 * 60 * 1000, stoppingToken);
        }

    }
}
}

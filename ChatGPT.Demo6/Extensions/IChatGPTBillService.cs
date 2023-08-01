namespace ChatGPT.Demo6.Extensions
{
public interface IChatGPTBillService
{
    /// <summary>
    /// 查询账单
    /// </summary>
    /// <param name="apiKey">api密钥</param>
    /// <returns></returns>
    Task<ChatGPTBillModel?> QueryAsync(string apiKey);

    /// <summary>
    /// 账单详情
    /// </summary>
    /// <param name="apiKey">api密钥</param>
    /// <param name="startTime">开始日期</param>
    /// <param name="endTime">结束日期</param>
    /// <returns></returns>
    Task<ChatGPTBillDetailsModel?> QueryDetailsAsync(string apiKey, DateTimeOffset startTime, DateTimeOffset endTime);
}
}

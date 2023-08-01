using System.Text.Json.Serialization;

namespace ChatGPT.Demo6.Extensions
{
    public class ChatGPTBillDetailsModel
    {
        [JsonPropertyName("object")]
        public required string Object { get; set; }

        [JsonPropertyName("daily_costs")]
        public DailyCostInfo[]? DailyCosts { get; set; }

        [JsonPropertyName("total_usage")]
        public decimal TotalUsage { get; set; }


        public class DailyCostInfo
        {
            [JsonPropertyName("timestamp")]
            public double Timestamp { get; set; }

            [JsonPropertyName("line_items")]
            public LineItemInfo[]? LineItems { get; set; }
        }

        public class LineItemInfo
        {
            [JsonPropertyName("name")]
            public required string Name { get; set; }

            [JsonPropertyName("cost")]
            public double Cost { get; set; }
        }

    }
}

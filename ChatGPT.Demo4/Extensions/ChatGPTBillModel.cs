using System.Text.Json.Serialization;

namespace ChatGPT.Demo4.Extensions
{
    public class ChatGPTBillModel
    {

        [JsonPropertyName("object")]
        public required string Object { get; set; }

        [JsonPropertyName("has_payment_method")]
        public bool HasPaymentMethod { get; set; }

        [JsonPropertyName("canceled")]
        public bool Canceled { get; set; }

        [JsonPropertyName("canceled_at")]
        public string? CanceledAt { get; set; }

        [JsonPropertyName("delinquent")]
        public string? Delinquent { get; set; }

        /// <summary>
        /// key到期时间
        /// </summary>
        [JsonPropertyName("access_until")]
        public long AccessUntil { get; set; }

        [JsonPropertyName("soft_limit")]
        public int SoftLimit { get; set; }

        [JsonPropertyName("hard_limit")]
        public int HardLimit { get; set; }

        [JsonPropertyName("system_hard_limit")]
        public int SystemHardLimit { get; set; }

        [JsonPropertyName("soft_limit_usd")]
        public decimal SoftLimitUsd { get; set; }

        /// <summary>
        /// 账户额度
        /// </summary>
        [JsonPropertyName("hard_limit_usd")]
        public decimal HardLimitUsd { get; set; }

        [JsonPropertyName("system_hard_limit_usd")]
        public decimal SystemHardLimitUsd { get; set; }

        [JsonPropertyName("plan")]
        public PlanInfo? Plan { get; set; }

        [JsonPropertyName("primary")]
        public bool Primary { get; set; }

        [JsonPropertyName("account_name")]
        public string? AccountName { get; set; }

        [JsonPropertyName("po_number")]
        public string? PoNumber { get; set; }

        [JsonPropertyName("billing_email")]
        public string? BillingEmail { get; set; }

        [JsonPropertyName("tax_ids")]
        public string? TaxIds { get; set; }

        [JsonPropertyName("billing_address")]
        public string? BillingAddress { get; set; }

        [JsonPropertyName("business_address")]
        public string? BusinessAddress { get; set; }

        public class PlanInfo
        {
            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("id")]
            public string? Id { get; set; }
        }
    }
}

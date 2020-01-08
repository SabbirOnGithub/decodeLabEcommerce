using Newtonsoft.Json;

namespace Nop.Plugin.Payments.bKashAdvance.Models
{
    public class PaymentCreateModel
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }

        [JsonProperty(PropertyName = "intent")]
        public string Intent { get; set; }

        [JsonProperty(PropertyName = "merchantInvoiceNumber")]
        public string MerchantInvoiceNumber { get; set; }
    }

    public class RefundPostModel
    {
        [JsonProperty(PropertyName = "amount")]
        public string Amount { get; set; }

        [JsonProperty(PropertyName = "currency")]
        public string Currency { get; set; }
    }
}

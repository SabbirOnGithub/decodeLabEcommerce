using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Payza
{
    public class PayzaPaymentSettings : ISettings
    {
        public string GatewayUrl { get; set; }
        public string MerchantAccount { get; set; }
        public string SuccessUrl { get; set; }
        public string CancelUrl { get; set; }
    }
}

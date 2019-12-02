using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Dmoney
{
    public class DmoneyPaymentSettings : ISettings
    {
        public string GatewayUrl { get; set; }
        public string TransactionVerificationUrl { get; set; }
        public string OrgCode { get; set; } 
        public string Password { get; set; }
        public string SecretKey { get; set; }
        public string BillerCode { get; set; }
        public bool EnableLog { get; set; }
    }
}

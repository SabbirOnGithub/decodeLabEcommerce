using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Payments.EblSkyPay
{
    public class EblSkyPayPaymentSettings : ISettings
    {
        public string DescriptionText { get; set; }
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFeePercentage { get; set; }
        public bool UseSandbox { get; set; }
        public bool EnableLogging { get; set; }
        public string MerchantId { get; set; }
        public string MerchantName { get; set; }
        public string OperatorId { get; set; }
        public string Password { get; set; }
        public bool AutoRedirectEnable { get; set; }
        public bool UseSandBox { get; set; }
        public bool UseHttsForRedirection { get; set; }

        //version 2 (update)
        public bool Debug { get; set; }
        public bool UseSsl { get; set; }
        public bool IgnoreSslErrors { get; set; }
        public string GatewayUrl { get; set; }
        public bool UseProxy { get; set; }
        public string ProxyHost { get; set; }
        public string ProxyUser { get; set; }
        public string ProxyPassword { get; set; }
        public string ProxyDomain { get; set; }
    }
}

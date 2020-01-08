using Nop.Core.Configuration;
using System;

namespace Nop.Plugin.Payments.bKashAdvance
{
    public class BkashAdvancePaymentSettings : ISettings
    {
        public string AppKey { get; set; }
        public string AppSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFeePercentage { get; set; }
        public string Description { get; set; }
        public bool UseSandbox { get; set; }
        public bool EnableCapture { get; set; }

        public string IdToken { get; set; }
        public int ExpiresInSec { get; set; }
        public string RefreshToken { get; set; }
        public string TokenType { get; set; }
        public DateTime TokenCreateTime { get; set; }
        public bool EnableRefund { get; set; }
        public bool EnableVoid { get; set; }
    }
}

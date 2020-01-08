using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.Bkash
{
    public class BkashPaymentSettings:ISettings
    {
        public string DescriptionText { get; set; }
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFeePercentage { get; set; }

        public string DialingNumber { get; set; }
        public string CustomerSupportPhoneNumber{ get; set; }
        
        public string BkashSendMoneyPhoneNumber { get; set; }
        public string BkashPaymentPhoneNumber { get; set; }
    }
}
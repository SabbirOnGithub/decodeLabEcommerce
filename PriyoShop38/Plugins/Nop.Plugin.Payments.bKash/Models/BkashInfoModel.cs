using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Bkash.Models
{
    public class BkashInfoModel:BaseNopModel
    {
        public string DialNumber { get; set; }
        public string CustomerSupportPhoneNumber { get; set; }

        public string BkashSendMoneyPhoneNumber { get; set; }
        public string BkashPaymentPhoneNumber { get; set; }
    }
}
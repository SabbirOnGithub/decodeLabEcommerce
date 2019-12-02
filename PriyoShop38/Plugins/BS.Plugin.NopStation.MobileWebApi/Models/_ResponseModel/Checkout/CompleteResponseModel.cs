using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class CompleteResponseModel : BaseResponse
    {
        public int OrderId { get; set; }
        public bool CompleteOrder { get; set; }
        public PaypalModel PayPal { get; set; }
        public int PaymentType { get; set; }
        public decimal ConversionRate { get; set; }
        public string ProcessUrl { get; set; }

        public class PaypalModel
        {
            public string ClientId { get; set; }

        }
    }

}

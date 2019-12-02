using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions.Paypal
{
    public class PaypalDetailModel
    {
        public string PaymentStatus { get; set; }
        public string Total { get; set; }
        public string Currency { get; set; }
        public string PayeeId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.PayPal
{
    public class PayPalResponseModel
    {
        public int OrderId { get; set; }
        public string PaymentId { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{
    public enum PaymentType
    {
        CashOnDelivery = 1,
        PayPal = 2,
        AuthorizeDotNet=3,
        ReDirectType = 4,
        BkashAdvance = 5
    }
}

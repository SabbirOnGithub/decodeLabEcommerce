using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Extensions
{

    

    public enum AddressType
    {
        [Description("BillingNewAddress")]
        Billing = 1,
        [Description("ShippingNewAddress")]
        Shipping = 2,
        [Description("Address")]
        Address=3
    }
   
}

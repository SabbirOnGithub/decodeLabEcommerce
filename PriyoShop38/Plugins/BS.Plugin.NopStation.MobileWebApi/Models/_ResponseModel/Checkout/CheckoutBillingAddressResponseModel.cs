using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Models.Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public class CheckoutBillingAddressResponseModel : ExistingAddressCommonResponseModel
    {
         public CheckoutBillingAddressResponseModel()
        {
            NewAddress = new AddressModel();
        }
        public AddressModel NewAddress { get; set; }

        /// <summary>
        /// Used on one-page checkout page
        /// </summary>
        public bool NewAddressPreselected { get; set; }
    }
}

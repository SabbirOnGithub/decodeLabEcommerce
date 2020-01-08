﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Shipping;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Checkout
{
    public partial class CheckoutShippingMethodResponseModel : BaseResponse
    {
        public CheckoutShippingMethodResponseModel()
        {
            ShippingMethods = new List<ShippingMethodModel>();
            Warnings = new List<string>();
        }

        public IList<ShippingMethodModel> ShippingMethods { get; set; }

        public bool NotifyCustomerAboutShippingFromMultipleLocations { get; set; }

        public IList<string> Warnings { get; set; }

        #region Nested classes

        public partial class ShippingMethodModel : BaseNopModel
        {
            public string ShippingRateComputationMethodSystemName { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Fee { get; set; }
            public bool Selected { get; set; }

            public ShippingOption ShippingOption { get; set; }
        }
        #endregion
    }
}

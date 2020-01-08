using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public partial class CheckoutShippingAddressModel : BaseNopModel
    {
        public CheckoutShippingAddressModel()
        {
            //ExistingAddresses = new List<AddressModel>();
            //NewAddress = new AddressModel();

            Warnings = new List<string>();
            ExistingAddresses = new List<AddressModel>();
            NewAddress = new AddressModel();
            PickupPoints = new List<CheckoutPickupPointModel>();
        }
        public IList<string> Warnings { get; set; }
        public IList<AddressModel> ExistingAddresses { get; set; }

        public AddressModel NewAddress { get; set; }
        public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
        public bool NewAddressPreselected { get; set; }

        public bool AllowPickUpInStore { get; set; }
        public string PickUpInStoreFee { get; set; }
        public bool PickUpInStore { get; set; }
        public bool PickUpInStoreOnly { get; set; }
        public bool DisplayPickupPointsOnMap { get; set; }
        public string GoogleMapsApiKey { get; set; }
    }
}

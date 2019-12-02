using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Misc.OnePageCheckOut.Models.Common;

namespace Nop.Plugin.Misc.OnePageCheckOut.Models.Checkout
{
    public partial class CheckoutShippingAddressModel : BaseNopModel
    {
        public CheckoutShippingAddressModel()
        {
            Warnings = new List<string>();
            ExistingAddresses = new List<AddressModel>();
            NewAddress = new AddressModel();
            PickupPoints = new List<CheckoutPickupPointModel>();
        }
        public IList<string> Warnings { get; set; }
        public IList<AddressModel> ExistingAddresses { get; set; }

        public AddressModel NewAddress { get; set; }

        public bool NewAddressPreselected { get; set; }
        public IList<CheckoutPickupPointModel> PickupPoints { get; set; }
        public bool AllowPickUpInStore { get; set; }
        public string PickUpInStoreFee { get; set; }
        public bool PickUpInStore { get; set; }
        public bool PickUpInStoreOnly { get; set; }
        public int SelectedShippingAdressId { get; set; }
        public bool DisplayPickupPointsOnMap { get; set; }
        public string GoogleMapsApiKey { get; set; }
    }
}
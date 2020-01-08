using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Models.Common;


namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public partial class CustomerAddressListModel : BaseNopModel
    {
        public CustomerAddressListModel()
        {
            Addresses = new List<AddressModel>();
        }

        public IList<AddressModel> Addresses { get; set; }
    }
}
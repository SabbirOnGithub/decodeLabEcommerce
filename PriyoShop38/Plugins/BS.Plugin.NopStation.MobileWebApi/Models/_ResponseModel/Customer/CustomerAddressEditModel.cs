using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Common;


namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer
{
    public partial class CustomerAddressEditModel : BaseNopModel
    {
        public CustomerAddressEditModel()
        {
            this.Address = new AddressModel();
        }
        public AddressModel Address { get; set; }
    }
}
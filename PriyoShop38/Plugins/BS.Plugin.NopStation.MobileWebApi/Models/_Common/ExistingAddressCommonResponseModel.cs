using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Web.Models.Common;

namespace BS.Plugin.NopStation.MobileWebApi.Models._Common
{
    public class ExistingAddressCommonResponseModel : BaseResponse
    {
        public ExistingAddressCommonResponseModel()
        {
            ExistingAddresses= new List<AddressModel>();
        }
        public IList<AddressModel> ExistingAddresses { get; set; }
    }
}

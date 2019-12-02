using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class AddProductToCartResponseModel : BaseResponse
    {
        public bool Success { get; set; }
        public bool ForceRedirect { get; set; }
        public int Count { get; set; }
    }
}

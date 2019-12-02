using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class ProductDetailPriceResponseModel : BaseResponse
    {
        public string Sku { get; set; }
        public string Price { get; set; }
        public string Gtin { get; set; }
        public string Mpn { get; set; }
    }
}

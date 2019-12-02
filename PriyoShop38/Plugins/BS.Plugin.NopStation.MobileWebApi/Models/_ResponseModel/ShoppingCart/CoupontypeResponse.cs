using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class CoupontypeResponse :BaseResponse
    {
        public CoupontypeResponse()
        {
            OrderTotalResponseModel = new OrderTotalsResponseModel();
        }
        public bool Data { get; set; }
        public OrderTotalsResponseModel OrderTotalResponseModel { get; set; }
    }
}

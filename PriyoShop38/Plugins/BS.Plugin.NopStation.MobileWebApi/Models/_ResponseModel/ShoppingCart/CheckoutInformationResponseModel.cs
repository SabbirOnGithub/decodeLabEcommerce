using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class CheckoutInformationResponseModel : BaseResponse
    {
        public CheckoutInformationResponseModel()
        {
            OrderTotalModel= new OrderTotalsResponseModel();
            ShoppingCartModel=new ShoppingCartResponseModel();
        }

        public OrderTotalsResponseModel OrderTotalModel;
        public ShoppingCartResponseModel ShoppingCartModel;
    }
}

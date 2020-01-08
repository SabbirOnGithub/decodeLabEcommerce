using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.ShoppingCart
{
    public class PurchaseOfferResponseModel : BaseResponse
    {
        public PurchaseOfferResponseModel()
        {
            GiftModel = new GiftItemModel();
            AvailableOptions = new List<GiftItemModel>();
        }

        public bool OfferActive { get; set; }

        public bool GiftAvailable { get; set; }

        public List<GiftItemModel> AvailableOptions { get; set; }

        public GiftItemModel GiftModel { get; set; }

        public class GiftItemModel
        {
            public string ImageUrl { get; set; }
            public string GiftItemName { get; set; }
            public int Quantity { get; set; }
            public string GiftNote { get; set; }
            public string MinimumPurchaseAmount { get; set; }
        }
    }
}

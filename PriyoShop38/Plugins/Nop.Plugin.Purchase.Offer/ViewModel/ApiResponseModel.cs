using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using System.Collections.Generic;

namespace Nop.Plugin.Purchase.Offer.ViewModel
{
    public class ApiResponseModel : BaseResponse
    {
        public ApiResponseModel()
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

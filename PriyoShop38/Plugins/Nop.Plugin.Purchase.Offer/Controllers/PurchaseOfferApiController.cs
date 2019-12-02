using BS.Plugin.NopStation.MobileWebApi.Controllers;
using System.Linq;
using Nop.Plugin.Purchase.Offer.ViewModel;
using Nop.Plugin.Widgets.CustomFooter.Data;
using Nop.Web.Framework.Kendoui;
using System.Web.Http;

namespace Nop.Plugin.Purchase.Offer.Controllers
{
    public class PurchaseOfferApiController : WebApiController
    {
        private readonly IPurchaseOfferService _purchaseOfferService;

        public PurchaseOfferApiController(IPurchaseOfferService purchaseOfferService)
        {
            _purchaseOfferService = purchaseOfferService;
        }

        [System.Web.Http.Route("api/purchase-offer")]
        [System.Web.Http.HttpGet]
        public IHttpActionResult PublicInfo()
        {
            var publicInfo = _purchaseOfferService.GetPublicInfo();
            var options = _purchaseOfferService.GetOptions();

            var model = new ApiResponseModel()
            {
                OfferActive = publicInfo.OfferActive
            };

            if (publicInfo.OfferActive)
            {
                if (publicInfo.OfferAvailable)
                {
                    model.GiftAvailable = publicInfo.OfferAvailable;
                    model.GiftModel.ImageUrl = publicInfo.ProductImage;
                    model.GiftModel.GiftItemName = publicInfo.ProductName;
                    model.GiftModel.Quantity = publicInfo.Quantity;
                    model.GiftModel.GiftNote = publicInfo.Note;
                    model.GiftModel.MinimumPurchaseAmount = publicInfo.MinimumPurchaseAmountStr;
                }

                if (options != null && options.Any())
                {
                    foreach (var option in options)
                    {
                        model.AvailableOptions.Add(new ApiResponseModel.GiftItemModel()
                        {
                            ImageUrl = option.ProductImage,
                            GiftItemName = option.ProductName,
                            Quantity = option.Quantity,
                            GiftNote = option.Note,
                            MinimumPurchaseAmount = option.MinimumPurchaseAmountStr
                        });
                    }
                }
            }

            return Ok(model);
        }
    }
}

using Nop.Plugin.Purchase.Offer.ViewModel;
using Nop.Plugin.Widgets.CustomFooter.Data;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Purchase.Offer.Controllers
{
    public class PurchaseOfferController : BasePluginController
    {
        private readonly IPurchaseOfferService _purchaseOfferService;

        public PurchaseOfferController(IPurchaseOfferService purchaseOfferService)
        {
            _purchaseOfferService = purchaseOfferService;
        }

        public ActionResult Settings()
        {
            var model = _purchaseOfferService.GetOfferDetails();
            return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/Settings.cshtml", model);
        }

        public ActionResult Configure()
        {
            var model = _purchaseOfferService.GetOfferDetails();
            return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult Configure(PurchaseOfferViewModel model)
        {
            if(_purchaseOfferService.CreateOrUpdateOffer(model))
                SuccessNotification("Data updated successfully!");
            else
                ErrorNotification("Data updated successfully!");
            return RedirectToAction("Settings");
        }

        [HttpPost]
        public ActionResult GetOptions(DataSourceRequest command, PurchaseOfferOptionViewModel model)
        {
            var data = _purchaseOfferService.GetOptions();
            var gridModel = new DataSourceResult
            {
                Data = data.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize),
                Total = data.Count()
            };
            return Json(gridModel);
        }

        public ActionResult OptionAddPopup(int offerId, int? optionId, string btnId, string formId)
        {
            ViewBag.formId = formId;
            ViewBag.btnId = btnId;
            var model = _purchaseOfferService.GetPopUpModel(optionId);
            return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/OptionAddPopup.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult OptionAddPopup(string btnId, string formId, AddOptionModel model)
        {
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            _purchaseOfferService.AddOrUpdateOption(model);
            return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/OptionAddPopup.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult DeleteOption(int id)
        {
            _purchaseOfferService.DeleteOption(id);
            return new NullJsonResult();
        }

        public ActionResult PublicInfo(string widgetZone)
        {
            var model = _purchaseOfferService.GetPublicInfo();
            if (widgetZone == "purchase_offer_order_summary")
                return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/PublicInfo.cshtml", model);

            var offer = _purchaseOfferService.GetOfferDetails();
            if (offer != null && offer.IsActive && offer.ValidFrom <= DateTime.UtcNow && offer.ValidTo >= DateTime.UtcNow)
            {
                offer.Options = _purchaseOfferService.GetOptions();
                offer.GainedOption = model;
            }
            else
                offer = null;
            return View("~/Plugins/Purchase.Offer/Views/PurchaseOffer/OfferList.cshtml", offer);
        }
    }
}

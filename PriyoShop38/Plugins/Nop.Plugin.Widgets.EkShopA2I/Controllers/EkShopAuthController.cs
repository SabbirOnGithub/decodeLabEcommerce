using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Plugin.Widgets.EkShopA2I.Models;
using Nop.Plugin.Widgets.EkShopA2I.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Web.Framework.Controllers;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.EkShopA2I.Controllers
{
    public class EkshopAuthController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IEkshopEpService _esEpService;
        private readonly EkshopSettings _ekShopA2ISettings;
        private readonly ILocalizationService _localizationService;
        private readonly ILogger _logger;

        public EkshopAuthController(ISettingService settingService,
            IWorkContext workContext,
            IEkshopEpService esEpService,
            EkshopSettings ekShopA2ISettings,
            ILocalizationService localizationService,
            ILogger logger)
        {
            this._ekShopA2ISettings = ekShopA2ISettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._workContext = workContext;
            this._esEpService = esEpService;
            this._logger = logger;
        }

        public ActionResult Create(string auth_token, string redirect_url, string ekshop_api_base_url, string secret)
        {
            if (_esEpService.TryCreateSession(auth_token, ekshop_api_base_url))
                return Redirect(redirect_url);

            ErrorNotification("Failed to create session");
            return RedirectToRoute("HomePage");
        }

        public ActionResult SetCart()
        {
            if (_esEpService.TrySetCart(out string redirectUrl))
                return Redirect(redirectUrl);

            return RedirectToRoute("HomePage");
        }

        public ActionResult Public(string widgetZone, object additionalData = null, string area = null)
        {
            if (!string.IsNullOrWhiteSpace(_workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.EkshopSessionToken)))
            {
                if (widgetZone == "footer")
                    return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/Public.cshtml");
                if (widgetZone == "productbox_ekshop_commission" || widgetZone == "productdetails_before_pictures")
                {
                    var model = new NotificationModel();
                    model.CommissionRate = _esEpService.GetCommissionRate((int)additionalData);

                    if (widgetZone == "productbox_ekshop_commission")
                        model.ShowBadge = _ekShopA2ISettings.ShowUdcCommissionOnProductBox;
                    else
                        model.ShowBadge = _ekShopA2ISettings.ShowUdcCommissionOnProductDetails;

                    model.IsVendorRestricted = _esEpService.IsVendorRestricted((int)additionalData);
                    model.ProductId = (int)additionalData;

                    if (model.IsVendorRestricted)
                        model.Message = _localizationService.GetResource("Ekshop.VendorRestrictionMessage");
                    return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/CommissionBadge.cshtml", model);
                }
            }

            return Content("");
        }
    }
}

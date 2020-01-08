using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.NexusPay.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Payments.NexusPay.Controllers
{
    public class PaymentNexusPayController : BasePaymentController
    {
        #region Fields

        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly ILanguageService _languageService;

        #endregion

        #region Ctor

        public PaymentNexusPayController(IWorkContext workContext,
            IStoreService storeService, 
            ISettingService settingService,
            IStoreContext storeContext,
            ILocalizationService localizationService,
            ILanguageService languageService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._languageService = languageService;
        }

        #endregion

        #region Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var NexusPayPaymentSettings = _settingService.LoadSetting<NexusPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel {DescriptionText = NexusPayPaymentSettings.DescriptionText};

            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.DescriptionText = NexusPayPaymentSettings.GetLocalizedSetting(x => x.DescriptionText, languageId, false, false);
            });

            model.AdditionalFee = NexusPayPaymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = NexusPayPaymentSettings.AdditionalFeePercentage;
            model.ShippableProductRequired = NexusPayPaymentSettings.ShippableProductRequired;
            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.DescriptionText_OverrideForStore = _settingService.SettingExists(NexusPayPaymentSettings, x => x.DescriptionText, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(NexusPayPaymentSettings, x => x.AdditionalFee, storeScope);
                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(NexusPayPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ShippableProductRequired_OverrideForStore = _settingService.SettingExists(NexusPayPaymentSettings, x => x.ShippableProductRequired, storeScope);
            }

            return View("~/Plugins/Payments.NexusPay/Views/PaymentNexusPay/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var NexusPayPaymentSettings = _settingService.LoadSetting<NexusPayPaymentSettings>(storeScope);

            //save settings
            NexusPayPaymentSettings.DescriptionText = model.DescriptionText;
            NexusPayPaymentSettings.AdditionalFee = model.AdditionalFee;
            NexusPayPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            NexusPayPaymentSettings.ShippableProductRequired = model.ShippableProductRequired;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
             _settingService.SaveSettingOverridablePerStore(NexusPayPaymentSettings, x=>x.DescriptionText, model.DescriptionText_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NexusPayPaymentSettings, x => x.AdditionalFee, model.AdditionalFee_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NexusPayPaymentSettings, x => x.AdditionalFeePercentage, model.AdditionalFeePercentage_OverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NexusPayPaymentSettings, x => x.ShippableProductRequired, model.ShippableProductRequired_OverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //localization. no multi-store support for localization yet.
            foreach (var localized in model.Locales)
            {
                NexusPayPaymentSettings.SaveLocalizedSetting(x => x.DescriptionText,
                    localized.LanguageId,
                    localized.DescriptionText);
            }

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var NexusPayPaymentSettings = _settingService.LoadSetting<NexusPayPaymentSettings>(_storeContext.CurrentStore.Id);

            var model = new PaymentInfoModel
            {
                DescriptionText = NexusPayPaymentSettings.GetLocalizedSetting(x=>x.DescriptionText, _workContext.WorkingLanguage.Id)
            };

            return View("~/Plugins/Payments.NexusPay/Views/PaymentNexusPay/PaymentInfo.cshtml", model);
        }

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();

            return paymentInfo;
        }

        [OutputCache(Duration = 1 * 60)]
        public ActionResult HowToPay()
        {
            return View("~/Plugins/Payments.NexusPay/Views/PaymentNexusPay/HowToPay.cshtml");
        }

        #endregion
    }
}
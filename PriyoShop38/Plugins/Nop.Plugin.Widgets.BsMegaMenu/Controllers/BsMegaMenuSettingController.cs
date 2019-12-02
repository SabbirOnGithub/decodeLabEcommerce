using System.Web.Mvc;
using Nop.Admin.Controllers;
using Nop.Plugin.Widgets.BsMegaMenu.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;

using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.BsMegaMenu.Controllers
{
    public class BsMegaMenuSettingController : BaseAdminController
    {


        private readonly ISettingService _settingService;
        private readonly ILocalizationService _localizationService;


        public BsMegaMenuSettingController(ISettingService settingService, ILocalizationService localizationService)
        {
            this._settingService = settingService;
            this._localizationService = localizationService;
        }


        #region Configure Action

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {

            var model = new BsMegaMenuSettingsModel();
            var BsMegaMenuSettings = _settingService.LoadSetting<BsMegaMenuSettings>();

            if(BsMegaMenuSettings.noOfMenufactureItems < 1)
            {
                BsMegaMenuSettings.noOfMenufactureItems = 1;
            }

            
            model.SetWidgetZone = BsMegaMenuSettings.SetWidgetZone;
            model.ShowImage = BsMegaMenuSettings.ShowImage;
            model.MenuType = BsMegaMenuSettings.MenuType;
            model.noOfMenufactureItems = BsMegaMenuSettings.noOfMenufactureItems;
            return View("~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenuConfiguare.cshtml", model);
        }

        [AdminAuthorize]
        [ChildActionOnly]
        [HttpPost]
        public ActionResult Configure(BsMegaMenuSettingsModel model)
        {
            var settings = new BsMegaMenuSettings()
            {
                
                SetWidgetZone = model.SetWidgetZone,
                ShowImage = model.ShowImage,
                MenuType = model.MenuType,
                noOfMenufactureItems = model.noOfMenufactureItems
            };

            _settingService.SaveSetting(settings);
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

     
        #endregion

    }
}
using System.Web.Mvc;
using Nop.Plugin.Widgets.FacebookChat.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using System.Globalization;
using Nop.Core;
using Nop.Services.Stores;

namespace Nop.Plugin.Widgets.FacebookChat.Controllers
{
    public class WidgetsFacebookChatController : BasePluginController
    {
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        
        public WidgetsFacebookChatController(IWorkContext workContext,
            IStoreContext storeContext, IStoreService storeService, ISettingService settingService)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
        }
        
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookChatSettings = _settingService.LoadSetting<FacebookChatSettings>(storeScope);
            var model = new FacebookChatModel()
            {
                FacebookChatAppId = facebookChatSettings.FacebookChatAppId,
                FacebookChatPageId = facebookChatSettings.FacebookChatPageId,
                ThemeColor = facebookChatSettings.ThemeColor
            };
            return View("~/Plugins/Widgets.FacebookChat/Views/WidgetsFacebookChat/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(FacebookChatModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookChatSettings = _settingService.LoadSetting<FacebookChatSettings>(storeScope);
            facebookChatSettings.FacebookChatPageId = model.FacebookChatPageId;
            facebookChatSettings.FacebookChatAppId = model.FacebookChatAppId;
            facebookChatSettings.ThemeColor = model.ThemeColor;

            _settingService.SaveSetting(facebookChatSettings);
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookChatSettings = _settingService.LoadSetting<FacebookChatSettings>(storeScope);

            var themeColor = "";
            if (!string.IsNullOrWhiteSpace(facebookChatSettings.ThemeColor))
            {
                themeColor = "theme_color=\"" + facebookChatSettings.ThemeColor + "\" ";
            }
            var text = "<style>.fb_dialog.fb_dialog_advanced, .fb_dialog.fb_dialog_mobile {\r\n    right: 75px !important;\r\n    bottom: 15px !important;\r\n}\r\n</style>" +
                "<div class=\"fb-customerchat\" " +
                     "page_id=\"" + facebookChatSettings.FacebookChatPageId + "\" " + themeColor + "" +
                     "minimized=\"true\">" +
                     "</div>\r\n\r\n" +
                     "<script>\r\n" +
                     "  window.fbAsyncInit = function() {\r\n" +
                     "    FB.init({\r\n" +
                     "      appId            : '" + facebookChatSettings.FacebookChatAppId + "', \r\n" +
                     "      autoLogAppEvents : true,\r\n" +
                     "      xfbml            : true,\r\n" +
                     "      version          : 'v2.11'\r\n" +
                     "    });\r\n" +
                     "  };\r\n" +
                     "(function(d, s, id){\r\n" +
                     "     var js, fjs = d.getElementsByTagName(s)[0];\r\n" +
                     "     if (d.getElementById(id)) {return;}\r\n" +
                     "     js = d.createElement(s); js.id = id;\r\n" +
                     "     js.src = 'https://connect.facebook.net/en_US/sdk.js';\r\n" +
                     "     fjs.parentNode.insertBefore(js, fjs);\r\n" +
                     "   }(document, 'script', 'facebook-jssdk'));\r\n" +
                     "</script>";

            return Content(text);
        }
    }
}
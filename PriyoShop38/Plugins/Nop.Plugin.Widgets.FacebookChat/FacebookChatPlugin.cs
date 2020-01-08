using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.FacebookChat
{
    public class FacebookChatPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly FacebookChatSettings _facebookChatSettings;

        public FacebookChatPlugin(ISettingService settingService,
            FacebookChatSettings facebookChatSettings)
        {
            _settingService = settingService;
            _facebookChatSettings = facebookChatSettings;
        }
        
        public IList<string> GetWidgetZones()
        {
            return new List<string>()
            { 
                "body_end_html_tag_before", 
                "mobile_body_end_html_tag_before" 
            };
        }
        
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsFacebookChat";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.FacebookChat.Controllers" }, { "area", null } };
        }
        
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsFacebookChat";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.FacebookChat.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
        
        public override void Install()
        {
            var settings = new FacebookChatSettings()
            {
                FacebookChatAppId = "506808346366325",
                FacebookChatPageId = "1630780507175219",
                ThemeColor = "#e30047"
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.ThemeColor", "Theme Color");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.ThemeColor.Hint", "Theme color code should be in hexadecimal value (Except white).");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatAppId", "Facebook App ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatAppId.Hint", "Paste the Facebook App ID here, and replace hard coded values by tokens.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatPageId", "Facebook Page ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatPageId.Hint", "Paste the Facebook Page ID here, and replace hard coded values by tokens.");

            base.Install();
        }
        
        public override void Uninstall()
        {
            _settingService.DeleteSetting<FacebookChatSettings>();

            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.ThemeColor");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.ThemeColor.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatAppId");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatAppId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatPageId");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookChat.FacebookChatPageId.Hint");

            base.Uninstall();
        }
    }
}

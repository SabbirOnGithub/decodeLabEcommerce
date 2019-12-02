using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Domain.Cms;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;

namespace Nop.Plugin.Widgets.FacebookPixel
{
    /// <summary>
    /// Live person provider
    /// </summary>
    public class FacebookPixelPlugin : BasePlugin, IWidgetPlugin
    {

        public const string FACEBOOKPIXEL_SCRIPT_FIRSTPART = @"<!-- Facebook Pixel Code -->
<script>
    !function (f, b, e, v, n, t, s) {
        if (f.fbq) return; n = f.fbq = function () {
            n.callMethod ?
            n.callMethod.apply(n, arguments) : n.queue.push(arguments)
        }; if (!f._fbq) f._fbq = n;
        n.push = n; n.loaded = !0; n.version = '2.0'; n.queue = []; t = b.createElement(e); t.async = !0;
        t.src = v; s = b.getElementsByTagName(e)[0]; s.parentNode.insertBefore(t, s)
    }(window,
    document, 'script', '//connect.facebook.net/en_US/fbevents.js');

    fbq('init', '1769980523222843')";



        public const string FACEBOOKPIXEL_SCRIPT_LASTPART = @"fbq('track', ""PageView"");</script>
<noscript>
    <img height=""1"" width=""1"" style=""display:none""
         src=""https://www.facebook.com/tr?id=1769980523222843&ev=PageView&noscript=1"" />
</noscript>
<!-- End Facebook Pixel Code -->";

        private readonly ISettingService _settingService;
        private readonly FacebookPixelSettings _facebookPixelSettings;

        public FacebookPixelPlugin(ISettingService settingService,
            FacebookPixelSettings facebookPixelSettings)
        {
            this._settingService = settingService;
            this._facebookPixelSettings = facebookPixelSettings;
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>()
            { 
                //desktop version (you can also replace it with "head_html_tag")
                "body_end_html_tag_before", 
                //mobile version
                "mobile_body_end_html_tag_before" 
            };
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsFacebookPixel";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.FacebookPixel.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsFacebookPixel";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.FacebookPixel.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            var settings = new FacebookPixelSettings()
            {

                FacebookPixelScriptFirstPart = FACEBOOKPIXEL_SCRIPT_FIRSTPART,
                FacebookPixelScriptLastPart = FACEBOOKPIXEL_SCRIPT_LASTPART,
                
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookPixel.Configure.Description", @"

<ul>
    <li>The Facebook Pixel code divided into two part </li>
    <li>First parts start from &#60;&#33;&#45;&#45; Facebook Pixel Code &#45;&#45;&#62; and end fbq('init', '111111111111');</li>
    <li>Second parts start from fbq('track', ""PageView"");  and end &#60;&#33;&#45;&#45; End Facebook Pixel Code &#45;&#45;&#62;;</li>
    <li>Two script Part put into the two different text feld</li>
    <li>Save</li>
</ul>

");
          
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptFirstPart", "FacebookPixel Script_First");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptFirstPart.Hint", "Paste the FacebookPixel Script here, and replace hard coded values by tokens. http will be automatically replaced with https if necessary.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptLastPart", "FacebookPixel Script_Last");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptLastPart.Hint", "Paste the FacebookPixel Script Last  here, and replace hard coded values by tokens. http will be automatically replaced with https if necessary.");

            base.Install();
        }

         ///<summary>
         ///Uninstall plugin
         ///</summary>
        public override void Uninstall()
        {
            _settingService.DeleteSetting<FacebookPixelSettings>();
            //locales
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptFirstPart");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptFirstPart.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptLastPart");
            this.DeletePluginLocaleResource("Plugins.Widgets.FacebookPixel.FacebookPixelScriptLastPart.Hint");

            base.Uninstall();
        }
    }
}

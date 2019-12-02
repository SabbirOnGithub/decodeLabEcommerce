using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Data;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using BS.Plugin.NopStation.MobileWebApi.Services;
using System.Linq;
using Nop.Services.Security;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;

namespace BS.Plugin.NopStation.MobileWebApi
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class MobileWebApiPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin //, IWidgetPlugin
    {
        #region Fields

        private readonly IBsNopMobilePluginService _BsPluginService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IContentManagementTemplateService _contentmanagementTemplate;
        private readonly MobileWebApiObjectContext _context;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctor
        public MobileWebApiPlugin(IBsNopMobilePluginService BsPluginService,
            ISettingService settingService,
            IWorkContext workContext,
            MobileWebApiSettings webapiSettings,
            IContentManagementTemplateService contentmanagementTemplate,
            MobileWebApiObjectContext context,
            IPermissionService permissionService)
        {
            this._BsPluginService = BsPluginService;
            this._settingService = settingService;
            this._workContext = workContext;
            this._contentmanagementTemplate = contentmanagementTemplate;
            this._context = context;
            this._permissionService = permissionService;
        }

        #endregion

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.NopStationApiPluginManage))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = "Mobile Web Api",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                    IconClass = "fa-mobile"
                };

                var menusixthItem = new SiteMapNode()
                {
                    Title = "Category Icons",
                    ControllerName = "MobileWebApiConfiguration",
                    ActionName = "CategoryIconList",
                    Visible = true,
                    SystemName = "MobileWebApi.CategoryIcons",
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                    IconClass = "fa-genderless"
                };
                menuItem.ChildNodes.Add(menusixthItem);

                var sliderInfoItem = new SiteMapNode()
                {
                    Title = "Slider",
                    ControllerName = "MobileWebApiConfiguration",
                    ActionName = "SliderImage",
                    Visible = true,
                    SystemName = "MobileWebApi.SliderImage",
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                    IconClass = "fa-genderless"
                };
                menuItem.ChildNodes.Add(sliderInfoItem);

                var nstSettings = new SiteMapNode()
                {
                    Title = "NST_Settings",
                    ControllerName = "MobileWebApiConfiguration",
                    ActionName = "NopStationSecrateToken",
                    Visible = true,
                    SystemName = "MobileWebApi.NopStationSecrateToken",
                    RouteValues = new RouteValueDictionary() { { "area", null } },
                    IconClass = "fa-genderless"
                };
                menuItem.ChildNodes.Add(nstSettings);

                var apiSettings = new SiteMapNode()
                {
                    Title = "Api Settings",
                    ControllerName = "MobileWebApiConfiguration",
                    ActionName = "ApiSetting",
                    Visible = true,
                    SystemName = "MobileWebApi.ApiSetting",
                    RouteValues = new RouteValueDictionary() { { "area", null } },
                    IconClass = "fa-genderless"
                };
                menuItem.ChildNodes.Add(apiSettings);

                var homePageCategory = new SiteMapNode()
                {
                    Title = "Home page catgory",
                    ControllerName = "MobileWebApiConfiguration",
                    ActionName = "HomePageCategory",
                    Visible = true,
                    SystemName = "MobileWebApi.HomePageCategory",
                    RouteValues = new RouteValueDictionary() { { "area", null } },
                    IconClass = "fa-genderless"
                };
                menuItem.ChildNodes.Add(homePageCategory);

                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Nop Station");
                if (pluginNode != null)
                    pluginNode.ChildNodes.Add(menuItem);
                else
                {
                    var nopStation = new SiteMapNode()
                    {
                        Visible = true,
                        Title = "Nop Station",
                        Url = "",
                        SystemName = "Nop Station",
                        IconClass = "fa-folder-o"
                    };
                    rootNode.ChildNodes.Add(nopStation);
                    nopStation.ChildNodes.Add(menuItem);
                }

                //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
                //if (pluginNode != null)
                //{
                //    pluginNode.ChildNodes.Add(menuItem);
                //}
                //else
                //{
                //    rootNode.ChildNodes.Add(menuItem);
                //}
            }
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
          //  actionName = "Configure";
            actionName = "GeneralSetting";
            controllerName = "MobileWebApiConfiguration";
            routeValues = new RouteValueDictionary()
            {
                { "Namespaces", "BS.Plugin.NopStation.MobileWebApi.Controllers" }, 
                { "area", null }
            };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "admin_webapi_bottom" };

        }
        public bool Authenticate()
        {
            return true;
        }


        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Dashboard";
            controllerName = "MobileWebApiConfiguration";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "BS.Plugin.NopStation.MobileWebApi.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public override void Install()
        {
            //settings
            var settings = new MobileWebApiSettings()
            {
                //ProductPictureSize = 125,
                //PassShippingInfo = false,
                //StaticFileName = string.Format("froogle_{0}.xml", CommonHelper.GenerateRandomDigitCode(10)),
            };
            _settingService.SaveSetting(settings);
            var nstsettings = new NstSettingsModel()
            {
                NST_KEY = "Nop_Station",
                NST_SECRET = "BrainStation"
            };
            _settingService.SaveSetting(nstsettings);

            #region Local Resources

            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NopMobilemenuMainTitle", "NopMobileWebApiConfigration");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Willusedefaultnopcategory", "Will use default nopcategory?");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.DefaultNopFlowSameAs", "Will use default nopflow same as?");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory", "Select a Sub Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.CategoryIcon", "Upload Category Icon");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ActivatePushNotification", "Activate push notification");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SandboxMode", "Sandbox mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.GoogleApiProjectNumber", "Google api project number");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.UploudeIOSPEMFile", "Uploude IOS PEMFile");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PEMPassword", "PEM password");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnGooglePlayStore", "App name on google playstore");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlOnGooglePlayStore", "App URL on google playstore");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnAppleStore", "App name on Applestore");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlonAppleStore", "App URL on Applestore");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppDescription", "App description");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppImage", "App image");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogo", "App Logo");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogoAltText", "App Logo Alternate Text");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.GcmApiKey", "Gcm Apikey");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppKey", "AppKey");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppName", "App name");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.LicenceType", "Licence type");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.CreatedDate", "Created date");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AndroidAppStatus", "Android app status");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.DownloadUrl", "Download url");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.iOsAPPUDID", "Ios APPUDID");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.MobilWebsiteURL", "Mobil website URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationHeading", "Push notification heading");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationMessage", "Push notification message");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColor", "Header background color");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColor", "Header font and icon color");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColor", "High lighted text color");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColor", "Primary text color");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColor", "Secondary text color");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButton", "Background color of primary button");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButton", "Text color of primary Button");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButton", "Background color of secondary button");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.menu", "NopMobile Menu");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.overview", "Overview");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.generalsetting", "General Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.themepersonalization", "Theme Personalization");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannericon", "Banner Slide");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.contantmanegement", "Content Management");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotification", "Push Notification");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryicon", "Category Icon");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesetting", "Mobile Website Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColorHint", "Configure background color for heaer of the app");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColorHint", "Configure font and icon color for heaer of the app");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColorHint", "Configure font color for highlighted text message such warning,product,price etc.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColorHint", "Configure font color for primary text labels such as product name,category name etc.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColorHint", "Configure font color for secondary text labels such as option name,no. of rating etc.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButtonHint", "Configure background color for all primary button such as Add to Cart etc");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButtonHint", "Configure font color for all primary button such as Add to Cart etc");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButtonHint", "Configure background color for all secondary button");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.persionalizewebsiteTitle", "Personalization [Website]");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryiconTitle", "Configure Category Icon");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.configureTitle", "Configure Plugin");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.contentmanagementTitle", "ConfigureContent Management");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannerconfigureTitle", "Configure Content Banner");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesettingTitle", "Configure Mobile Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotificationTitle", "Configure Push Notification");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture1", "Picture 1");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture2", "Picture 2");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture3", "Picture 3");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture4", "Picture 4");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture5", "Picture 5");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture", "Picture");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture.Hint", "Upload picture.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text", "Comment");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text.Hint", "Enter comment for picture. Leave empty if you don't want to display any text.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link", "URL");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link.Hint", "Enter URL. Leave empty if you don't want this picture to be clickable.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct", "Is Product");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct.Hint", "Checked, if you enter Product Id or Unchecked for Category Id.");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat", "Product or Category Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat.Hint", "Enter your product or category id");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableBestseller", "Enable Bestseller Home Page");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableFeaturedProducts", "Enable Featured Products Home Page");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableNewProducts", "Enable New Products Home Page");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.AddNew", "Add New FeturedProduct");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.sliderImage", "Slider Image");
            this.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.StartDate", "Start Date");
            this.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.EndDate", "End Date");
            this.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.IsProduct", "Is Product");
            this.AddOrUpdatePluginLocaleResource("Slider.Products.Fields.ProductOrCategory", "Product or Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopMobile.WebApi.Slider", "Configure Slider Image");
            this.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.slideractivestartdate", "Slider Active Start Date");
            this.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.slideractiveenddate", "Slider Active End Date");
            this.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.isproduct", "Is Product");
            this.AddOrUpdatePluginLocaleResource("admin.bsslider.fields.productorcategory", "Product Or Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY", "NST_Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET", "NST_Secret");
            #endregion

            //install db
            _context.Install();

            //base install
            base.Install();
        }
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            #region Local Resources

            //settings

            _settingService.DeleteSetting<MobileWebApiSettings>();

            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NopMobilemenuMainTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Willusedefaultnopcategory");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SubCategory");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.CategoryIcon");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.DefaultNopFlowSameAs");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ActivatePushNotification");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SandboxMode");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.GoogleApiProjectNumber");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.UploudeIOSPEMFile");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PEMPassword");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnGooglePlayStore");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlOnGooglePlayStore");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppNameOnAppleStore");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppUrlonAppleStore");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppDescription");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppImage");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogo");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.GcmApiKey");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppKey");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppName");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.LicenceType");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.CreatedDate");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AndroidAppStatus");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.DownloadUrl");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.iOsAPPUDID");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.MobilWebsiteURL");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationHeading");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PushNotificationMessage");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColor");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColor");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColor");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColor");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColor");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButton");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButton");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButton");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.menu");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.overview");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.generalsetting");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannericon");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.contantmanegement");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotification");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryicon");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesetting");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderBackgroundColorHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HeaderFontandIconColorHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.HighlightedTextColorHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.PrimaryTextColorHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.SecondaryTextColorHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofPrimaryButtonHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.TextColorofPrimaryButtonHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.BackgroundColorofSecondaryButtonHint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.persionalizewebsiteTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.categoryiconTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.configureTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.contentmanagementTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.mobilewebsitesettingTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.pushnotificationTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture1");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture2");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture3");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture4");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture5");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Picture.Hint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Text.Hint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.Link.Hint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.IsProduct.Hint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.ProdOrCat.Hint");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.bannerconfigureTitle");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableBestseller");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableFeaturedProducts");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.EnableNewProducts");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AddNew");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.AppLogoAltText");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.sliderImage");
            this.DeletePluginLocaleResource("Slider.Products.Fields.StartDate");
            this.DeletePluginLocaleResource("Slider.Products.Fields.EndDate");
            this.DeletePluginLocaleResource("Slider.Products.Fields.IsProduct");
            this.DeletePluginLocaleResource("Slider.Products.Fields.ProductOrCategory");
            this.DeletePluginLocaleResource("Plugins.NopMobile.WebApi.Slider");
            this.DeletePluginLocaleResource("admin.bsslider.fields.slideractivestartdate");
            this.DeletePluginLocaleResource("admin.bsslider.fields.slideractiveenddate");
            this.DeletePluginLocaleResource("admin.bsslider.fields.isproduct");
            this.DeletePluginLocaleResource("admin.bsslider.fields.productorcategory");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_KEY");
            this.DeletePluginLocaleResource("Plugins.NopStation.MobileWebApi.NstSettingsModel.NST_SECRET");
            #endregion

            _context.Uninstall();
            base.Uninstall();
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        /// 
       

    }
}

using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    public class AlgoliaSearchPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly AlgoliaSettings _algoliaSettings;
        private readonly IWebHelper _webHelper;

        private const string HeaderWidget = "header";

        #endregion

        #region Ctr

        public AlgoliaSearchPlugin(ILocalizationService localizationService,
            ISettingService settingService,
            AlgoliaSettings algoliaSettings, 
            IWebHelper webHelper)
        {
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._algoliaSettings = algoliaSettings;
            this._webHelper = webHelper;
        }

        #endregion

        #region Install / Uninstall


        public override void Install()
        {
            _algoliaSettings.AdminKey = "Enter admin key";
            _algoliaSettings.ApplicationId = "Enter application id";
            _algoliaSettings.MonitoringKey = "Enter monitoring key";
            _algoliaSettings.SeachOnlyKey = "Enter seach only key";
            _algoliaSettings.MinimumQueryLength = 2;
            _algoliaSettings.SearchBoxThumbnailSize = 70;
            _algoliaSettings.PageSize = 20;
            _algoliaSettings.HideSoldOutProducts = false;
            _algoliaSettings.UploadSoldOutProducts = true;
            _algoliaSettings.UploadUnPublishedProducts = false;
            _algoliaSettings.AllowEmiFilter = true;
            _algoliaSettings.AllowPriceRangeFilter = true;
            _algoliaSettings.AllowRatingFilter = true;
            _algoliaSettings.MaximumCategoriesShowInFilter = 10;
            _algoliaSettings.MaximumManufacturersShowInFilter = 10;
            _algoliaSettings.MaximumVendorsShowInFilter = 10;
            _settingService.SaveSetting(_algoliaSettings);

            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Create", "Algolia Search Create");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.UploadProduct.Title", "Upload products to Algolia");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Configure.Title", "Algolia Configuration");

            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ApplicationId", "Application Id");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ApplicationId.Hint", "Application Id");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SeachOnlyKey", "Seach Only Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SeachOnlyKey.Hint", "Seach Only Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AdminKey", "Admin Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AdminKey.Hint", "Admin Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MonitoringKey", "Monitoring Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MonitoringKey.Hint", "Monitoring Key");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.PageSize", "Page Size");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.PageSize.Hint", "Page Size");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SearchBoxThumbnailSize", "Search Box Thumbnail Size");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SearchBoxThumbnailSize.Hint", "Search box thumbnail size");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MinimumQueryLength", "Minimum Query Length");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MinimumQueryLength.Hint", "Minimum Query Length");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ResetSettings", "Reset Index Settings");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ResetSettings.Hint", "This will reset index Faceting and Ranking ");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadSoldOutProducts", "Upload SoldOut Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.HideSoldOutProducts", "Hide SoldOut Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadUnPublishedProducts", "Upload UnPublished Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumCategoriesShowInFilter", "Maximum Categories Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumVendorsShowInFilter", "Maximum Vendors Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumManufacturersShowInFilter", "Maximum Manufacturers Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowPriceRangeFilter", "Allow Price Range Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowEmiFilter", "Allow Emi Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowRatingFilter", "Allow Rating Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowVendorFilter", "Allow Vendor Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowOthobaCertifiedFilter", "Allow Othoba Certified Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadSoldOutProducts.Hint", "Upload SoldOut Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.HideSoldOutProducts.Hint", "Hide SoldOut Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadUnPublishedProducts.Hint", "Upload UnPublished Products");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumCategoriesShowInFilter.Hint", "Maximum Categories Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumVendorsShowInFilter.Hint", "Maximum Vendors Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumManufacturersShowInFilter.Hint", "Maximum Manufacturers Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowPriceRangeFilter.Hint", "Allow Price Range Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowEmiFilter.Hint", "Allow Emi Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowRatingFilter.Hint", "Allow Rating Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowVendorFilter.Hint", "Allow Vendor Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowOthobaCertifiedFilter.Hint", "Allow Othoba Certified Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowManufacturerFilter", "Allow Manufacturer Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowSpecificationFilter", "Allow Specification Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowCategoryFilter", "Allow Category Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SelectablePageSizes", "Selectable Page Sizes");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumSpecificationsShowInFilter", "Maximum Specifications Show In Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowManufacturerFilter.Hint", "Allow Manufacturer Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowSpecificationFilter.Hint", "Allow Specification Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowCategoryFilter.Hint", "Allow Category Filter");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SelectablePageSizes.Hint", "Selectable Page Sizes");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumSpecificationsShowInFilter.Hint", "Maximum Specifications Show In Filter");


            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.FromId", "From Id");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.FromId.Hint", "Specify from which product to upload");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.ToId", "To Id");
            this.AddOrUpdatePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.ToId.Hint", "Specify to which product to upload");

            this.AddOrUpdatePluginLocaleResource("FILTERING.CATEGORYFILTER", "Categories");
            this.AddOrUpdatePluginLocaleResource("filtering.search.placeholder.category", "Enter Category Name");
            this.AddOrUpdatePluginLocaleResource("FILTERING.MANUFACTURERFILTER", "Manufacturers");
            this.AddOrUpdatePluginLocaleResource("filtering.search.placeholder.manufacturer", "Enter Manufacturer Name");
            this.AddOrUpdatePluginLocaleResource("FILTERING.VENDORFILTER", "Merchants");
            this.AddOrUpdatePluginLocaleResource("filtering.search.placeholder.vendor", "Enter Merchant Name");
            this.AddOrUpdatePluginLocaleResource("FILTERING.EMIFILTER", "EMI");
            this.AddOrUpdatePluginLocaleResource("Filtering.PriceRangeFilter", "Price Range");
            this.AddOrUpdatePluginLocaleResource("message.datanotfound", "No products found.");
            
            base.Install();
        }

        /// <summar
        /// y>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            //_settingService.DeleteSetting<AlgoliaSettings>();

            //data
            //_context.Uninstall();
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Create");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.UploadProduct.Title");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Configure.Title");

            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ApplicationId");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ApplicationId.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SeachOnlyKey");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SeachOnlyKey.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AdminKey");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AdminKey.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MonitoringKey");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MonitoringKey.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.PageSize");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.PageSize.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SearchBoxThumbnailSize");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SearchBoxThumbnailSize.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MinimumQueryLength");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MinimumQueryLength.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ResetSettings");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.ResetSettings.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadSoldOutProducts");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.HideSoldOutProducts");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadUnPublishedProducts");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumCategoriesShowInFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumVendorsShowInFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumManufacturersShowInFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowPriceRangeFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowEmiFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowRatingFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowVendorFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowOthobaCertifiedFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadSoldOutProducts.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.HideSoldOutProducts.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.UploadUnPublishedProducts.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumCategoriesShowInFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumVendorsShowInFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumManufacturersShowInFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowPriceRangeFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowEmiFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowRatingFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowVendorFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowOthobaCertifiedFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowManufacturerFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowSpecificationFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowCategoryFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SelectablePageSizes");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumSpecificationsShowInFilter");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowManufacturerFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowSpecificationFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.AllowCategoryFilter.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.SelectablePageSizes.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.Setting.MaximumSpecificationsShowInFilter.Hint");

            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.FromId");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.FromId.Hint");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.ToId");
            this.DeletePluginLocaleResource("Plugin.AlgoliaSearch.UploadModel.ToId.Hint");

            this.DeletePluginLocaleResource("FILTERING.CATEGORYFILTER");
            this.DeletePluginLocaleResource("filtering.search.placeholder.category");
            this.DeletePluginLocaleResource("FILTERING.MANUFACTURERFILTER");
            this.DeletePluginLocaleResource("filtering.search.placeholder.manufacturer");
            this.DeletePluginLocaleResource("FILTERING.VENDORFILTER");
            this.DeletePluginLocaleResource("filtering.search.placeholder.vendor");
            this.DeletePluginLocaleResource("FILTERING.EMIFILTER");
            this.DeletePluginLocaleResource("Filtering.PriceRangeFilter");
            this.DeletePluginLocaleResource("No products found.");
            
            base.Uninstall();
        }
        #endregion
        
        public IList<string> GetWidgetZones()
        {
            return new List<string> { "algolia_search_box" };
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
            controllerName = "AlgoliaAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.AlgoliaSearch.Controllers" }, { "area", null } };
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
            controllerName = "Algolia";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.AlgoliaSearch.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public bool Authenticate()
        {
            return true;
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menu = new SiteMapNode()
            {
                Visible = true,
                Title = "Algolia Search",
                IconClass = "fa-search"
            };

            var settings = new SiteMapNode()
            {
                Visible = true,
                Url = "/Admin/Widget/ConfigureWidget?systemName=Widgets.AlgoliaSearch",
                Title = "Settings",
                IconClass = "fa-genderless",
                SystemName = "AlgoliaAdmin.Configure"
            };

            var upload = new SiteMapNode()
            {
                Visible = true,
                Url = "/AlgoliaAdmin/UploadProducts",
                Title = "Upload Products",
                IconClass = "fa-genderless",
                SystemName = "AlgoliaAdmin.UploadProducts"
            };

            var synonym = new SiteMapNode()
            {
                Visible = true,
                Url = "/AlgoliaAdmin/Synonym",
                Title = "Synonyms",
                IconClass = "fa-genderless",
                SystemName = "AlgoliaAdmin.Synonyms"
            };

            var searchableAttribute = new SiteMapNode()
            {
                Visible = true,
                Url = "/AlgoliaAdmin/SearchableAttribute",
                Title = "Searchable Attributes",
                IconClass = "fa-genderless",
                SystemName = "AlgoliaAdmin.SearchableAttribute"
            };

            var ui = new SiteMapNode()
            {
                Visible = true,
                Url = "/AlgoliaAdmin/UpdatableItems",
                Title = "Updatable Items",
                IconClass = "fa-genderless",
                SystemName = "AlgoliaAdmin.UpdatableItems"
            };

            menu.ChildNodes.Add(settings);
            menu.ChildNodes.Add(upload);
            menu.ChildNodes.Add(synonym);
            menu.ChildNodes.Add(searchableAttribute);
            menu.ChildNodes.Add(ui);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Nop Station");
            if (pluginNode != null)
                pluginNode.ChildNodes.Add(menu);
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
                nopStation.ChildNodes.Add(menu);
            }
        }
    }
}
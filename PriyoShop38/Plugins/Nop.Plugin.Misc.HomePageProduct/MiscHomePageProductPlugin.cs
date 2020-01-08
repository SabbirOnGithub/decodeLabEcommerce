using Nop.Core.Plugins;
using Nop.Plugin.Misc.HomePageProduct.Data;
using Nop.Plugin.Misc.HomePageProduct.Services;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Web.Routing;
using System.Linq;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.HomePageProduct
{
    public class MiscHomePageProductPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly HomePageProductObjectContext _context;
        private readonly IHomePageProductService _biponeePreOrderService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;

        #endregion

        #region Ctr

        public MiscHomePageProductPlugin(HomePageProductObjectContext context,
            IHomePageProductService preOrderService,
            ILocalizationService localizationService,
            IPermissionService permissionService)
        {
            _context = context;
            _biponeePreOrderService = preOrderService;
            _localizationService = localizationService;
            this._permissionService = permissionService;
        }

        #endregion

        #region Install / Uninstall

        public override void Install()
        {
            //resource
            //this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.LivePersonChat.ButtonCode", "Button code(max 2000)");

            this.AddOrUpdatePluginLocaleResource("misc.homepageproduct.menu.text", "Manage HomePage Product");
            this.AddOrUpdatePluginLocaleResource("admin.common.addcategory", "Add Category");
            this.AddOrUpdatePluginLocaleResource("category.priority", "Category Priority");
            this.AddOrUpdatePluginLocaleResource("admin.addproduct", "Add Product");
            this.AddOrUpdatePluginLocaleResource("admin.addcategoryimage", "Add Categoryimage");
            this.AddOrUpdatePluginLocaleResource("category.addsubcategory", "Add Subcategory");
            this.AddOrUpdatePluginLocaleResource("category.deletecategoryfromhomepage", "Delete Category From Homepage");
            this.AddOrUpdatePluginLocaleResource("admin.catalog.bulkedit.fields.productname", "Product Name");
            this.AddOrUpdatePluginLocaleResource("admin.catalog.products.variants.fields.disablebuybutton", "Disable Buybutton");
            this.AddOrUpdatePluginLocaleResource("admin.catalog.bulkedit.fields.subcategoryname", "Subcategory Name");

            //install db
            _context.InstallSchema();

            //base install
            base.Install();
        }
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            //_settingService.DeleteSetting<FroogleSettings>();

            //data
            _context.Uninstall();

            this.DeletePluginLocaleResource("misc.homepageproduct.menu.text");
            this.DeletePluginLocaleResource("admin.common.addcategory");
            this.DeletePluginLocaleResource("category.priority");
            this.DeletePluginLocaleResource("admin.addproduct");
            this.DeletePluginLocaleResource("admin.addcategoryimage");
            this.DeletePluginLocaleResource("category.addsubcategory");
            this.DeletePluginLocaleResource("category.deletecategoryfromhomepage");
            this.DeletePluginLocaleResource("admin.catalog.bulkedit.fields.productname");
            this.DeletePluginLocaleResource("admin.catalog.products.variants.fields.disablebuybutton");
            this.DeletePluginLocaleResource("admin.catalog.bulkedit.fields.subcategoryname");

            base.Uninstall();
        }

        #endregion

        #region Menu Builder

        public bool Authenticate()
        {
            return true;
        }
        #endregion


        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.NopStationHompageCategoryPluginManage))
            {
                var menuItem = new SiteMapNode()
                {
                    SystemName = "Misc.HomePageProduct.CategoryList",
                    Title = _localizationService.GetResource("Misc.HomePageProduct.Menu.Text"),
                    Visible = true,
                    Url = "~/Plugin/HomePageProduct/CategoryList",
                    RouteValues = new RouteValueDictionary() {{"area", "Admin"}},
                    IconClass = "fa-product-hunt"
                };

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
            }
        }
    }
}
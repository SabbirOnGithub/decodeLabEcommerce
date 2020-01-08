using Nop.Core.Plugins;
using Nop.Plugin.Purchase.Offer.Data;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop.Plugin.Purchase.Offer
{
    public class PurchaseOfferPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private PurchaseOfferObjectContext _context;
        public PurchaseOfferPlugin(PurchaseOfferObjectContext context)
        {
            _context = context;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PurchaseOffer";
            routeValues = new RouteValueDictionary { { "Area", null }, { "Namespaces", "Nop.Plugin.Purchase.Offer.Controllers" } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "PurchaseOffer";
            routeValues = new RouteValueDictionary { { "Area", null }, { "Namespaces", "Nop.Plugin.Purchase.Offer.Controllers" } };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "purchase_offer_order_summary", "purchase_offer_list" };
        }

        public override void Install()
        {
            _context.Install();
            base.Install();

            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.ValidFrom", "Valid From");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.ValidTo", "Valid To");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.IsActive", "Is Active");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Description", "Description");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Options", "Options");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Info", "Info");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchProductName", "Product Name");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchCategoryId", "Category");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchManufacturerId", "Manufacturer");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchStoreId", "Store");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchVendorId", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.SearchProductTypeId", "Product Type");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Quantity", "Quantity");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.MinimumPurchaseAmount", "Min. Purchase");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.Note", "Note");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.ProductName", "Product Name");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.ProductImage", "Product Image");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.AddNew", "Add New");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.Offer.ShowNotificationOnCart", "Notify Offer On Cart");
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var parent = new SiteMapNode()
            {
                Title = "Purchase Offer",
                Url = "/PurchaseOffer/Settings",
                Visible = true,
                IconClass = "fa fa-dot-circle-o"
            };
            foreach (var item in rootNode.ChildNodes)
            {
                if (item.SystemName == "Promotions")
                {
                    item.ChildNodes.Add(parent);
                    break;
                }
            }
        }

        public override void Uninstall()
        {
            _context.UnInstall();
            base.Uninstall();

            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Name");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.ValidFrom");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.ValidTo");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.IsActive");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Description");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Options");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Info");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchProductName");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchCategoryId");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchManufacturerId");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchStoreId");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchVendorId");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.SearchProductTypeId");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Quantity");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.MinimumPurchaseAmount");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.Note");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.ProductName");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.ProductImage");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.AddNew");
            this.DeletePluginLocaleResource("Admin.Purchase.Offer.ShowNotificationOnCart");
        }
    }
}

using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.CustomFooter
{
    public class PurchaseOfferPlugin : BasePlugin, IAdminMenuPlugin
    {
        public PurchaseOfferPlugin()
        {
        }

        public override void Install()
        {
            base.Install();

            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.UserAgent", "User Agent");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.Title", "Purchase User Agent");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.OrderId", "Order Id");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.CreatedOnUtc", "Created On");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.CustomerName", "Customer Name");
            this.AddOrUpdatePluginLocaleResource("Admin.Purchase.UserAgent.CustomerId", "Customer Id");
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var parent = new SiteMapNode()
            {
                Url = "/PurchaseUserAgent/List",
                Title = "Purchase User Agent",
                IconClass = "fa fa-dot-circle-o",
                Visible = true
            };
            foreach (var item in rootNode.ChildNodes)
            {
                if (item.SystemName == "Sales")
                {
                    item.ChildNodes.Add(parent);
                    break;
                }
            }
        }

        public override void Uninstall()
        {
            base.Uninstall();

            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.UserAgent");
            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.Title");
            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.OrderId");
            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.CreatedOnUtc");
            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.CustomerName");
            this.DeletePluginLocaleResource("Admin.Purchase.UserAgent.CustomerId");
        }
    }
}

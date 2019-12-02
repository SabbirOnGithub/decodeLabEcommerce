using Nop.Core.Plugins;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Web.Framework.Menu;
using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.EkShopA2I
{
    public class EkshopPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private EkshopObjectContext _context;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;

        public EkshopPlugin(EkshopObjectContext context, 
            ISettingService settingService,
            IPermissionService permissionService)
        {
            _context = context;
            _settingService = settingService;
            _permissionService = permissionService;
        }

        public override void Install()
        {
            _permissionService.InstallPermissions(new EkshopPermissionProvider());

            var settings = new EkshopSettings()
            {
                AccessToken = "Ek-shop provided access token here",
                ApiKey = "Secret API key here",
                Authorization = "Secret Authentication key here",
                UdcCommission = 3
            };
            _settingService.SaveSetting(settings);

            _context.Install();

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommission", "Udc Commission (%)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommission.Hint", "Udc Commission will be provided to Udc from Priyoshop.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.AccessToken", "Access Token");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.AccessToken.Hint", "Access Token will be provided by Ek-Shop");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.EkShopBaseUrl", "Ek-Shop Base Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.EkShopBaseUrl.Hint", "Ek-Shop Base Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.ApiKey", "Api Key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.ApiKey.Hint", "Api Key is the secret key to access as a a2i user.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Authentication", "Authentication");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Authentication.Hint", "Authentication is the secret key to access as a a2i user.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.Configure", "Configure");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductBox", "Show Udc Commission On Product Box");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductBox.Hint", "Show Udc Commission On Product Box");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductDetails", "Show Udc Commission On Product Details");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductDetails.Hint", "Show Udc Commission On Product Details");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.Orders", "Orders");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.UdcCommission", "Udc Commission");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderCode", "Order Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpCode", "Logistic Partner Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpName", "Logistic Partner Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactPerson", "Logistic Partner Contact Person");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactNumber", "Logistic Partner Contact Number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpLocation", "Logistic Partner Location");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryCharge", "Delivery Charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.PaymentMethod", "Payment Method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.Total", "Total (tk)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryDuration", "Delivery Duration");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OtherRequiredData", "Other Required Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.CreatedOn", "Created On");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderId", "Order Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderCode.Hint", "Order Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.UdcCommission.Hint", "Udc Commission will be provided to UDC from Priyoshop.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpCode.Hint", "Logistic Partner Code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpName.Hint", "Logistic Partner Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactPerson.Hint", "Logistic Partner Contact Person");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactNumber.Hint", "Logistic Partner Contact Number");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpLocation.Hint", "Logistic Partner Location");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryCharge.Hint", "Delivery Charge");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.PaymentMethod.Hint", "Payment Method");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.Total.Hint", "Total");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryDuration.Hint", "Delivery Duration");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OtherRequiredData.Hint", "Other Required Data");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.CreatedOn.Hint", "Created On");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderId.Hint", "Order Id");

            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityName", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityId", "Entity Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate", "Commission Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.VendorId", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CategoryId", "Category");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityName.Hint", "Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityId.Hint", "Entity Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type.Hint", "Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate.Hint", "Commission Rate");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.VendorId.Hint", "Vendor");
            this.AddOrUpdatePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CategoryId.Hint", "Category");

            base.Install();
        }

        public override void Uninstall()
        {
            _permissionService.UninstallPermissions(new EkshopPermissionProvider());

            _context.UnInstall();

            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityName");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityId");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.VendorId");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CategoryId");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.EntityId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.Type.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CommissionRate.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.VendorId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommissionRate.CategoryId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommission");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.UdcCommission.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.UdcCommission");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.UdcCommission.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderCode");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpCode");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpName");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactPerson");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactNumber");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpLocation");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryCharge");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.PaymentMethod");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.Total");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryDuration");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OtherRequiredData");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.CreatedOn");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderId");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpName.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactPerson.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpContactNumber.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.LpLocation.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryCharge.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.PaymentMethod.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.Total.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.DeliveryDuration.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OtherRequiredData.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.CreatedOn.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Domain.OrderId.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.AccessToken");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.EkShopBaseUrl");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.ApiKey");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Authentication");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.Configure");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.Orders");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductBox");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductBox.Hint");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductDetails");
            this.DeletePluginLocaleResource("Plugins.Widgets.EkShopA2I.Configure.ShowUdcCommissionOnProductDetails.Hint");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menu = new SiteMapNode()
            {
                Visible =true,
                IconClass = "fa-cube",
                Title = "Ek-Shop a2i"
            };

            if (_permissionService.Authorize(EkshopPermissionProvider.EkShopConfigure))
            {
                var settings = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/EkShopA2i/Configure",
                    Title = "Configuration",
                    SystemName = "EkShopA2i Settings"
                };
                menu.ChildNodes.Add(settings);
            }

            if (_permissionService.Authorize(EkshopPermissionProvider.EkShopViewOrders) ||
                _permissionService.Authorize(EkshopPermissionProvider.EkShopManageVendor))
            {
                var order = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/EkShopA2i/OrderList",
                    Title = "Orders",
                    SystemName = "EkShopA2i Orders"
                };
                menu.ChildNodes.Add(order);
            }

            if (_permissionService.Authorize(EkshopPermissionProvider.EkShopManageCommission) ||
                _permissionService.Authorize(EkshopPermissionProvider.EkShopViewCommission))
            {
                var rate = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/EkShopA2i/CommissionRate",
                    Title = "Commission Rate",
                    SystemName = "EkShopA2i Commission Rate"
                };
                menu.ChildNodes.Add(rate);
            }

            if (_permissionService.Authorize(EkshopPermissionProvider.EkShopManageVendor))
            {
                var vendor = new SiteMapNode()
                {
                    Visible = true,
                    IconClass = "fa-genderless",
                    Url = "/EkShopA2i/RestrictVendor",
                    Title = "Manage Vendor",
                    SystemName = "EkShopA2i Manage Vendor"
                };
                menu.ChildNodes.Add(vendor);
            }

            if (menu.ChildNodes != null && menu.ChildNodes.Count > 0)
            {
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

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Settings";
            controllerName = "EkShopA2i";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.EkShopA2I.Controllers" }, { "area", null } };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>() { "footer", "productbox_ekshop_commission", "productdetails_before_pictures" };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Public";
            controllerName = "EkShopAuth";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.EkShopA2I.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
    }
}

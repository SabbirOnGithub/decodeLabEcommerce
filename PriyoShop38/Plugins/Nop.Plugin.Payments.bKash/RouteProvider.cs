using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.bKash
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.Bkash.Configure",
                 "Plugins/PaymentBkash/Configure",
                 new { controller = "PaymentBkash", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.Bkash.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Bkash.PaymentInfo",
                 "Plugins/PaymentBkash/PaymentInfo",
                 new { controller = "PaymentBkash", action = "PaymentInfo" },
                 new[] { "Nop.Plugin.Payments.Bkash.Controllers" }
            );

            routes.MapRoute("Admin.Plugins.Payments.Bkash.Settings", "Plugins/PaymentBkash/Settings",
            new
            {
                controller = "PaymentBkash",
                action="Settings"
            },
            new[] { "Nop.Plugin.Payments.Bkash.Controllers" }).DataTokens.Add("area","admin");

            //bkash page
            routes.MapRoute("Admin.Plugins.Payments.Bkash.BkashPaymentsPage", "bKash-payment",
            new
            {
                controller = "PaymentBkash",
                action = "BkashPaymentsPage"
            },
            new[] { "Nop.Plugin.Payments.Bkash.Controllers" }).DataTokens.Add("area", "");

        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}

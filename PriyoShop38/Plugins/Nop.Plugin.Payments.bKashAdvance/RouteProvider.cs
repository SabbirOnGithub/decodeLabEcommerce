using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using System.Web.Routing;

namespace Nop.Plugin.Payments.bKashAdvance
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 1000000;
            }
        }

        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("BkashPay",
                            "bkash/pay/{orderId}",
                            new { controller = "Bkash", action = "Pay" },
                            new[] { "Nop.Plugin.Payments.bKashAdvance.Controllers" });

            routes.MapLocalizedRoute("BkashApiPay",
                "api/bkash/pay/{orderId}",
                new { controller = "Bkash", action = "Pay" },
                new[] { "Nop.Plugin.Payments.bKashAdvance.Controllers" });
        }
    }
}

using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapLocalizedRoute("CustomerAffiliateOrders",
            //                "affiliate/orders",
            //                new { controller = "BsAffiliate", action = "Orders" },
            //                new[] { "Nop.Plugin.Widgets.BsAffiliate.Controllers" });

            //routes.MapLocalizedRoute("CustomerAffiliateInfo",
            //                "affiliate/info",
            //                new { controller = "BsAffiliate", action = "Info" },
            //                new[] { "Nop.Plugin.Widgets.BsAffiliate.Controllers" });
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

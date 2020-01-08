using Nop.Web.Framework.Mvc.Routes;
using System.Web.Routing;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.EasyPayWay
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //Success and failure URL
            routes.MapRoute("Plugin.Payments.SSLCommerzEmi.PaymentResult",
                 "Plugins/PaymentSSLCommerzEmi/PaymentResult",
                 new { controller = "PaymentSSLCommerzEmi", action = "PaymentResult" },
                 new[] { "Nop.Plugin.Payments.SSLCommerzEmi.Controllers" }
            );
            //cancel URL
            routes.MapRoute("Plugin.Payments.SSLCommerzEmi.CancelOrder",
                 "Plugins/PaymentSSLCommerzEmi/CancelOrder",
                 new { controller = "PaymentSSLCommerzEmi", action = "CancelOrder" },
                 new[] { "Nop.Plugin.Payments.SSLCommerzEmi.Controllers" }
            );
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

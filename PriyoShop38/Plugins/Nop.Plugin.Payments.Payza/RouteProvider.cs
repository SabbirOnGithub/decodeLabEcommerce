using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Payza
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //IPN
            routes.MapRoute("Plugin.Payments.Payza.IPNHandler",
                 "Plugins/PaymentPayza/IPNHandler",
                 new { controller = "PaymentPayza", action = "IPNHandler" },
                 new[] { "Nop.Plugin.Payments.Payza.Controllers" }
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

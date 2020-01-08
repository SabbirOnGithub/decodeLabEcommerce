using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.Dmoney
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Payments.Dmoney.Configure",
                 "Plugins/PaymentDmoney/Configure",
                 new { controller = "PaymentDmoney", action = "Configure" },
                 new[] { "Nop.Plugin.Payments.Dmoney.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Dmoney.ApprovePayment",
                 "dmoney/approve/{transactionTrackingNo}",
                 new { controller = "PaymentDmoney", action = "Approve" },
                 new[] { "Nop.Plugin.Payments.Dmoney.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Dmoney.CancelPayment",
                 "dmoney/cancel/{transactionTrackingNo}",
                 new { controller = "PaymentDmoney", action = "Cancel" },
                 new[] { "Nop.Plugin.Payments.Dmoney.Controllers" }
            );

            routes.MapRoute("Plugin.Payments.Dmoney.DeclinePayment",
                 "dmoney/decline/{transactionTrackingNo}",
                 new { controller = "PaymentDmoney", action = "Decline" },
                 new[] { "Nop.Plugin.Payments.Dmoney.Controllers" }
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

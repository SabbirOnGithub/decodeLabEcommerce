using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Payments.EblSkyPay
{
	public class RouteProvider : IRouteProvider
	{
		public void RegisterRoutes(RouteCollection routes)
		{
			routes.MapRoute("Plugin.Payments.EblSkyPay.EblSkyPay", "Plugins/PaymentEblSkyPay/EblSkyPay",
			new
            {
				controller = "PaymentEblSkyPay",
				action = "EblSkyPay"
			},
            new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });

            routes.MapRoute("Plugin.Payments.EblSkyPay.SuccessOrder", "Plugins/PaymentEblSkyPay/SuccessOrder",
            new
            {
                controller = "PaymentEblSkyPay",
                action = "SuccessOrder"
            },
            new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });
		
			routes.MapRoute("Plugin.Payments.EblSkyPay.FailOrder", "Plugins/PaymentEblSkyPay/FailOrder",
			new
			{
				controller = "PaymentEblSkyPay",
				action = "FailOrder"
			},
			new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });
		
			routes.MapRoute("Plugin.Payments.EblSkyPay.CancelOrder", "Plugins/PaymentEblSkyPay/CancelOrder",
			new
			{
				controller = "PaymentEblSkyPay",
				action = "CancelOrder"
			},
			new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });

			routes.MapRoute("Plugin.Payments.EblSkyPay.PaymentInfo", "Plugins/PaymentEblSkyPay/PaymentInfo",
			new
			{
				controller = "PaymentEblSkyPay",
				action = "PaymentInfo"
			},
			new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });

            routes.MapRoute("Plugin.Payments.EblSkyPay.ErrorCallback", "Plugins/PaymentEblSkyPay/ErrorCallback",
            new
            {
                controller = "PaymentEblSkyPay",
                action = "ErrorCallback"
            },
            new[] { "Nop.Plugin.Payments.EblSkyPay.Controllers" });
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
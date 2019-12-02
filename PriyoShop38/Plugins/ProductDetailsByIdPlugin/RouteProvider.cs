using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using Nop.Plugin.Misc.ProductDetailsById;

namespace Nop.Plugin.Misc.ProductDetailsById
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Nop.Plugin.Misc.ProductDetailsById.Details", "ProductFind/{productId}",
             new
             {
                 controller = "Product",
                 action = "ProductDetails"
             },
             new[] { "Nop.Web.Controllers" }
             );

            routes.MapRoute("Nop.Plugin.Misc.ProductsByVendorId", "ProductsByVendorId/{vendorId}",
             new
             {
                 controller = "VendorCustom",
                 action = "GetProductsByVendorId"
             },
             new[] { "Nop.Plugin.Misc.ProductDetailsById.Controllers" });

        }
        public int Priority
        {
            get
            {
                return 100;
            }
        }

    }
}
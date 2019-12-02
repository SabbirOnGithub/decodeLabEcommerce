using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Misc.OnePageCheckOut.Infrastructure;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.OnePageCheckOut
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {


           // ViewEngines.Engines.Insert(0, new OnePageCheckOutViewEngine());
            //one page check out
            routes.MapRoute("Plugin.Misc.OnePageCheckOut.OnePage",
                 "onepagecheckout",
                 new { controller = "MiscOnePageCheckOut", action = "OnePageCheckout" },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOut.Controllers" }
            );
 

            


            routes.MapRoute(
                "Plugin.Misc.OnePageCheckOut.Default",
                "MiscOnePageCheckOut/{action}/{id}",
                new {controller = "MiscOnePageCheckOut", action = "OnePageCheckout", area = "", id = ""},
                new[] {"Nop.Plugin.Misc.OnePageCheckOut.Controllers"}
                );

        
            routes.MapRoute("Plugin.Misc.OnePageCheckOut.GetDelivaryChargeByStateProvinceId", "GetDelivaryChargeByStateProvinceId/{stProvinceId}",
                 new
                 {
                     controller = "MiscOnePageCheckOut",
                     action = "GetDelivaryChargeByStateProvinceId"
                 },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOut.Controllers" }).DataTokens.Add("area", "");
            

        }
        public int Priority
        {
            get
            {
                return 2;
            }
        }
    }
}

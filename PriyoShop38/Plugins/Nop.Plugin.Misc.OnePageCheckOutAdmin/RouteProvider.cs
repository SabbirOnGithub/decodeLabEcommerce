using System.Web.Mvc;
using System.Web.Routing;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Infrastructure;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapRoute("Plugin.Misc.OnePageCheckOutAdmin.Configure",
                 "Plugins/MiscOnePageCheckOutAdmin/Configure",
                 new { controller = "OnePageCheckOutAdmin", action = "Configure" },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" }
            );

            ViewEngines.Engines.Insert(0, new OnePageCheckOutViewEngine());
            //one page check out
            routes.MapRoute("Plugin.Misc.OnePageCheckOutAdmin.OnePageCheckOutAdmin",
                 "Plugins/MiscOnePageCheckOutAdmin/OnePageCheckOutAdmin",
                 new { controller = "MiscOnePageCheckOutAdmin", action = "OnePageCheckOutAdmin" },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" }
            );

            routes.MapRoute("Plugin.Misc.OnePageCheckOutAdmin.CustomerSearchAutoComplete",
                            "Plugins/MiscOnePageCheckOutAdmin/customersearchautocomplete",
                            new { controller = "MiscOnePageCheckOutAdmin", action = "CustomerSearchAutoComplete" },
                            new[] { "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" });

            routes.MapRoute("Plugin.Misc.OnePageCheckOutAdmin.OnePageCheckOutAdmin.Set",
                 "onepagecheckoutadmin/setavailabledatetime",
                 new { controller = "DelivaryDateTime", action = "SetAvailableDatetime" },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" }
            );
            //routes.MapRoute(
            //    "Plugin.Misc.OnePageCheckOut.Default",
            //    "MiscOnePageCheckOutAdmin/{action}/{id}",
            //    new {controller = "MiscOnePageCheckOut", action = "OnePageCheckout", area = "", id = ""},
            //    new[] {"Nop.Plugin.Misc.OnePageCheckOut.Controllers"}
            //    );


            routes.MapRoute("Plugin.Misc.OnePageCheckOutAdmin.GetDelivaryChargeByStateProvinceId", "GetDelivaryChargeByStateProvinceId",
                 new
                 {
                     controller = "MiscOnePageCheckOutAdmin",
                     action = "GetDelivaryChargeByStateProvinceId"
                 },
                 new[] { "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" });
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

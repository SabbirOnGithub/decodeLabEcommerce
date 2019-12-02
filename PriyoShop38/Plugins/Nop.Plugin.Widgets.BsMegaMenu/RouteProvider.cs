using Nop.Plugin.Widgets.BsMegaMenu.ViewEngines;
using Nop.Web.Framework.Mvc.Routes;
using System.Web.Mvc;
using System.Web.Routing;


namespace Nop.Plugin.Widgets.BsMegaMenu
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //System.Web.Mvc.ViewEngines.Engines.Insert(0, new CustomViewEngine());

            //routes.MapRoute("GenerateMegaMenu", "BsMegaMenu",
            //                new { controller = "BsMegaMenu", action = "Generate" },
            //                new[] { "Nop.Plugin.Widgets.BsMegaMenu.Controllers" });

            //routes.MapRoute("Admin.Plugin.Widgets.BsMegaMenu.Settings", "Admin/Plugin/BsMegaMenu/Settings",
            //       new { controller = "BsMegaMenuSetting", action = "BsMegaMenuSettings" },
            //       new[] { "Nop.Plugin.Widgets.BsMegaMenu.Controllers" }).DataTokens.Add("area", "admin");


            //routes.MapRoute("Admin.Plugin.Widgets.BsMegaMenu.SaveSettings", "Admin/Plugin/BsMegaMenu/Settings/Save",
            //       new { controller = "BsMegaMenuSetting", action = "BsMegaMenuSettings" },
            //       new[] { "Nop.Plugin.Widgets.BsMegaMenu.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Plugin.Misc.BsMegaMenuCache", "bsmegamenucache",
          new
          {
              controller = "BsMegaMenu",
              action = "BsMegaMenuCache"
          },
          new[] { "Nop.Plugin.Widgets.BsMegaMenu.Controllers" });

        }

        public int Priority
        {
            get { return 0; }
        }

    }
}

          
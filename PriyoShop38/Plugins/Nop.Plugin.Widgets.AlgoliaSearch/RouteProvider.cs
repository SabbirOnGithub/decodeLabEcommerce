using Nop.Web.Framework.Mvc.Routes;
using Owin;
using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    public class RouteProvider : IRouteProvider
    {
        public int Priority
        {
            get
            {
                return 0;
            }
        }

        public IAppBuilder _appBuilderService { get; set; }

        public void RegisterRoutes(RouteCollection routes)
        {
            RouteTable.Routes.MapOwinPath("/signalr", _appBuilderService => _appBuilderService.RunSignalR());

            routes.MapRoute("AlgoliaSearch", "src",
                 new { controller = "Algolia", action = "AlgoliaSearch" });

            routes.MapRoute("AlgoliaSearchByCatagoty", "src/{seName}",
                new { controller = "Algolia", action = "AlgoliaSearchByCategory" });
        }
    }
}

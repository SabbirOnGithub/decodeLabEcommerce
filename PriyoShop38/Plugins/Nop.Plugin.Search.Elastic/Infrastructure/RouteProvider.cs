using Nop.Web.Framework.Mvc.Routes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;
using Nop.Web.Framework.Localization;

namespace Nop.Plugin.Search.Elastic.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        { //product search
            //routes.MapLocalizedRoute("ProductSearch",
            //                "search/",
            //                new { controller = "Catalog", action = "Search" },
            //                new[] { "Nop.Web.Controllers" });
            routes.MapLocalizedRoute("ElasticProductSearchAutoComplete",
                            "catalog/searchtermautocomplete",
                            new { controller = "Catalog", action = "SearchTermAutoCompleteFromElastic" },
                            new[] { "Nop.Plugin.Search.Elastic.Controllers" });

        }

        public int Priority
        {
            get
            {
                return 10;
            }
        }
    }
}

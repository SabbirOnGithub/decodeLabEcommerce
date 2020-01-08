using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;
using System.Web.Http;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            var config = GlobalConfiguration.Configuration;

            //config.EnableCors();
            config.MessageHandlers.Add(new CorsHandler());

            routes.MapHttpRoute(
                name: "DefaultWeb",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

           // routes.MapRoute("MobileWebApiConfigure",
           //     "Plugins/MobileWebApi/Generalsetting",
           //     new { controller = "MobileWebApiConfiguration", action = "Generalsetting" },
           //     new[] { "BS.Plugin.NopStation.MobileWebApi.Controllers" }
           //);
            routes.MapRoute("MobileWebApiConfigure.SliderImageAdd", "Plugins/MobileWebApi/SliderImageAdd",
            new
            {
                controller = "MobileWebApiConfiguration",
                action = "SliderImageAdd"
            },
            new[] { "BS.Plugin.NopStation.MobileWebApi.Controllers" });
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

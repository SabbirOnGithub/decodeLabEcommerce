using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.WebApi;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;


namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            routes.MapLocalizedRoute("Plugin.NopStation.MobileWebApi.",
                "KeepAlive/indexApi",
                new { controller = "KeepAliveApi", action = "Index", area = "" },
                new[] { "BS.Plugin.NopStation.MobileWebApi.Controllers" }
            );
            routes.MapRoute("Plugin.NopStation.MobileWebApi.OpcCompleteRedirectionPayment",
                "api/checkout/OpcCompleteRedirectionPayment",
                new { controller = "RedirectPage", action = "OpcCompleteRedirectionPayment", area = "" },
                new[] { "BS.Plugin.NopStation.MobileWebApi.Controllers" }
            );
            //web api 2
            var config = GlobalConfiguration.Configuration;
            WebApiConfig.Register(config);
           
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

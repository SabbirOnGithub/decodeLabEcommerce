using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.WebApi;
using Nop.Core.Infrastructure;
using Nop.Plugin.Misc.EkShopA2I.Infrastructure.WebApi.Logger;

namespace Nop.Plugin.Misc.EkShopA2I.Infrastructure.WebApi
{
    public static class A2IWebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "Plugin.Misc.EkShopA2I.CheckOut",
            //    routeTemplate: "api/escheckout/complete",
            //    defaults: new { Controller = "Customer", id = RouteParameter.Optional }
            //);

            //config.Routes.MapHttpRoute("Plugin.Misc.EkShopA2I.CheckOut",
            //        "api/escheckout/complete",
            //        new { controller = "EsCheckout", action = "Complete", area = "" },
            //        new[] { "Nop.Plugin.Misc.EkShopA2I.Controllers" }
            //   );

            //config.Routes.MapHttpRoute(
            //    name: "NopStation.MobileWebApi.Offer",
            //    routeTemplate: "MobileWebApi/Offer/{action}/{id}",
            //    defaults: new { Controller = "BsInstragramOfferApi", id = RouteParameter.Optional }
            //);



            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            
            // We have to register services specifically for the API calls!
            //builder.RegisterType<CategoryService>().AsImplementedInterfaces().InstancePerLifetimeScope();
            //Update existing, don't create a new container
            builder.Update(EngineContext.Current.ContainerManager.Container);

            //Feed the current container to the AutofacWebApiDependencyResolver
            var resolver = new AutofacWebApiDependencyResolver(EngineContext.Current.ContainerManager.Container);
            config.DependencyResolver = resolver;


            

            //exception logger 
            config.Services.Add(typeof(IExceptionLogger), new SimpleExceptionLogger());
            //exception handler
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());


            //we will get JSON by default, but it will still allow you to return XML if you pass text/xml as the request Accept header
            var appXmlType = config.Formatters.XmlFormatter.SupportedMediaTypes.FirstOrDefault(t => t.MediaType == "application/xml");
            config.Formatters.XmlFormatter.SupportedMediaTypes.Remove(appXmlType);

            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}


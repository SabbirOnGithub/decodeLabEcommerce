//using System.Web.Mvc;
//using System.Web.Routing;
//using Nop.Web.Framework.Localization;
//using Nop.Web.Framework.Mvc.Routes;

//namespace Nop.Plugin.Widgets.MobileLogin
//{
//    public partial class RouteProvider : IRouteProvider
//    {
//        public void RegisterRoutes(RouteCollection routes)
//        {
//            //Login page
//            routes.MapLocalizedRoute("Plugin.Widgets.MobileLogin.Login",
//                 "login",
//                 new { controller = "MobileLoginCustomer", action = "Login" },
//                 new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" }
//            );

//            ////login page for checkout as guest
//            //routes.MapRoute("Plugin.Widgets.MobileLogin.LoginCheckoutAsGuest",
//            //    "login/checkoutasguest",
//            //    new { controller = "MobileLoginCustomer", action = "Login", checkoutAsGuest = true },
//            //    new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" });

//            routes.MapRoute("Plugin.Widgets.MobileLogin.LoginStep2",
//                 "loginStep2",
//                 new { controller = "MobileLoginCustomer", action = "LoginStep2" },
//                 new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" }
//            );

//            routes.MapRoute("Plugin.Widgets.MobileLogin.MobileLoginData",
//                 "MobileLoginData",
//                 new { controller = "MobileLoginCustomer", action = "MobileLoginData" },
//                 new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" }
//            );

//            routes.MapRoute("Plugin.Widgets.MobileLogin.RequestOTP",
//                 "MobileLogin/RequestOTP",
//                 new { controller = "MobileLoginCustomer", action = "RequestOTP" },
//                 new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" }
//            );

//            routes.MapRoute(
//                "Plugin.Widgets.MobileLogin.Default",
//                "MobileLogin/{action}/{id}",
//                new { controller = "MobileLoginCustomer", action = "Login", area = "", id = "" },
//                new[] { "Nop.Plugin.Widgets.MobileLogin.Controllers" }
//                );                         

//        }
//        public int Priority
//        {
//            get
//            {
//                return 2;
//            }
//        }
//    }
//}

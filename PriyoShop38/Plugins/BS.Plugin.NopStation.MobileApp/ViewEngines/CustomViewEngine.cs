
using Nop.Web.Framework.Themes;
namespace BS.Plugin.NopStation.MobileApp.ViewEngines
{
    class CustomViewEngine : ThemeableRazorViewEngine 
    {
        
        public  CustomViewEngine()
        {



            ViewLocationFormats = new[]
                                             {
                                                 "~/Plugins/NopStation.MobileApp/Views/BsNotification/{0}.cshtml",
                                                 "~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/{0}.cshtml"
                                             };

            PartialViewLocationFormats = new[]
                                             {

                                                 "~/Plugins/NopStation.MobileApp/Views/BsNotification/{0}.cshtml",
                                                 "~/Plugins/NopStation.MobileApp/Views/BsNotificationMessageTemplate/{0}.cshtml"
                                             };

            
        }
    }
}

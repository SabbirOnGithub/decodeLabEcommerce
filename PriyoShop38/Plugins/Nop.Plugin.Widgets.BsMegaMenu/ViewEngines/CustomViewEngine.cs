
using Nop.Web.Framework.Themes;
namespace Nop.Plugin.Widgets.BsMegaMenu.ViewEngines
{
    class CustomViewEngine : ThemeableRazorViewEngine
    {

        public CustomViewEngine()
        {

            ViewLocationFormats = new[]
                                             {
                                                 "~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenu/{0}.cshtml", "~/Plugins/Widgets.BsMegaMenu/Views/{0}.cshtml"
                                             };


            PartialViewLocationFormats = new[]
                                             {

                                                 "~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenu/{0}.cshtml","~/Plugins/Widgets.BsMegaMenu/Views/{0}.cshtml"
                                             };


            //ViewLocationFormats = new[]
            //                                 {
            //                                     "~/Plugins/Widgets.BsMegaMenu/Views/{0}.cshtml"
            //                                 };

            //PartialViewLocationFormats = new[]
            //                                 {

            //                                     "~/Plugins/Widgets.BsMegaMenu/Views/{0}.cshtml"
            //                                 };

          

           
        }
    }
}

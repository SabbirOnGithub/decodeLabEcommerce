using Nop.Web.Framework.Themes;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Infrastructure
{
    public class OnePageCheckOutViewEngine : ThemeableRazorViewEngine
    {
        public OnePageCheckOutViewEngine()
        {
            ViewLocationFormats = new[] { "~/Plugins/Misc.OnePageCheckOutAdmin/Views/MiscOnePageCheckOutAdmin/{0}.cshtml" };
            PartialViewLocationFormats = new[] { "~/Plugins/Misc.OnePageCheckOutAdmin/Views/MiscOnePageCheckOutAdmin/{0}.cshtml" };
        }
    }
}
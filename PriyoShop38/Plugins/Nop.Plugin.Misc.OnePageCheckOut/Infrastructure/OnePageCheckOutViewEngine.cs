using Nop.Web.Framework.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Misc.OnePageCheckOut.Infrastructure
{
    public class OnePageCheckOutViewEngine : ThemeableRazorViewEngine
    {
        public OnePageCheckOutViewEngine()
        {
            ViewLocationFormats = new[] { "~/Plugins/Misc.OnePageCheckOut/Views/MiscOnePageCheckOut/{0}.cshtml" };
            PartialViewLocationFormats = new[] { "~/Plugins/Misc.OnePageCheckOut/Views/MiscOnePageCheckOut/{0}.cshtml" };
        }
    }
}
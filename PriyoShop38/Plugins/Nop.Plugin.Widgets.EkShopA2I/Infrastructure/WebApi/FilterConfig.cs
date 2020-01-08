using System.Web.Mvc;

namespace Nop.Plugin.Misc.EkShopA2I.Infrastructure.WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

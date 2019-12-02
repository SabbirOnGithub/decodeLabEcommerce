using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Misc.HomePageProduct
{
    public class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            #region Manage

            routes.MapRoute("Plugin.Misc.HomePageProduct.CategoryList", "Plugin/HomePageProduct/CategoryList",
            new
            {
                controller = "HomePageProduct",
                action = "CategoryList"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Plugin.Misc.HomePageProduct.List", "Plugin/HomePageProduct/List/{CategoryId}",
            new
            {
                controller = "HomePageProduct",
                action = "List"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Plugin.Misc.HomePageProduct.CategoryImage", "Plugin/HomePageProduct/CategoryImage/{CategoryId}",
            new
            {
                controller = "HomePageProduct",
                action = "CategoryImage"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Plugin.Misc.HomePageProduct.SubCategoryList", "Plugin/HomePageProduct/SubCategoryList/{CategoryId}",
            new
            {
                controller = "HomePageProduct",
                action = "SubCategoryList"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");


            routes.MapRoute("Plugin.Misc.HomePageProduct.CategoryImageAdd", "Plugin/HomePageProduct/CategoryImageAdd",
            new
            {
                controller = "HomePageProduct",
                action = "CategoryImageAdd"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Plugin.Misc.HomePageProduct.UpdateCategoryColor", "Plugin/HomePageProduct/UpdateCategoryColor",
            new
            {
                controller = "HomePageProduct",
                action = "UpdateCategoryColor"
            },
            new[] { "Nop.Plugin.Misc.HomePageProduct.Controllers" }).DataTokens.Add("area", "admin");

            #endregion

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
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Web.Framework.Menu;
using System.Linq;
using Nop.Services.Security;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin
{
    public class CheckOutPlugIn : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        #endregion
        #region Ctor

        public CheckOutPlugIn(IPermissionService permissionService)
        {
            this._permissionService = permissionService;
        }
        #endregion
        #region Methods

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "MiscOnePageCheckOutAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers" }, { "area", null } };
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.NopStationOnePageCheckoutPluginManage))
            {
                var menuItemBuilder = new SiteMapNode()
                {
                    Visible = true,
                    Title = "One Page CheckOut",

                };
           
                var menuItem = new SiteMapNode()
                {
                    SystemName = "Misc.OnePageCheckOutAdmin",
                    Title = "One Page CheckOut",
                    ControllerName = "MiscOnePageCheckOutAdmin",
                    ActionName = "OnePageCheckOutAdmin",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                };
                menuItemBuilder.ChildNodes.Add(menuItem);
           
                //var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Misc.OnePageCheckOutAdmin");
                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Third party plugins");
                if (pluginNode != null)
                {
                    pluginNode.ChildNodes.Add(menuItemBuilder);
                }
                else
                {
                    rootNode.ChildNodes.Add(menuItemBuilder);
                }
            }
        }

        #endregion
    }
}

using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Services.Common;

namespace Nop.Plugin.Misc.OnePageCheckOut
{
    public class CheckOutPlugIn : BasePlugin, IMiscPlugin
    {
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
            controllerName = "MiscOnePageCheckOut";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Misc.OnePageCheckOut.Controllers" }, { "area", null } };
        }

        #endregion
    }
}

using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.MobileLogin.Data;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using Nop.Services.Security;

namespace Nop.Plugin.Widgets.MobileLogin
{
    public class MobileLoginMethod : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields        
        private readonly ISettingService _settingService;       
        private readonly MobileLoginObjectContext _objectContext;
        private readonly IRepository<ScheduleTask> _scheduleRepository;
        private readonly IPermissionService _permissionService;
        #endregion

        #region Ctor
        public MobileLoginMethod(
            ISettingService settingService,
            MobileLoginObjectContext objectContext,
            IRepository<ScheduleTask> scheduleRepository,
            IPermissionService permissionService)
        {            
            this._settingService = settingService;
            this._objectContext = objectContext;
            this._scheduleRepository = scheduleRepository;
            this._permissionService = permissionService;
        }

        #endregion

        #region Utilities
        private void InstallScheduleTask()
        {
            var data = new ScheduleTask
            {
                Enabled = true,
                Name = "Make Token Invalid Mobile Login",
                Seconds = 60,
                StopOnError = false,
                Type = "Nop.Plugin.Widgets.MobileLogin.MobileLoginScheduleTask, Nop.Plugin.Widgets.MobileLogin"
            };
            _scheduleRepository.Insert(data);
        }

        private void UninstallScheduleTask()
        {
            ScheduleTask task1 = _scheduleRepository.Table.FirstOrDefault(p => p.Type == "Nop.Plugin.Widgets.MobileLogin.MobileLoginScheduleTask, Nop.Plugin.Widgets.MobileLogin");
            if (task1 != null)
            {
                _scheduleRepository.Delete(task1);
            }
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
            controllerName = "MobileLoginCustomer";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.MobileLogin.Controllers" }, { "area", null } };
        }        
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new MobileLoginSettings
            {
                
            };
            //_settingService.SaveSetting(settings);
            
            InstallScheduleTask();
            //data
            _objectContext.Install();

            //locales
            this.AddOrUpdatePluginLocaleResource("Admin.Customers.Customers.List.SearchName", "Name");
            this.AddOrUpdatePluginLocaleResource("Account.Login.Fields.MobileNumber", "Mobile Number");
            this.AddOrUpdatePluginLocaleResource("Account.Fields.Name", "Name");
            this.AddOrUpdatePluginLocaleResource("Account.Login.Fields.OTP", "OTP");
            this.AddOrUpdatePluginLocaleResource("Admin.Customers.Customers.Fields.MobileNumber", "Mobile Number");
            this.AddOrUpdatePluginLocaleResource("Account.Login.LoginWithEmail", "Login With Email Address");
            this.AddOrUpdatePluginLocaleResource("Account.Login.LoginWithMobile", "Login with Mobile Number");
            this.AddOrUpdatePluginLocaleResource("Account.Login.LoginSignUpButton", "Sign Up /Login");
            this.AddOrUpdatePluginLocaleResource("Account.SignUpButton", "Sign Up");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<MobileLoginSettings>();

            UninstallScheduleTask();
            //data
            _objectContext.Uninstall();

            //locales
            this.DeletePluginLocaleResource("Admin.Customers.Customers.List.SearchName");
            this.DeletePluginLocaleResource("Account.Login.Fields.MobileNumber");
            this.DeletePluginLocaleResource("Account.Fields.Name");
            this.DeletePluginLocaleResource("Account.Login.Fields.OTP");
            this.DeletePluginLocaleResource("Admin.Customers.Customers.Fields.MobileNumber");
            this.DeletePluginLocaleResource("Account.Login.LoginWithEmail");
            this.DeletePluginLocaleResource("Account.Login.LoginWithMobile");
            this.DeletePluginLocaleResource("Account.Login.LoginSignUpButton");
            this.DeletePluginLocaleResource("Account.SignUpButton");

            base.Uninstall();
        }

        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.NopStationMobileLoginPluginManage))
            {
                var menuItem = new SiteMapNode()
                {
                    Title = "Mobile Login",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                };

                var settingsItem = new SiteMapNode()
                {
                    Title = "Settings",
                    ControllerName = "MobileLoginCustomer",
                    ActionName = "Settings",
                    Visible = true,
                    RouteValues = new RouteValueDictionary() {{"area", null}},
                };
                menuItem.ChildNodes.Add(settingsItem);

                var mobileLoginData = new SiteMapNode()
                {
                    SystemName = "Widgets.MobileLogin",
                    Title = "Mobile Login Customers",
                    Visible = true,
                    Url = "~/MobileLoginData",
                    RouteValues = new RouteValueDictionary() {{"area", "Admin"}},
                };
                menuItem.ChildNodes.Add(mobileLoginData);

                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "NopStation MobileLoginPlugin");

                if (pluginNode != null)
                {
                    pluginNode.ChildNodes.Add(menuItem);
                }
                else
                {
                    var nopStation = new SiteMapNode()
                    {
                        Visible = true,
                        Title = "nopStation",
                        Url = "",
                        SystemName = "NopStation MobileLoginPlugin"
                    };
                    rootNode.ChildNodes.Add(nopStation);
                    nopStation.ChildNodes.Add(menuItem);
                }
            }
        }

        #endregion
    }
}
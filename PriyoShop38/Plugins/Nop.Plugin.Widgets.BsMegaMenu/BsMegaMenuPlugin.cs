using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Web.Framework.Menu;
using Nop.Web.Framework;
using Nop.Plugin.Widgets.BsMegaMenu.Models;
using Nop.Plugin.Widgets.BsMegaMenu.Data;
using Nop.Core.Data;
using Nop.Core.Domain.Tasks;
using Nop.Plugin.Widgets.BsMegaMenu.Domain;

namespace Nop.Plugin.Widgets.BsMegaMenu
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class BsMegaMenuPlugin : BasePlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        private readonly BsMegaMenuObjectContext _megaMenuObjectContext;
        private readonly IRepository<BsMegaMenuDomain> _megaMenuRepo;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly IRepository<ScheduleTask> _scheduleRepository;

        public BsMegaMenuPlugin(BsMegaMenuObjectContext context, IRepository<BsMegaMenuDomain> megaMenuRepo,
            IPictureService pictureService, 
            ISettingService settingService, IWebHelper webHelper, IRepository<ScheduleTask> scheduleRepository)
        {
            _megaMenuObjectContext = context;
            _megaMenuRepo = megaMenuRepo;
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            _scheduleRepository = scheduleRepository;
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        /// 
        #region utilites
        private void InstallScheduleTask()
        {
            //var data = new ScheduleTask
            //{
            //    Enabled = true,
            //    Name = "Cache Uvited Mega Menu",
            //    Seconds = 300,
            //    StopOnError = false,
            //    Type = "Nop.Plugin.Widgets.BsMegaMenu.MegaMenuScheduleTask, Nop.Plugin.Widgets.BsMegaMenu"
            //};
            //_scheduleRepository.Insert(data);

            

        }

        private void UninstallScheduleTask()
        {
            ScheduleTask task1 = _scheduleRepository.Table.FirstOrDefault(p => p.Type == "Nop.Plugin.Widgets.BsMegaMenu.MegaMenuScheduleTask, Nop.Plugin.Widgets.BsMegaMenu");
            if (task1!=null)
            {
                _scheduleRepository.Delete(task1);
            }
         
          
        }
        #endregion
        public override void Install()
        {

            InstallScheduleTask();
            _megaMenuObjectContext.Install();
            //settings
            var settings = new BsMegaMenuSettings()
            {

                ShowImage = true
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("admin.plugin.widgets.BsMegaMenu.save", "Save");
            this.AddOrUpdatePluginLocaleResource("admin.plugin.widgets.BsMegaMenu.settings", "Settings");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsMegaMenu.Fields.ShowImage", "Show Image");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsMegaMenu.Fields.SetWidgetZone", "Set WidgetZone");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsMegaMenu.Fields.MenuType", "Menu Type");
            this.AddOrUpdatePluginLocaleResource("Plugins.BsMegaMenu.Fields.NoOfMenufactureItems", "Number of Menufacture Items");

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _megaMenuObjectContext.Uninstall();
            //settings
            _settingService.DeleteSetting<BsMegaMenuSettings>();

            this.DeletePluginLocaleResource("admin.plugin.widgets.BsMegaMenu.save");
            this.DeletePluginLocaleResource("admin.plugin.widgets.BsMegaMenu.settings");
            this.DeletePluginLocaleResource("Plugins.BsMegaMenu.Fields.ShowImage");
            this.DeletePluginLocaleResource("Plugins.BsMegaMenu.Fields.SetWidgetZone");
            this.DeletePluginLocaleResource("Plugins.BsMegaMenu.Fields.MenuType");
            this.DeletePluginLocaleResource("Plugins.BsMegaMenu.Fields.NoOfMenufactureItems");
            UninstallScheduleTask();
            base.Uninstall();
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            var widgetZone = _settingService.LoadSetting<BsMegaMenuSettings>().SetWidgetZone;
            return new List<string>() { widgetZone };
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BsMegaMenuSetting";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.BsMegaMenu.Controllers" }, { "area", null } };
        }
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "BsMegaMenu";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.BsMegaMenu.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }
                
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                Visible = true,
                Title = "BS Mega Menu"
            };

            var manageBsMegaMenu = new SiteMapNode()
            {
                Visible = true,
                Title = "Manage BS Mega Menu",
                Url = "/BsMegaMenu/ManageBsMegaMenu"
            };

            menuItem.ChildNodes.Add(manageBsMegaMenu);

            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "nopStation");
            if (pluginNode != null)
            {
                // pluginNode.ChildNodes.Add(menuItem);
            }
            else
            {
                var nopStation = new SiteMapNode()
                {
                    Visible = true,
                    Title = "nopStation",
                    Url = "",
                    SystemName = "nopStation"
                };
                nopStation.ChildNodes.Add(menuItem);

               // rootNode.ChildNodes.Add(nopStation);
            }



            // rootNode.ChildNodes.Add(menuItem);
            //rootNode.ChildNodes.Add(menuItem);
        }

    }
}

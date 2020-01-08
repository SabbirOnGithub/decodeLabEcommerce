using System.Collections.Generic;
using System.IO;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileApp.Data;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Services.Vendors;
using Nop.Web.Framework.Menu;
using Nop.Core.Domain.Tasks;
using Nop.Core.Data;
using System.Linq;
using Nop.Services.Security;

namespace BS.Plugin.NopStation.MobileApp
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class MobileAppPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly MobileAppObjectContext _objectContext;
        private readonly IWorkContext _workContext;
        private  readonly  IRepository<ScheduleTask> _scheduleRepository;
        private readonly IPermissionService _permissionService;
        public MobileAppPlugin(IPictureService pictureService,
            ISettingService settingService, IWebHelper webHelper,
            MobileAppObjectContext objectContext, IWorkContext workContext,
            IVendorService vendorService,
            IRepository<ScheduleTask> scheduleRepository,
            IPermissionService permissionService)
        {
            this._pictureService = pictureService;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._objectContext = objectContext;
            this._workContext = workContext;
            this._scheduleRepository = scheduleRepository;
            this._permissionService = permissionService;
        }

        #region utilites
        private void InstallScheduleTask()
        {
            ScheduleTask data = new ScheduleTask
            {
                Enabled = true,
                Name = "Queued Notification Send",
                Seconds = 60, StopOnError = false,
                Type = "BS.Plugin.NopStation.MobileApp.Services.QueuedNotificationSendTask, BS.Plugin.NopStation.MobileApp"
            };
            _scheduleRepository.Insert(data);

            data = new ScheduleTask
            {

                Enabled = true,
                Name = "Insert Queued Notifications",
                Seconds = 60,
                StopOnError = false,
                Type = "BS.Plugin.NopStation.MobileApp.Services.InsertQueuedNotificationsTask, BS.Plugin.NopStation.MobileApp"
            };
            _scheduleRepository.Insert(data);

        }

        private void UninstallScheduleTask()
        {
            ScheduleTask task1 = _scheduleRepository.Table.Where(p => p.Type == "BS.Plugin.NopStation.MobileApp.Services.QueuedNotificationSendTask, BS.Plugin.NopStation.MobileApp").FirstOrDefault();
            _scheduleRepository.Delete(task1);
            ScheduleTask task2 = _scheduleRepository.Table.Where(p => p.Type == "BS.Plugin.NopStation.MobileApp.Services.InsertQueuedNotificationsTask, BS.Plugin.NopStation.MobileApp").FirstOrDefault();
            _scheduleRepository.Delete(task2);
        }  
        #endregion

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "BsNotificationAdmin";
            routeValues = new RouteValueDictionary { { "Namespaces", "BS.Plugin.NopStation.MobileApp.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        
        
        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            #region Local Resources

            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Test.Push", "Test Push");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List", "Groups");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List.Header", "Group List");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.EditGroup", "Edit Group");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Title.AddNew", "Create Group");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.AddNew", "Create Group");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Manage", "Manage Schedules");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Header", "Schedule List");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.EditSchedule", "Edit Schedule");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.EditSchedule", "Edit Schedule");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.CreateSchedule", "Create Schedule");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.CreateSchedule", "Create Schedule");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Manage", "Queue");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Header", "Queue List");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Manage", "Manage Device");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Header", "Device List");

            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Common.BackToList", "Back to List");
            this.AddOrUpdatePluginLocaleResource("Admin.Plugin.BsNotification.Groups.Fields.Name", "Group Name");

            #endregion

            _objectContext.Install();

            InstallScheduleTask();
            base.Install();
           
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            #region Local Resources

            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Test.Push");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Group.List.Header");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.EditGroup");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Title.AddNew");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Groups.Header.AddNew");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Manage");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Schedules.Header");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.EditSchedule");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.EditSchedule");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Title.CreateSchedule");
            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Schedule.Header.CreateSchedule");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Manage");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Queued.Header");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Manage");
            this.DeletePluginLocaleResource("Admin.Plugin.Misc.BsNotificaton.Device.Header");

            this.DeletePluginLocaleResource("Admin.Plugin.NopStation.MobileApp.Common.BackToList");
            this.DeletePluginLocaleResource("Admin.Plugin.BsNotification.Groups.Fields.Name");

            #endregion
           
            //_objectContext.Uninstall();
            _objectContext.Uninstall();
            UninstallScheduleTask();
            base.Uninstall();
        }

        

        public bool Authenticate()
        {
            return true;
        }



        public void ManageSiteMap(SiteMapNode rootNode)
        {
            if (_permissionService.Authorize(StandardPermissionProvider.AdvertiseMentSystemManage))
            {

                var menuItemBuilder = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Mobile Notification App",

                };

                #region notification message template

                var messageTemplateNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Message Template",

                };
                messageTemplateNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Templates",
                    Url = "/Admin/Plugin/NopStation/MobileApp/MessageTemplateList",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                messageTemplateNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Message Template",
                    Url = "/Admin/Plugin/NopStation/MobileApp/NotificationMessageTemplateCreate",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                menuItemBuilder.ChildNodes.Add(messageTemplateNode);

                #endregion

                #region device

                var deviceNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Customer Devices",

                };
                deviceNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Device List",
                    Url = "/Admin/Plugin/NopStation/MobileApp/DeviceList",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                //deviceNode.ChildNodes.Add(new SiteMapNode
                //{
                //    Visible = true,
                //    Title = "Create Sample Device",
                //    Url = "/Admin/Plugin/NopStation/MobileApp/CreateDevice",
                //    RouteValues = new RouteValueDictionary() { { "Area", "Admin" } }
                //});
                menuItemBuilder.ChildNodes.Add(deviceNode);

                #endregion

                #region queued

                var queuedNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Queued Notification",

                };
                queuedNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Queued List",
                    Url = "/Admin/Plugin/NopStation/MobileApp/QueuedList",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });

                menuItemBuilder.ChildNodes.Add(queuedNode);

                #endregion

                #region group

                var groupNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Group",

                };
                groupNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Customer Groups",
                    Url = "/Admin/Plugin/NopStation/MobileApp/SmartGroups",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                groupNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Group",
                    Url = "/Admin/Plugin/NopStation/MobileApp/CreateGroup",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                menuItemBuilder.ChildNodes.Add(groupNode);

                #endregion

                #region schedule

                var scheduleNode = new SiteMapNode()
                {
                    Visible = true,
                    Title = "Schedule",

                };
                scheduleNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Schedules",
                    Url = "/Admin/Plugin/NopStation/MobileApp/ScheduleList",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                scheduleNode.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Create Schedule",
                    Url = "/Admin/Plugin/NopStation/MobileApp/CreateSchedule",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });
                menuItemBuilder.ChildNodes.Add(scheduleNode);

                #endregion

                #region test push

                menuItemBuilder.ChildNodes.Add(new SiteMapNode
                {
                    Visible = true,
                    Title = "Test Push",
                    Url = "/Admin/Plugin/NopStation/MobileApp/Test",
                    RouteValues = new RouteValueDictionary() {{"Area", "Admin"}}
                });

                #endregion



                // rootNode.ChildNodes.Add(menuItemBuilder);

                var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Advertisement plugins");
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

     
    }
}

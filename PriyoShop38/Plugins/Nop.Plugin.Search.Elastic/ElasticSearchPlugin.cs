using System;
using System.Linq;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Tasks;
using Nop.Core.Domain.Tasks;

namespace Nop.Plugin.Search.Elastic
{
    /// <summary>
    /// Represents the Elastic Search provider
    /// </summary>
    public class ElasticSearchPlugin : BasePlugin, IMiscPlugin
    {
        private readonly ElasticSearchSettings _searchSettings;
        private readonly ISettingService _settingService;
        private readonly IScheduleTaskService _scheduleTaskService;
        public ElasticSearchPlugin(
            ElasticSearchSettings searchSettings,
            ISettingService settingService,
            IScheduleTaskService scheduleTaskService
)
        {
            this._searchSettings = searchSettings;
            _settingService = settingService;
            _scheduleTaskService = scheduleTaskService;
            
        }

        #region utilities
        private const string ElasticSearchProductsSyncTaskType = "Nop.Plugin.Search.Elastic.Tasks.ProductSyncTask, Nop.Plugin.Search.Elastic";
        private ScheduleTask FindElasticSearchProductsSyncTask()
        {
            return _scheduleTaskService.GetTaskByType(ElasticSearchProductsSyncTaskType);
        }

        private void InstallElasticSearchProductsSyncTask()
        {
            var task = FindElasticSearchProductsSyncTask();
            if (task == null)
            {
                task = new ScheduleTask
                {
                    Name = "Sync Elastic Search  Products",
                    Seconds = 86400,
                    Type = ElasticSearchProductsSyncTaskType,
                    Enabled = false,
                    StopOnError = false,
                };
                _scheduleTaskService.InsertTask(task);
            }
        }

        private void UnInstallElasticSearchProductsSyncTask()
        {
            var task = FindElasticSearchProductsSyncTask();
            if (task != null)
            {

                _scheduleTaskService.DeleteTask(task);
            }
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
            controllerName = "ElasticSearchConfigure";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Search.Elastic.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            //settings
            var settings = new ElasticSearchSettings()
            {
                ElasticBulkSize=1000,
                ProviderName="Elatic",
                ElasticSearchServerUrl= "http://localhost:9200/",
                StoreName= "Elatic"
            };
            _settingService.SaveSetting(settings);

            InstallElasticSearchProductsSyncTask();
            //locales
            //  this.AddOrUpdatePluginLocaleResource("Plugins.Search.Elastic.TestFailed", "Test message sending failed");


            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<ElasticSearchSettings>();
            UnInstallElasticSearchProductsSyncTask();
            //locales
            // this.DeletePluginLocaleResource("Plugins.Search.Elastic.TestFailed");


            base.Uninstall();
        }
    }
}

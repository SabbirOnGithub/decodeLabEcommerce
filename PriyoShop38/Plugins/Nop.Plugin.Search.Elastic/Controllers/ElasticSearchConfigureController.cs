using System;
using System.Web.Mvc;
using Nop.Core.Plugins;
using Nop.Plugin.Search.Elastic.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Services.Tasks;
using Nop.Plugin.Search.Elastic.Services;

namespace Nop.Plugin.Search.Elastic.Controllers
{
    [AdminAuthorize]
    public class ElasticSearchConfigureController : BasePluginController
    {
        private readonly ElasticSearchSettings _elasticSearchSettings;
        private readonly ISettingService _settingService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ILocalizationService _localizationService;
        private readonly IElasticSearchService _elasticSearchService;

        public ElasticSearchConfigureController(ElasticSearchSettings verizonSettings,
            ISettingService settingService, 
            IPluginFinder pluginFinder,
            ILocalizationService localizationService,
            IElasticSearchService elasticSearchService)
        {
            this._elasticSearchSettings = verizonSettings;
            this._settingService = settingService;
            this._pluginFinder = pluginFinder;
            this._localizationService = localizationService;
            this._elasticSearchService = elasticSearchService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var model = new ConfigurationModel();
            model.ProviderName = _elasticSearchSettings.ProviderName;
            model.ElasticBulkSize = _elasticSearchSettings.ElasticBulkSize;
            model.ElasticSearchServerUrl = _elasticSearchSettings.ElasticSearchServerUrl;
            model.StoreName = _elasticSearchSettings.StoreName;
            return View("~/Plugins/Search.Elastic/Views/ElasticSearch/Configure.cshtml", model);
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public ActionResult ConfigurePOST(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _elasticSearchSettings.ProviderName = model.ProviderName;
            _elasticSearchSettings.ElasticSearchServerUrl = model.ElasticSearchServerUrl;
            _elasticSearchSettings.ElasticBulkSize = model.ElasticBulkSize;
            _elasticSearchSettings.StoreName = model.StoreName;
            _settingService.SaveSetting(_elasticSearchSettings);
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }

        [ChildActionOnly]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("reindex")]
        public ActionResult Reindex(ConfigurationModel model)
        {
            try
            {
               
                    var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Search.Elastic");
                    if (pluginDescriptor == null)
                        throw new Exception("Cannot load the plugin");
                    var plugin = pluginDescriptor.Instance() as ElasticSearchPlugin;
                    if (plugin == null)
                        throw new Exception("Cannot load the plugin");

              var reindexDone=  _elasticSearchService.ReIndex();
                if(reindexDone)
                    SuccessNotification("Renindex Successfully!");

                if (!reindexDone)
                    ErrorNotification("Renindex faild!");


            }
            catch(Exception exc)
            {
                //ignore
            }

            return View("~/Plugins/Search.Elastic/Views/ElasticSearch/Configure.cshtml", model);
        }
    }
}
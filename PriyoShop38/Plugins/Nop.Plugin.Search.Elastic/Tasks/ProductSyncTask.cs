using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Search.Elastic.Services;
using Nop.Services.Logging;
using Nop.Services.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Search.Elastic.Tasks
{
   

    public class ProductSyncTask : ITask
    {
        private readonly IPluginFinder _pluginFinder;
        private readonly IElasticSearchService _elasticSearchService;
        private readonly ILogger _logger;

        public ProductSyncTask(
            IPluginFinder pluginFinder,
            IElasticSearchService elasticSearchService,
            ILogger logger)
        {
            _pluginFinder = pluginFinder;
            _elasticSearchService = elasticSearchService;
            _logger = logger;
        }

        public void Execute()
        {
           

            try
            {
                var pluginDescriptor = _pluginFinder.GetPluginDescriptorBySystemName("Search.Elastic");
                if (pluginDescriptor == null)
                {
                    _logger.Error("Elastic Search (Search.Elastic) plugin cound not found");
                    return;
                }

               
                    var exportData = _elasticSearchService.SyncProducts();
                    if (!exportData)
                    {
                        _logger.Warning("Elastic Product Sync Task failed!");
                    }
               
            }
            catch (Exception e)
            {

                _logger.Warning("ElasticSearch Export Task", e);
            }
        }
    }
}

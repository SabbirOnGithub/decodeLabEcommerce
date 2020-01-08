
using Nop.Core.Configuration;

namespace Nop.Plugin.Search.Elastic
{
    public class ElasticSearchSettings : ISettings
    {
        public string ProviderName { get; set; }
        public string StoreName { get; set; }
        public string ElasticSearchServerUrl { get; set; }

        public int ElasticBulkSize { get; set; } = 1000;
    }
}
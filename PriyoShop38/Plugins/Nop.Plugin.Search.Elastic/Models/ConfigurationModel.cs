using Nop.Web.Framework;

namespace Nop.Plugin.Search.Elastic.Models
{
    public class ConfigurationModel
    {
        public string ProviderName { get; set; }
        public string ElasticSearchServerUrl { get; set; }
        public int ElasticBulkSize { get; set; }
        public string StoreName { get; set; }
    }
}
using Newtonsoft.Json;
using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class AlgoliaReplica
    {
        public string Name { get; set; }
        public string SortType { get; set; }
    }

    public class AlgoliaSynonymModel
    {
        public string ObjectId { get; set; }
        [NopResourceDisplayName("Plugin.AlgoliaSearch.AlgoliaSynonymModel.Synonyms")]
        public string Synonyms { get; set; }
    }

    public class AlgoliaSynonymJsonModel
    {
        public AlgoliaSynonymJsonModel()
        {
            Synonyms = new List<string>();
        }

        [JsonProperty(PropertyName = "objectID")]
        public string ObjectId { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
        [JsonProperty(PropertyName = "synonyms")]
        public List<string> Synonyms { get; set; }
    }
}

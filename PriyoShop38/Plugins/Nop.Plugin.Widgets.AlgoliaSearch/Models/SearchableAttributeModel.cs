using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class SearchableAttributeListModel
    {
        public SearchableAttributeListModel()
        {
            Attributes = new List<SearchableAttributeModel>();
            SelectableProperties = new List<string>();
        }

        public IList<SearchableAttributeModel> Attributes { get; set; }

        public IList<string> SelectableProperties { get; set; }

        public string[] PropertyName { get; set; }

        public bool[] IsOrdered { get; set; }

        public class SearchableAttributeModel
        {
            public string PropertyName { get; set; }

            public bool IsOrdered { get; set; }

            public bool ChangableModifier { get; set; }
        }
    }
}

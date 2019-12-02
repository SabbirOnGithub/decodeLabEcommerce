using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct
{
    public class AlgoliaPriceRangeModel
    {
        public string CurrencySymbol { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public decimal CurrentMinPrice { get; set; }
        public decimal CurrentMaxPrice { get; set; }
    }
}

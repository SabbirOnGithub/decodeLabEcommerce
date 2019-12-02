using Nop.Core;
using Nop.Web.Framework.UI.Paging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models
{
    public class AlgoliaPagingFilteringModel : BasePageableModel
    {
        public AlgoliaPagingFilteringModel()
        {
            SelectedCategoryIds = new List<int>();
            SelectedManufacturerIds = new List<int>();
            SelectedRatings = new List<int>();
            SelectedVendorIds = new List<int>();
        }

        public IList<int> SelectedManufacturerIds { get; set; }

        public IList<int> SelectedVendorIds { get; set; }

        public IList<int> SelectedCategoryIds { get; set; }

        public IList<string> SelectedSpecs { get; set; }

        public IList<int> SelectedRatings { get; set; }

        public decimal MaxPrice { get; set; }

        public decimal MinPrice { get; set; }

        public bool EmiProductsOnly { get; set; }

        public int? OrderBy { get; set; }

        public string ViewMode { get; set; }

        public string q { get; set; }

        public bool IncludeEkshopProducts { get; set; }

        public void LoadFilters(IWebHelper webHelper)
        { 
            var mafStr = webHelper.QueryString<string>("m");
            if (!string.IsNullOrWhiteSpace(mafStr))
                SelectedManufacturerIds = mafStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            var vendStr = webHelper.QueryString<string>("v");
            if (!string.IsNullOrWhiteSpace(vendStr))
                SelectedVendorIds = vendStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
            
            var catStr = webHelper.QueryString<string>("c");
            if (!string.IsNullOrWhiteSpace(catStr))
                SelectedCategoryIds = catStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            var rateStr = webHelper.QueryString<string>("r");
            if (!string.IsNullOrWhiteSpace(rateStr))
                SelectedRatings = rateStr.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            var emiStr = webHelper.QueryString<string>("emi");
            if (bool.TryParse(emiStr, out bool emiOnly))
                EmiProductsOnly = emiOnly;
        }
    }
}

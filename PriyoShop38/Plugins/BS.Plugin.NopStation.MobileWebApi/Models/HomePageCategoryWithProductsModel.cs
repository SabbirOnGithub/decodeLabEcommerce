using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models
{
    public class HomePageCategoryWithProductsModel : BaseNopEntityModel
    {
        public HomePageCategoryWithProductsModel()
        {
            Products = new List<ProductOverViewModelApi>();
        }

        public int CategoryId { get; set; }

        public string TextPrompt { get; set; }

        public string IconUrl { get; set; }

        public bool ForBlApp { get; set; }

        public byte ApplicableFor { get; set; }

        public int? PercentValue { get; set; }

        public DateTime? ActiveStartDate { get; set; }

        public DateTime? ActiveEndDate { get; set; }

        public IList<ProductOverViewModelApi> Products { get; set; }
    }
}

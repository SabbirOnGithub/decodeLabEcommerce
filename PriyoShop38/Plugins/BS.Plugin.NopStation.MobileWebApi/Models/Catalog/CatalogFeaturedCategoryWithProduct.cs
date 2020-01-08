using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CatalogFeaturedCategoryWithProduct
    {
        public CatalogFeaturedCategoryWithProduct()
        {
            this.Category= new CategoryOverViewModelApi();
            this.Product= new List<ProductOverViewModelApi>();
            this.SubCategory = new List<SubCategoriesWithNameAndId>();
        }
        public CategoryOverViewModelApi Category { get; set; }
        public IEnumerable<ProductOverViewModelApi> Product { get; set; }
        public IEnumerable<SubCategoriesWithNameAndId> SubCategory { get; set; }

        #region Nested Class
        public partial class SubCategoriesWithNameAndId
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string IconPath { get; set; }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Models.Media;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CategoryModelApi :CategoryBaseModelApi
    {
        public CategoryModelApi()
        {
            PictureModel = new PictureModel();
            FeaturedProducts = new List<ProductOverViewModelApi>();
            Products = new List<ProductOverViewModelApi>();
            PagingFilteringContext = new CatalogPagingFilteringModel();
            SubCategories = new List<SubCategoryModelApi>();
            CategoryBreadcrumb = new List<CategoryModelApi>();
        }

        public string Description { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
        public string SeName { get; set; }
        
        public PictureModel PictureModel { get; set; }

        public CatalogPagingFilteringModel PagingFilteringContext { get; set; }

        public bool DisplayCategoryBreadcrumb { get; set; }
        public IList<CategoryModelApi> CategoryBreadcrumb { get; set; }
        
        public IList<SubCategoryModelApi> SubCategories { get; set; }

        public IList<ProductOverViewModelApi> FeaturedProducts { get; set; }
        public IList<ProductOverViewModelApi> Products { get; set; }
    }
}

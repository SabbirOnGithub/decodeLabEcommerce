using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;

namespace BS.Plugin.NopStation.MobileWebApi.Models.Catalog
{
    public class CategoriesAndManufacturersModelApi : BaseResponse
    {
        public CategoriesAndManufacturersModelApi()
        {
            Categories = new List<CatalogSearchyModelApi>();
            Manufacturers = new List<CatalogSearchyModelApi>();
        }
        public List<CatalogSearchyModelApi> Categories { get; set; }
        public List<CatalogSearchyModelApi> Manufacturers { get; set; } 
    }

    public class CatalogSearchyModelApi
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

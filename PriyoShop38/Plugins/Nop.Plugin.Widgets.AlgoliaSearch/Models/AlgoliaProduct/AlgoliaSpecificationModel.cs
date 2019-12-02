using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct
{
    public class AlgoliaSpecificationModel : ProductSpecificationModel
    {
        public AlgoliaSpecificationModel(ProductSpecificationModel obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(obj, null), null);
            }
        }

        public AlgoliaSpecificationModel()
        { }

        public string IdOption { get; set; } 
        public string IdOptionGroup { get; set; }
    }
}

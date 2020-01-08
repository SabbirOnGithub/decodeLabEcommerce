using Nop.Web.Models.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct
{
    public class AlgoliaVendorModel : VendorBriefInfoModel
    {
        public AlgoliaVendorModel()
        {

        }

        public AlgoliaVendorModel(VendorBriefInfoModel obj)
        {
            foreach (var prop in obj.GetType().GetProperties())
            {
                this.GetType().GetProperty(prop.Name).SetValue(this, prop.GetValue(obj, null), null);
            }
        }

        public string IdName { get; set; }
    }
}

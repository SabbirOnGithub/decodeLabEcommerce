using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public partial class ProductUnpublishRequestByVendorListModel : BaseNopModel
    {
        public ProductUnpublishRequestByVendorListModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
        public int SearchVendorId { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }
    }
}
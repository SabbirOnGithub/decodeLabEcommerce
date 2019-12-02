using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Vendors
{
    public partial class VendorListModel : BaseNopModel
    {
        [NopResourceDisplayName("Admin.Vendors.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }

        [NopResourceDisplayName("Admin.Vendors.List.SearchVendorStatus")]
        public VendorStatus SearchVendorStatus { get; set; }

        public IList<SelectListItem> AvailableStatuses { get; set; }
    }
}
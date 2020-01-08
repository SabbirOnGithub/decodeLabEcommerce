using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Vendors
{
    public class VendorHistoryModel : BaseNopEntityModel
    {
        public int VendorId { get; set; }
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.VendorHistory.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.VendorHistory.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.VendorHistory.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
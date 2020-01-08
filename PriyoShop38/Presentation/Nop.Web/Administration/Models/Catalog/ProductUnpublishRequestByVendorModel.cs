using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Nop.Admin.Models.Vendors;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class ProductUnpublishRequestByVendorModel : BaseNopModel
    {
        public VendorModel Vendor { get; set; }     
        public ProductModel Product { get; set; }
        public DateTime CreatedOnUtc { get; set; }
    }
}
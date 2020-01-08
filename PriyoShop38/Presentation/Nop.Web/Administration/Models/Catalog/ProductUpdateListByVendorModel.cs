using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models.Catalog
{
    public class ProductUpdateListByVendorModel : BaseNopModel
    {
        public ProductUpdateListByVendorModel()
        {
            AvailableVendors = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Admin.Catalog.Products.List.SearchVendor")]
        public int SearchVendorId { get; set; }
        [NopResourceDisplayName("Admin.Catalog.VendorUpdatesProducts.List.StartDate")]
        [UIHint("DateNullable")]
        public DateTime? StartDate { get; set; }
        [UIHint("DateNullable")]
        [NopResourceDisplayName("Admin.Catalog.ProductUpdateByVendor.List.EndDate")]
        public DateTime? EndDate { get; set; }

        public IList<SelectListItem> AvailableVendors { get; set; }
    }
}
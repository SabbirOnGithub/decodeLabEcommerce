using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class ProductHistoryModel : BaseNopEntityModel
    {
        public int ProductId { get; set; }
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductHistory.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductHistory.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ProductHistory.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
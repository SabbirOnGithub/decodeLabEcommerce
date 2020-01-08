using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class CategoryHistoryModel : BaseNopEntityModel
    {
        public int CategoryId { get; set; }
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.CategoryHistory.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Catalog.CategoryHistory.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.CategoryHistory.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
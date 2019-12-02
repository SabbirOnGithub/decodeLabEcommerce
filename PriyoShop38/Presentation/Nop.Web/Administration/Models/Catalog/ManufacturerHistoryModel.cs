using System;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class ManufacturerHistoryModel : BaseNopEntityModel
    {
        public int ManufacturerId { get; set; }
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ManufacturerHistory.Fields.CustomerEmail")]
        public string CustomerEmail { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ManufacturerHistory.Fields.Description")]
        public string Description { get; set; }

        [NopResourceDisplayName("Admin.Catalog.ManufacturerHistory.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
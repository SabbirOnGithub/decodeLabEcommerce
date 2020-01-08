using Nop.Core.Domain.Vendors;
using System;

namespace Nop.Core.Domain.Catalog
{
    public partial class ProductUnpublishRequestByVendor : BaseEntity
    {
        public int ProductId { get; set; }
        public DateTime CreatedOnUtc { get; set; }

        public virtual Product Product { get; set; }
    }
}

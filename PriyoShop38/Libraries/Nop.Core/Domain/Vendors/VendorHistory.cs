using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents an history of a vendor
    /// </summary>
    public partial class VendorHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the vendor identifier
        /// </summary>
        public int VendorId { get; set; }

        /// <summary>
        /// Gets or sets the customer identifier
        /// </summary>
        public int CustomerId { get; set; }


        /// <summary>
        /// Gets or sets the note
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time of record note creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets the vendor
        /// </summary>
        public virtual Vendor Vendor { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}

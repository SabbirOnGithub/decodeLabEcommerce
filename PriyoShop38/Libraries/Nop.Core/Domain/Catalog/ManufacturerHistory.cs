using System;
using Nop.Core.Domain.Customers;

namespace Nop.Core.Domain.Catalog
{
    /// <summary>
    /// Represents an history of a Manufacturer
    /// </summary>
    public partial class ManufacturerHistory : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Manufacturer identifier
        /// </summary>
        public int ManufacturerId { get; set; }

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
        /// Gets the Manufacturer
        /// </summary>
        public virtual Manufacturer Manufacturer { get; set; }

        /// <summary>
        /// Gets the customer
        /// </summary>
        public virtual Customer Customer { get; set; }
    }
}

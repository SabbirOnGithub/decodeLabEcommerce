using System;
using System.Collections.Generic;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Seo;

namespace Nop.Core.Domain.Vendors
{
    /// <summary>
    /// Represents a vendor
    /// </summary>
    public partial class Vendor : BaseEntity, ILocalizedEntity, ISlugSupported
    {
        private ICollection<Discount> _appliedDiscounts;
        private ICollection<VendorNote> _vendorNotes;
        private ICollection<VendorRestrictedPaymentMethod> _restrictedPaymentMethods;

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the short description
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Gets or sets the picture identifier
        /// </summary>
        public int PictureId { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity is active
        /// </summary>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }

        /// <summary>
        /// Gets or sets the display order
        /// </summary>
        public int DisplayOrder { get; set; }


        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        /// Gets or sets the page size
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether customers can select the page size
        /// </summary>
        public bool AllowCustomersToSelectPageSize { get; set; }

        /// <summary>
        /// Gets or sets the available customer selectable page size options
        /// </summary>
        public string PageSizeOptions { get; set; }


        /// <summary>
        /// Gets or sets vendor notes
        /// </summary>
        public virtual ICollection<VendorNote> VendorNotes
        {
            get { return _vendorNotes ?? (_vendorNotes = new List<VendorNote>()); }
            protected set { _vendorNotes = value; }
        }

        public virtual ICollection<VendorRestrictedPaymentMethod> RestrictedPaymentMethods
        {
            get { return _restrictedPaymentMethods ?? (_restrictedPaymentMethods = new List<VendorRestrictedPaymentMethod>()); }
            protected set { _restrictedPaymentMethods = value; }
        }

        public virtual ICollection<Discount> AppliedDiscounts
        {
            get { return _appliedDiscounts ?? (_appliedDiscounts = new List<Discount>()); }
            protected set { _appliedDiscounts = value; }
        }


        #region BS-23

        public bool IsFreeShipping { get; set; }

        public decimal AffiliateCommissionRate { get; set; }

        public bool RecentlyUpdated { get; set; }

        public int LastUpdatedBy { get; set; }

        public DateTime UpdatedOnUtc { get; set; }

        #endregion
    }
}

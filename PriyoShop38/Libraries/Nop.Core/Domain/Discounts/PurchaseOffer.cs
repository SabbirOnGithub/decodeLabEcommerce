using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;

namespace Nop.Core.Domain.Discounts
{
    public partial class PurchaseOffer : BaseEntity
    {
        private ICollection<PurchaseOfferCategory> _appliedToCategories;
        private ICollection<PurchaseOfferManufacturer> _appliedToManufacturers;
        private ICollection<PurchaseOfferProduct> _appliedToProducts;
        private ICollection<PurchaseOfferVendor> _appliedToVendors;

        public decimal MinimumCartAmount { get; set; }

        public int GiftProductId { get; set; }

        public int Quantity { get; set; }

        public DateTime? StartDateUtc { get; set; }
        
        public DateTime? EndDateUtc { get; set; }

        public bool ForAllProducts { get; set; }


        public virtual Product GiftProduct { get; set; }

        public virtual ICollection<PurchaseOfferCategory> AppliedToCategories
        {
            get { return _appliedToCategories ?? (_appliedToCategories = new List<PurchaseOfferCategory>()); }
            protected set { _appliedToCategories = value; }
        }

        public virtual ICollection<PurchaseOfferManufacturer> AppliedToManufacturers
        {
            get { return _appliedToManufacturers ?? (_appliedToManufacturers = new List<PurchaseOfferManufacturer>()); }
            protected set { _appliedToManufacturers = value; }
        }

        public virtual ICollection<PurchaseOfferProduct> AppliedToProducts
        {
            get { return _appliedToProducts ?? (_appliedToProducts = new List<PurchaseOfferProduct>()); }
            protected set { _appliedToProducts = value; }
        }

        public virtual ICollection<PurchaseOfferVendor> AppliedToVendors
        {
            get { return _appliedToVendors ?? (_appliedToVendors = new List<PurchaseOfferVendor>()); }
            protected set { _appliedToVendors = value; }
        }
    }
}

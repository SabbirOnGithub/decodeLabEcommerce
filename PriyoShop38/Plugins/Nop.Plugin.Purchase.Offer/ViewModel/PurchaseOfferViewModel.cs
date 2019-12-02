using Nop.Web.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Nop.Plugin.Purchase.Offer.ViewModel
{
    public class PurchaseOfferViewModel
    {
        public PurchaseOfferViewModel()
        {
            Options = new List<PurchaseOfferOptionViewModel>();
        }

        [NopResourceDisplayName("Admin.Purchase.Offer.Id")]
        public virtual int Id { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.Name")]
        public virtual string Name { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.IsActive")]
        public virtual bool IsActive { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.ShowNotificationOnCart")]
        public virtual bool ShowNotificationOnCart { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.ValidFrom")]
        [Required]
        public virtual DateTime ValidFrom { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.ValidTo")]
        [Required]
        public virtual DateTime ValidTo { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.CreatedOnUtc")]
        public virtual DateTime CreatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.UpdatedOnUtc")]
        public virtual DateTime UpdatedOnUtc { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.Description")]
        public string Description { get; set; }

        public List<PurchaseOfferOptionViewModel> Options { get; set; }
        public PurchaseOfferOptionViewModel GainedOption { get; set; }
    }
}

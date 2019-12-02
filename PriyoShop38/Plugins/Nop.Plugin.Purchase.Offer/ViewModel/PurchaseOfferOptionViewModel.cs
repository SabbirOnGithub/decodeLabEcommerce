using Nop.Web.Framework;
using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Nop.Plugin.Purchase.Offer.ViewModel
{
    public class PurchaseOfferOptionViewModel
    {
        public PurchaseOfferOptionViewModel()
        {
            ProductImage = "/Plugins/Purchase.Offer/Content/dummy.jpg";
        }

        [NopResourceDisplayName("Admin.Purchase.Offer.Id")]
        public virtual int Id { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.Quantity")]
        public virtual int Quantity { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.MinimumPurchaseAmount")]
        public virtual decimal MinimumPurchaseAmount { get; set; }
        public virtual string MinimumPurchaseAmountStr { get; set; }

        [NopResourceDisplayName("Admin.Purchase.Offer.Note")]
        public virtual string Note { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.ProductName")]
        public string ProductName { get; set; }
        [NopResourceDisplayName("Admin.Purchase.Offer.ProductImage")]
        public string ProductImage { get; set; }
        [UIHint("Picture")]
        [NopResourceDisplayName("Admin.Purchase.Offer.ProductImage")]
        public int PictureId { get; set; }
        public HttpPostedFileBase ImageBaseFile { get; set; }
        public string Message { get; set; }
        public bool OfferAvailable { get; set; }
        public string OfferName { get; set; }
        public bool ShowNotificationOnCart { get; set; }
        public bool OfferActive { get; set; }
    }
}

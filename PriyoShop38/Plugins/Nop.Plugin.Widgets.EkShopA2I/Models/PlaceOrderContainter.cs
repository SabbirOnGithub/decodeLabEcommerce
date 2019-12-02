using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.Tax;
using Nop.Services.Orders;
using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class PlaceOrderContainter
    {
        public PlaceOrderContainter()
        {
            this.Cart = new List<ShoppingCartItem>();
            this.AppliedDiscounts = new List<Discount>();
            this.AppliedGiftCards = new List<AppliedGiftCard>();
        }

        public Customer UdcCustomer { get; set; }
        public Customer A2iCustomer { get; set; }
        public Language CustomerLanguage { get; set; }
        public int AffiliateId { get; set; }
        public TaxDisplayType CustomerTaxDisplayType { get; set; }
        public string CustomerCurrencyCode { get; set; }
        public decimal CustomerCurrencyRate { get; set; }

        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
        public ShippingStatus ShippingStatus { get; set; }
        public string ShippingMethodName { get; set; }
        public string ShippingRateComputationMethodSystemName { get; set; }
        public bool PickUpInStore { get; set; }
        public Address PickupAddress { get; set; }

        public bool IsRecurringShoppingCart { get; set; }
        //initial order (used with recurring payments)
        public Order InitialOrder { get; set; }

        public string CheckoutAttributeDescription { get; set; }
        public string CheckoutAttributesXml { get; set; }

        public IList<ShoppingCartItem> Cart { get; set; }
        public List<Discount> AppliedDiscounts { get; set; }
        public List<AppliedGiftCard> AppliedGiftCards { get; set; }

        public decimal OrderSubTotalInclTax { get; set; }
        public decimal OrderSubTotalExclTax { get; set; }
        public decimal OrderSubTotalDiscountInclTax { get; set; }
        public decimal OrderSubTotalDiscountExclTax { get; set; }
        public decimal OrderShippingTotalInclTax { get; set; }
        public decimal OrderShippingTotalExclTax { get; set; }
        public decimal PaymentAdditionalFeeInclTax { get; set; }
        public decimal PaymentAdditionalFeeExclTax { get; set; }
        public decimal OrderTaxTotal { get; set; }
        public string VatNumber { get; set; }
        public string TaxRates { get; set; }
        public decimal OrderDiscountAmount { get; set; }
        public int RedeemedRewardPoints { get; set; }
        public decimal RedeemedRewardPointsAmount { get; set; }
        public decimal OrderTotal { get; set; }
        public bool Confident { get; set; }
    }
}

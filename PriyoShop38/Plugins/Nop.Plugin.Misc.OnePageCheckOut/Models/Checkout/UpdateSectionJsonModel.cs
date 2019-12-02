
namespace Nop.Plugin.Misc.OnePageCheckOut.Models.Checkout
{
    public class UpdateSectionJsonModel
    {
        public string name { get; set; }
        public string ShippingAdressUpdateHtml { get; set; }
        public string ShippingMethodUpdateHtml { get; set; }
        public string OrderTotalHtml { get; set; }

        public string PaymentInfoHtml{get; set;}

        public string ShoppigCartHtml { get; set; }

        public string EMIPaymentMethodHtml { get; set; }

        public bool warning { get; set; }

    }
}
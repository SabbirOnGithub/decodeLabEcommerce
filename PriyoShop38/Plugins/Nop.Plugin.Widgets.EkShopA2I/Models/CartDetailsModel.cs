using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class CartDetailsModel
    {
        public CartDetailsModel()
        {
            cart = new List<CartListModel>();
            cart_options = new CartOptions();
        }

        public List<CartListModel> cart { get; set; }
        public CartOptions cart_options { get; set; }
        public decimal shipping_cost { get; set; }
    }

    public class CartOptions
    {
        public CartOptions()
        {
            discount_details = new DiscountDetails();
        }

        public int cart_id { get; set; }
        public string other_required_data { get; set; }
        public DiscountDetails discount_details { get; set; }
    }

    public class DiscountDetails
    {
        public decimal discount_amount { get; set; }
    }

    public class CartListModel
    {
        public int id { get; set; }
        public string name { get; set; }
        public string product_url { get; set; }
        public string image { get; set; }
        public decimal unit_price { get; set; }
        public decimal price { get; set; }
        public decimal commission { get; set; }
        public int qty { get; set; }
    }
}

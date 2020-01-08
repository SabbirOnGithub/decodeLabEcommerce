using System.Collections.Generic;

namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class PlaceOrderDetailsModel
    {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public int unit_price { get; set; }
        public int quantity { get; set; }
        public int price { get; set; }
        public string variation_id { get; set; }
        public string option { get; set; }
    }

    public class PlaceOrderCartOptionsModel
    {
        public int cart_id { get; set; }
        public string other_required_data { get; set; }
    }

    public class PlaceOrderOrderModel
    {
        public List<PlaceOrderDetailsModel> order_details { get; set; }
        public string lp_code { get; set; }
        public string lp_name { get; set; }
        public string lp_contact_person { get; set; }
        public string lp_contact_number { get; set; }
        public int lp_delivery_charge { get; set; }
        public string lp_location { get; set; }
        public string delivery_duration { get; set; }
        public string recipient_center_name { get; set; }
        public string recipient_name { get; set; }
        public string recipient_mobile { get; set; }
        public string recipient_email { get; set; }
        public string recipient_division { get; set; }
        public string recipient_district { get; set; }
        public string recipient_upazila { get; set; }
        public string recipient_union { get; set; }
        public int payment_method { get; set; }
        public PlaceOrderCartOptionsModel cart_options { get; set; }
        public int total { get; set; }
        public string order_code { get; set; }
    }

    public class PlaceOrderModel
    {
        public PlaceOrderOrderModel order { get; set; }
    }

    //public class EsPlaceOrderModel
    //{
    //    public EsPlaceOrderModel()
    //    {
    //        order_details = new List<EsPlaceOrderDetailsModel>();
    //        cart_options = new EsPlaceOrderCartOptionsModel();
    //    }

    //    public List<EsPlaceOrderDetailsModel> order_details { get; set; }
    //    public string order_code { get; set; }
    //    public string lp_code { get; set; }
    //    public string lp_name { get; set; }
    //    public string lp_contact_person { get; set; }
    //    public string lp_contact_number { get; set; }
    //    public int lp_delivery_charge { get; set; }
    //    public string lp_location { get; set; }
    //    public string delivery_duration { get; set; }
    //    public string recipient_center_name { get; set; }
    //    public string recipient_name { get; set; }
    //    public string recipient_mobile { get; set; }
    //    public string recipient_email { get; set; }
    //    public string recipient_division { get; set; }
    //    public string recipient_district { get; set; }
    //    public string recipient_upazila { get; set; }
    //    public string recipient_union { get; set; }
    //    public int payment_method { get; set; }
    //    public int total { get; set; }
    //    public EsPlaceOrderCartOptionsModel cart_options { get; set; }
    //}

    //public class EsPlaceOrderDetailsModel
    //{
    //    public string product_id { get; set; }
    //    public string product_name { get; set; }
    //    public decimal unit_price { get; set; }
    //    public int quantity { get; set; }
    //    public decimal price { get; set; }
    //    public string variation_id { get; set; }
    //    public string option { get; set; }
    //}

    //public class EsPlaceOrderCartOptionsModel
    //{
    //    public int cart_id { get; set; }
    //    public string other_required_data { get; set; }
    //}
}

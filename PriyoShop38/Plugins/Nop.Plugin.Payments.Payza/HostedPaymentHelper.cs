using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using Nop.Core.Configuration;
using Nop.Core.Domain.Orders;
using Nop.Services.Catalog;

namespace Nop.Plugin.Payments.Payza
{
    public class HostedPaymentHelper
    {

        #region Creating Collection

        public static NameValueCollection CreatingCollection(string merchantAccount,string gateWayUrl,string successUrl,
            string cancelUrl,Order order
            )
         {
            var collectionOfKey = new NameValueCollection();
            collectionOfKey.Add("ap_purchasetype", "item");
            collectionOfKey.Add("ap_merchant", merchantAccount);
            collectionOfKey.Add("ap_currency", order.CustomerCurrencyCode);
            collectionOfKey.Add("apc_1",order.Id.ToString());
            int count = 1;
            
            foreach (var orderItem in order.OrderItems)
            {
                var key = "ap_itemname_" + count;
                collectionOfKey.Add(key,orderItem.Product.Name);
                key = "ap_description_" + count;
                collectionOfKey.Add(key,orderItem.Product.ShortDescription);
                key = "ap_itemcode_" + count;
                collectionOfKey.Add(key,""+orderItem.Product.Id);
                key = "ap_quantity_" + count;
                collectionOfKey.Add(key, "" + orderItem.Quantity);
                key = "ap_amount_" + count;
                collectionOfKey.Add(key, "" + RoundingHelper.RoundPrice(orderItem.UnitPriceInclTax * order.CurrencyRate));
                count++;
            }
            
            if (order.OrderDiscount > 0)
            {
                collectionOfKey.Add("ap_discountamount", "" + RoundingHelper.RoundPrice(order.OrderDiscount * order.CurrencyRate));
            }
            if (order.OrderShippingInclTax > 0)
            {
                collectionOfKey.Add("ap_shippingcharges", "" + RoundingHelper.RoundPrice(order.OrderShippingInclTax * order.CurrencyRate));
            }
            //collectionOfKey.Add("ap_test", "1");
            collectionOfKey.Add("ap_returnurl", successUrl + "?orderid=" + order.Id);
            collectionOfKey.Add("ap_cancelurl", cancelUrl);
            return collectionOfKey;
        }
        #endregion Creating Collection

       
        
    }
}

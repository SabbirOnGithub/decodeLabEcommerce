using Nop.Core;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.SSLCommerzEmi.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Routing;
using Nop.Services.Localization;
using System.Text;
using System.Web;
using Nop.Services.Tax;
using Nop.Web.Framework;
using System.Net;
using System.IO;
using Nop.Services.Logging;
using Newtonsoft.Json;
using Nop.Core.Domain.Shipping;
using Nop.Services.Catalog;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;
using Nop.Services.Stores;
using Nop.Core.Domain.Customers;
using Nop.Core.Infrastructure;
using Nop.Services.Common;
using System.Collections.Specialized;
using Newtonsoft.Json.Linq;
using System.Linq;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.SSLCommerzEmi
{
    /// <summary>
    /// SSLCommerzEmi payment processor
    /// </summary>
    public class SSLCommerzEmiPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly SSLCommerzEmiPaymentSettings _SSLCommerzEmiPaymentSettings;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly CurrencySettings _currencySettings;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly HttpContextBase _httpContext;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly IWorkContext _workContext;
        private readonly TaxSettings _taxSettings;
        private readonly IStoreService _storeService;
        private readonly IStoreContext _storeContext;
        private readonly IVendorService _vendorService;
        #endregion

        #region Ctor

        public SSLCommerzEmiPaymentProcessor(SSLCommerzEmiPaymentSettings SSLCommerzEmiPaymentSettings,
            ISettingService settingService,
            IWebHelper webHelper,
            ICheckoutAttributeParser checkoutAttributeParser,
            ITaxService taxService,
            ICurrencyService currencyService,
            CurrencySettings currencySettings,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            HttpContextBase httpContext,
            ILogger logger,
            IProductService productService,
            IWorkContext workContext,
            TaxSettings taxSettings,
            IStoreService storeService,
            IStoreContext storeContext,
            IVendorService vendorService)
        {
            this._vendorService = vendorService;
            this._SSLCommerzEmiPaymentSettings = SSLCommerzEmiPaymentSettings;
            this._settingService = settingService;
            this._webHelper = webHelper;
            this._checkoutAttributeParser = checkoutAttributeParser;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._currencySettings = currencySettings;
            this._orderService = orderService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._httpContext = httpContext;
            this._logger = logger;
            this._productService = productService;
            this._workContext = workContext;
            this._taxSettings = taxSettings;
            this._storeService = storeService;
            this._storeContext = storeContext;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Gets SSLCommerzEmi URL
        /// </summary>
        /// <returns></returns>
        private string GetSSLCommerzEmiUrl()
        {
            //return _SSLCommerzEmiPaymentSettings.UseSandbox ? "https://securepay.SSLCommerz.com/gwprocess/testbox/v3/process.php" : "https://securepay.SSLCommerz.com/gwprocess/v3/process.php";
            return _SSLCommerzEmiPaymentSettings.UseSandbox ? " https://sandbox.SSLCommerz.com/gwprocess/v3/api.php" : "https://securepay.SSLCommerz.com/gwprocess/v3/api.php";
        }

        private string GetSSLCommerzEmiValidationUrl()
        {
            //return _SSLCommerzEmiPaymentSettings.UseSandbox ? "https://securepay.SSLCommerz.com/validator/api/testbox/validationserverAPI.php" : "https://securepay.SSLCommerz.com/validator/api/validationserverAPI.php";
            return _SSLCommerzEmiPaymentSettings.UseSandbox ? "https://sandbox.SSLCommerz.com/validator/api/validationserverAPI.php" : "https://securepay.SSLCommerz.com/validator/api/validationserverAPI.php";

        }

        private bool IsNaturalNumber(decimal amount)
        {
            decimal diff = Math.Abs(Math.Truncate(amount) - amount);
            return (diff < Convert.ToDecimal(0.0000001)) || (diff > Convert.ToDecimal(0.9999999));
        }

        public bool GetValidationDetails(string valId, out Dictionary<string, string> values)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("val_id={0}", valId);
            builder.AppendFormat("&Store_Id={0}", _SSLCommerzEmiPaymentSettings.StoreId);
            builder.AppendFormat("&Store_Passwd={0}", _SSLCommerzEmiPaymentSettings.StorePassword);
            builder.Append("&format=json");

            var url = GetSSLCommerzEmiValidationUrl() + "?" + builder.ToString();

            var req = (HttpWebRequest)WebRequest.Create(url);

            string response = string.Empty;

            using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
            {
                response = HttpUtility.UrlDecode(sr.ReadToEnd());
            }

            if (!String.IsNullOrEmpty(response))
            {
                values = JsonConvert.DeserializeObject<Dictionary<string, string>>(response);
                return true;
            }

            values = new Dictionary<string, string>();
            return false;
        }

        #endregion

        #region Methods

        // <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.NewPaymentStatus = PaymentStatus.Pending;
            return result;
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        //public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        //{
        //    var post = new RemotePost();

        //    post.FormName = "SSLCommerzEmiForm";
        //    post.Url = GetSSLCommerzEmiUrl();
        //    post.Method = "POST";

        //    var order = postProcessPaymentRequest.Order;

        //    post.Add("tran_id", postProcessPaymentRequest.Order.Id.ToString());
        //    //total amount
        //    var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
        //    post.Add("total_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));

        //    var storeLocation = _webHelper.GetStoreLocation(false);
        //    string successUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
        //    string cancelUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/CancelOrder";
        //    string failureUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
        //    post.Add("success_url", successUrl);
        //    post.Add("fail_url", failureUrl);
        //    post.Add("cancel_url", cancelUrl);

        //    post.Add("store_id", _SSLCommerzEmiPaymentSettings.StoreId);
        //    post.Add("store_passwd", _SSLCommerzEmiPaymentSettings.StorePassword);


        //    #region brainstation
        //    var isEnabledEmi = true;
        //    var allCartProducts = postProcessPaymentRequest.Order.OrderItems;
        //    var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
        //    var paymentInfoemioption = _workContext.CurrentCustomer.GetAttribute<string>(
        //            SystemCustomerAttributeNames.SelectedEmiOption,
        //            _genericAttributeService, _storeContext.CurrentStore.Id);
        //    //foreach (var item in allCartProducts)
        //    //{
        //    //    var productData = _productService.GetProductById(item.Product.Id);
        //    //    if (productData.EnableEmi)
        //    //    {
        //    //        isEnabledEmi = true;
        //    //        break;
        //    //    }
        //    //}
        //    if (isEnabledEmi)
        //    {
        //        post.Add("emi_option", "1");
        //        //post.Add("emi_max_inst_option", "9");
        //        if (!string.IsNullOrEmpty(paymentInfoemioption))
        //        {
        //            post.Add("emi_selected_inst", paymentInfoemioption);
        //        }
        //        else
        //        {
        //            post.Add("emi_selected_inst", "3");
        //        }

        //    };            
        //    #endregion

        //    if (_SSLCommerzEmiPaymentSettings.PassProductNamesAndTotals)
        //    {
        //        //get the items in the cart
        //        decimal cartTotal = decimal.Zero;
        //        var cartItems = postProcessPaymentRequest.Order.OrderItems;
        //        int x = 0;                

        //        foreach (var item in cartItems)
        //        {
        //            var unitPriceExclTax = item.UnitPriceExclTax;
        //            var priceExclTax = item.PriceExclTax;
        //            //round
        //            var unitPriceExclTaxRounded = Math.Round(unitPriceExclTax, 2);

        //            post.Add(String.Format("cart[{0}][product]", x), item.Product.Name + " (Quantity: " + item.Quantity + ")");
        //            post.Add(String.Format("cart[{0}][amount]", x), unitPriceExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
        //            x++;
        //            cartTotal += priceExclTax;                               
        //        }

        //        //the checkout attributes that have a dollar value and send them to SSLCommerzEmi as items to be paid for
        //        var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
        //        foreach (var val in attributeValues)
        //        {
        //            var attPrice = _taxService.GetCheckoutAttributePrice(val, false, postProcessPaymentRequest.Order.Customer);
        //            //round
        //            var attPriceRounded = Math.Round(attPrice, 2);
        //            if (attPrice > decimal.Zero) //if it has a price
        //            {
        //                var attribute = val.CheckoutAttribute;
        //                if (attribute != null)
        //                {
        //                    var attName = attribute.Name; //set the name
        //                    post.Add(String.Format("cart[{0}][product]", x), attName); //name
        //                    post.Add(String.Format("cart[{0}][amount]", x), attPriceRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
        //                    x++;
        //                    cartTotal += attPrice;
        //                }
        //            }
        //        }

        //        //shipping
        //        var orderShippingExclTax = postProcessPaymentRequest.Order.OrderShippingExclTax;
        //        var orderShippingExclTaxRounded = Math.Round(orderShippingExclTax, 2);
        //        if (orderShippingExclTax > decimal.Zero)
        //        {
        //            post.Add(String.Format("cart[{0}][product]", x), "Shipping fee");
        //            post.Add(String.Format("cart[{0}][amount]", x), orderShippingExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
        //            x++;
        //            cartTotal += orderShippingExclTax;
        //        }

        //        //payment method additional fee
        //        var paymentMethodAdditionalFeeExclTax = postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax;
        //        var paymentMethodAdditionalFeeExclTaxRounded = Math.Round(paymentMethodAdditionalFeeExclTax, 2);
        //        if (paymentMethodAdditionalFeeExclTax > decimal.Zero)
        //        {
        //            post.Add(String.Format("cart[{0}][product]", x), "Payment method fee");
        //            post.Add(String.Format("cart[{0}][amount]", x), paymentMethodAdditionalFeeExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
        //            x++;
        //            cartTotal += paymentMethodAdditionalFeeExclTax;
        //        }

        //        //tax
        //        var orderTax = postProcessPaymentRequest.Order.OrderTax;
        //        var orderTaxRounded = Math.Round(orderTax, 2);
        //        if (orderTax > decimal.Zero)
        //        {
        //            //post.Add("tax_1", orderTax.ToString("0.00", CultureInfo.InvariantCulture));

        //            //add tax as item
        //            post.Add(String.Format("cart[{0}][product]", x), "Sales Tax"); //name
        //            post.Add(String.Format("cart[{0}][amount]", x), orderTaxRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount

        //            cartTotal += orderTax;
        //            x++;
        //        }

        //        if (cartTotal > postProcessPaymentRequest.Order.OrderTotal)
        //        {
        //            /* Take the difference between what the order total is and what it should be and use that as the "discount".
        //             * The difference equals the amount of the gift card and/or reward points used. 
        //             */
        //            decimal discountTotal = cartTotal - postProcessPaymentRequest.Order.OrderTotal;
        //            discountTotal = Math.Round(discountTotal, 2);
        //            //gift card or rewared point amount applied to cart in nopCommerce - shows in SSLCommerzEmi as "Product"
        //            post.Add(String.Format("cart[{0}][product]", x), "Discount"); //name
        //            post.Add(String.Format("cart[{0}][amount]", x), discountTotal.ToString("-0.00", CultureInfo.InvariantCulture)); //amount
        //        }
        //    }

        //    if (!String.IsNullOrEmpty(_SSLCommerzEmiPaymentSettings.PrefferedCardTypes))
        //         post.Add("multi_card_name", _SSLCommerzEmiPaymentSettings.PrefferedCardTypes);            

        //    post.Add("cus_name", HttpUtility.UrlEncode(order.BillingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.BillingAddress.LastName));
        //    //post.Add("cus_email", order.BillingAddress.Email);
        //    post.Add("cus_add1", HttpUtility.UrlEncode(order.BillingAddress.Address1));
        //    post.Add("cus_add2", HttpUtility.UrlEncode(order.BillingAddress.Address2));
        //    post.Add("cus_city", HttpUtility.UrlEncode(order.BillingAddress.City));
        //    if (order.BillingAddress.StateProvince != null)
        //        post.Add("cus_state", HttpUtility.UrlEncode(order.BillingAddress.StateProvince.Name));
        //    else
        //        post.Add("cus_state", "");
        //    post.Add("cus_postcode", HttpUtility.UrlEncode(order.BillingAddress.ZipPostalCode));
        //    if (order.BillingAddress.Country != null)
        //        post.Add("cus_country", HttpUtility.UrlEncode(order.BillingAddress.Country.Name));
        //    else
        //        post.Add("cus_country", "");
        //    //post.Add("cus_phone", HttpUtility.UrlEncode(order.BillingAddress.PhoneNumber));
        //    post.Add("cus_fax", HttpUtility.UrlEncode(order.BillingAddress.FaxNumber));

        //    if (order.ShippingStatus != ShippingStatus.ShippingNotRequired && (order.BillingAddress.FirstName != order.ShippingAddress.FirstName || order.BillingAddress.LastName != order.ShippingAddress.LastName || order.BillingAddress.Address1 != order.ShippingAddress.Address1)) //check whether billing & shipping address are the same
        //    {
        //        post.Add("ship_name", HttpUtility.UrlEncode(order.ShippingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.ShippingAddress.LastName));
        //        post.Add("ship_add1", HttpUtility.UrlEncode(order.ShippingAddress.Address1));
        //        post.Add("ship_add2", HttpUtility.UrlEncode(order.ShippingAddress.Address2));
        //        post.Add("ship_city", HttpUtility.UrlEncode(order.ShippingAddress.City));
        //        if (order.ShippingAddress.StateProvince != null)
        //            post.Add("ship_state", HttpUtility.UrlEncode(order.ShippingAddress.StateProvince.Name));
        //        else
        //            post.Add("ship_state", "");
        //        post.Add("ship_postcode", HttpUtility.UrlEncode(order.ShippingAddress.ZipPostalCode));
        //        if (order.ShippingAddress.Country != null)
        //            post.Add("ship_country", HttpUtility.UrlEncode(order.ShippingAddress.Country.Name));
        //        else
        //            post.Add("ship_country", "");
        //    }

        //    //custom values
        //    post.Add("value_a", "Order GUID: " + order.OrderGuid);
        //    post.Add("value_b", "Order GUID: " + order.OrderGuid);
        //    post.Add("value_c", "Order GUID: " + order.OrderGuid);
        //    post.Add("value_d", "Order GUID: " + order.OrderGuid);

        //    post.Add("currency", order.CustomerCurrencyCode);



        //    post.Post();
        //}
        public void PostProcessMakePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var post = new NameValueCollection();
            var order = postProcessPaymentRequest.Order;

            post.Add("tran_id", postProcessPaymentRequest.Order.Id.ToString());
            //total amount
            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            post.Add("total_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));

            var storeLocation = _webHelper.GetStoreLocation(false);
            string successUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
            string cancelUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/CancelOrder";
            string failureUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
            post.Add("success_url", successUrl);
            post.Add("fail_url", failureUrl);
            post.Add("cancel_url", cancelUrl);

            post.Add("store_id", _SSLCommerzEmiPaymentSettings.StoreId);
            post.Add("store_passwd", _SSLCommerzEmiPaymentSettings.StorePassword);


            #region brainstation
            var isEnabledEmi = true;
            var allCartProducts = postProcessPaymentRequest.Order.OrderItems;
            var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var paymentInfoemioption = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedEmiOption,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

            if (isEnabledEmi)
            {
                post.Add("emi_option", "1");
                if (!string.IsNullOrEmpty(paymentInfoemioption))
                {
                    post.Add("emi_selected_inst", paymentInfoemioption);
                }
                else
                {
                    post.Add("emi_selected_inst", "3");
                }

            };
            #endregion

            if (_SSLCommerzEmiPaymentSettings.PassProductNamesAndTotals)
            {
                //get the items in the cart
                decimal cartTotal = decimal.Zero;
                var cartItems = postProcessPaymentRequest.Order.OrderItems;
                int x = 0;

                foreach (var item in cartItems)
                {
                    var unitPriceExclTax = item.UnitPriceExclTax;
                    var priceExclTax = item.PriceExclTax;
                    //round
                    var unitPriceExclTaxRounded = Math.Round(unitPriceExclTax, 2);

                    post.Add(String.Format("cart[{0}][product]", x), item.Product.Name + " (Quantity: " + item.Quantity + ")");
                    post.Add(String.Format("cart[{0}][amount]", x), unitPriceExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += priceExclTax;
                }

                //the checkout attributes that have a dollar value and send them to SSLCommerzEmi as items to be paid for
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
                foreach (var val in attributeValues)
                {
                    var attPrice = _taxService.GetCheckoutAttributePrice(val, false, postProcessPaymentRequest.Order.Customer);
                    //round
                    var attPriceRounded = Math.Round(attPrice, 2);
                    if (attPrice > decimal.Zero) //if it has a price
                    {
                        var attribute = val.CheckoutAttribute;
                        if (attribute != null)
                        {
                            var attName = attribute.Name; //set the name
                            post.Add(String.Format("cart[{0}][product]", x), attName); //name
                            post.Add(String.Format("cart[{0}][amount]", x), attPriceRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
                            x++;
                            cartTotal += attPrice;
                        }
                    }
                }

                //shipping
                var orderShippingExclTax = postProcessPaymentRequest.Order.OrderShippingExclTax;
                var orderShippingExclTaxRounded = Math.Round(orderShippingExclTax, 2);
                if (orderShippingExclTax > decimal.Zero)
                {
                    post.Add(String.Format("cart[{0}][product]", x), "Shipping fee");
                    post.Add(String.Format("cart[{0}][amount]", x), orderShippingExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += orderShippingExclTax;
                }

                //payment method additional fee
                var paymentMethodAdditionalFeeExclTax = postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax;
                var paymentMethodAdditionalFeeExclTaxRounded = Math.Round(paymentMethodAdditionalFeeExclTax, 2);
                if (paymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    post.Add(String.Format("cart[{0}][product]", x), "Payment method fee");
                    post.Add(String.Format("cart[{0}][amount]", x), paymentMethodAdditionalFeeExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += paymentMethodAdditionalFeeExclTax;
                }

                //tax
                var orderTax = postProcessPaymentRequest.Order.OrderTax;
                var orderTaxRounded = Math.Round(orderTax, 2);
                if (orderTax > decimal.Zero)
                {
                    //post.Add("tax_1", orderTax.ToString("0.00", CultureInfo.InvariantCulture));

                    //add tax as item
                    post.Add(String.Format("cart[{0}][product]", x), "Sales Tax"); //name
                    post.Add(String.Format("cart[{0}][amount]", x), orderTaxRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount

                    cartTotal += orderTax;
                    x++;
                }

                if (cartTotal > postProcessPaymentRequest.Order.OrderTotal)
                {
                    /* Take the difference between what the order total is and what it should be and use that as the "discount".
                     * The difference equals the amount of the gift card and/or reward points used. 
                     */
                    decimal discountTotal = cartTotal - postProcessPaymentRequest.Order.OrderTotal;
                    discountTotal = Math.Round(discountTotal, 2);
                    //gift card or rewared point amount applied to cart in nopCommerce - shows in SSLCommerzEmi as "Product"
                    post.Add(String.Format("cart[{0}][product]", x), "Discount"); //name
                    post.Add(String.Format("cart[{0}][amount]", x), discountTotal.ToString("-0.00", CultureInfo.InvariantCulture)); //amount
                }
            }

            if (!String.IsNullOrEmpty(_SSLCommerzEmiPaymentSettings.PrefferedCardTypes))
                post.Add("multi_card_name", _SSLCommerzEmiPaymentSettings.PrefferedCardTypes);







            post.Add("cus_name", HttpUtility.UrlEncode(order.BillingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.BillingAddress.LastName));
            post.Add("cus_email", string.IsNullOrEmpty(order.BillingAddress.Email) ? "" : order.BillingAddress.Email);
            post.Add("cus_add1", HttpUtility.UrlEncode(order.BillingAddress.Address1));
            post.Add("cus_add2", HttpUtility.UrlEncode(order.BillingAddress.Address2));
            post.Add("cus_city", HttpUtility.UrlEncode(order.BillingAddress.City));
            if (order.BillingAddress.StateProvince != null)
                post.Add("cus_state", HttpUtility.UrlEncode(order.BillingAddress.StateProvince.Name));
            else
                post.Add("cus_state", "");
            post.Add("cus_postcode", HttpUtility.UrlEncode(order.BillingAddress.ZipPostalCode));
            if (order.BillingAddress.Country != null)
                post.Add("cus_country", HttpUtility.UrlEncode(order.BillingAddress.Country.Name));
            else
                post.Add("cus_country", "");
            //post.Add("cus_phone", HttpUtility.UrlEncode(order.BillingAddress.PhoneNumber));
            post.Add("cus_fax", HttpUtility.UrlEncode(order.BillingAddress.FaxNumber));

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired && (order.BillingAddress.FirstName != order.ShippingAddress.FirstName || order.BillingAddress.LastName != order.ShippingAddress.LastName || order.BillingAddress.Address1 != order.ShippingAddress.Address1)) //check whether billing & shipping address are the same
            {
                post.Add("ship_name", HttpUtility.UrlEncode(order.ShippingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.ShippingAddress.LastName));
                post.Add("ship_add1", HttpUtility.UrlEncode(order.ShippingAddress.Address1));
                post.Add("ship_add2", HttpUtility.UrlEncode(order.ShippingAddress.Address2));
                post.Add("ship_city", HttpUtility.UrlEncode(order.ShippingAddress.City));
                if (order.ShippingAddress.StateProvince != null)
                    post.Add("ship_state", HttpUtility.UrlEncode(order.ShippingAddress.StateProvince.Name));
                else
                    post.Add("ship_state", "");
                post.Add("ship_postcode", HttpUtility.UrlEncode(order.ShippingAddress.ZipPostalCode));
                if (order.ShippingAddress.Country != null)
                    post.Add("ship_country", HttpUtility.UrlEncode(order.ShippingAddress.Country.Name));
                else
                    post.Add("ship_country", "");
            }


            post.Add("multi_card_name", "brac_visa,dbbl_visa,city_visa,ebl_visa,sbl_visa,brac_master,dbbl_master,city_master,ebl_master,sbl_master,city_amex");

            //custom values
            post.Add("value_a", "Order GUID: " + order.OrderGuid);
            post.Add("value_b", "Order GUID: " + order.OrderGuid);
            post.Add("value_c", "Order GUID: " + order.OrderGuid);
            post.Add("value_d", "Order GUID: " + order.OrderGuid);

            post.Add("currency", order.CustomerCurrencyCode);

            using (var client = new WebClient())
            {
                var response = client.UploadValues(GetSSLCommerzEmiUrl(), post);
                var responseJson = JObject.Parse(Encoding.UTF8.GetString(response));
                var responseDictionary = responseJson.ToObject<Dictionary<string, object>>();

                foreach (var item in responseDictionary)
                {
                    if (item.Key == "GatewayPageURL")
                    {
                        _httpContext.Response.Redirect(item.Value.ToString());
                    }
                }

            }
        }
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var post = new NameValueCollection();
            var order = postProcessPaymentRequest.Order;

            post.Add("tran_id", postProcessPaymentRequest.Order.Id.ToString());
            //total amount
            var orderTotal = Math.Round(postProcessPaymentRequest.Order.OrderTotal, 2);
            post.Add("total_amount", orderTotal.ToString("0.00", CultureInfo.InvariantCulture));

            var storeLocation = _webHelper.GetStoreLocation(false);
            string successUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
            string cancelUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/CancelOrder";
            string failureUrl = storeLocation + "Plugins/PaymentSSLCommerzEmi/PaymentResult";
            post.Add("success_url", successUrl);
            post.Add("fail_url", failureUrl);
            post.Add("cancel_url", cancelUrl);

            post.Add("store_id", _SSLCommerzEmiPaymentSettings.StoreId);
            post.Add("store_passwd", _SSLCommerzEmiPaymentSettings.StorePassword);


            #region brainstation
            var isEnabledEmi = true;
            var allCartProducts = postProcessPaymentRequest.Order.OrderItems;
            var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var paymentInfoemioption = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedEmiOption,
                    _genericAttributeService, _storeContext.CurrentStore.Id);

            if (isEnabledEmi)
            {
                post.Add("emi_option", "1");
                if (!string.IsNullOrEmpty(paymentInfoemioption))
                {
                    post.Add("emi_selected_inst", paymentInfoemioption);
                }
                else
                {
                    post.Add("emi_selected_inst", "3");
                }

            };
            #endregion

            if (_SSLCommerzEmiPaymentSettings.PassProductNamesAndTotals)
            {
                //get the items in the cart
                decimal cartTotal = decimal.Zero;
                var cartItems = postProcessPaymentRequest.Order.OrderItems;
                int x = 0;

                foreach (var item in cartItems)
                {
                    var unitPriceExclTax = item.UnitPriceExclTax;
                    var priceExclTax = item.PriceExclTax;
                    //round
                    var unitPriceExclTaxRounded = Math.Round(unitPriceExclTax, 2);

                    post.Add(String.Format("cart[{0}][product]", x), item.Product.Name + " (Quantity: " + item.Quantity + ")");
                    post.Add(String.Format("cart[{0}][amount]", x), unitPriceExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += priceExclTax;
                }

                //the checkout attributes that have a dollar value and send them to SSLCommerzEmi as items to be paid for
                var attributeValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(postProcessPaymentRequest.Order.CheckoutAttributesXml);
                foreach (var val in attributeValues)
                {
                    var attPrice = _taxService.GetCheckoutAttributePrice(val, false, postProcessPaymentRequest.Order.Customer);
                    //round
                    var attPriceRounded = Math.Round(attPrice, 2);
                    if (attPrice > decimal.Zero) //if it has a price
                    {
                        var attribute = val.CheckoutAttribute;
                        if (attribute != null)
                        {
                            var attName = attribute.Name; //set the name
                            post.Add(String.Format("cart[{0}][product]", x), attName); //name
                            post.Add(String.Format("cart[{0}][amount]", x), attPriceRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount
                            x++;
                            cartTotal += attPrice;
                        }
                    }
                }

                //shipping
                var orderShippingExclTax = postProcessPaymentRequest.Order.OrderShippingExclTax;
                var orderShippingExclTaxRounded = Math.Round(orderShippingExclTax, 2);
                if (orderShippingExclTax > decimal.Zero)
                {
                    post.Add(String.Format("cart[{0}][product]", x), "Shipping fee");
                    post.Add(String.Format("cart[{0}][amount]", x), orderShippingExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += orderShippingExclTax;
                }

                //payment method additional fee
                var paymentMethodAdditionalFeeExclTax = postProcessPaymentRequest.Order.PaymentMethodAdditionalFeeExclTax;
                var paymentMethodAdditionalFeeExclTaxRounded = Math.Round(paymentMethodAdditionalFeeExclTax, 2);
                if (paymentMethodAdditionalFeeExclTax > decimal.Zero)
                {
                    post.Add(String.Format("cart[{0}][product]", x), "Payment method fee");
                    post.Add(String.Format("cart[{0}][amount]", x), paymentMethodAdditionalFeeExclTaxRounded.ToString("0.00", CultureInfo.InvariantCulture));
                    x++;
                    cartTotal += paymentMethodAdditionalFeeExclTax;
                }

                //tax
                var orderTax = postProcessPaymentRequest.Order.OrderTax;
                var orderTaxRounded = Math.Round(orderTax, 2);
                if (orderTax > decimal.Zero)
                {
                    //post.Add("tax_1", orderTax.ToString("0.00", CultureInfo.InvariantCulture));

                    //add tax as item
                    post.Add(String.Format("cart[{0}][product]", x), "Sales Tax"); //name
                    post.Add(String.Format("cart[{0}][amount]", x), orderTaxRounded.ToString("0.00", CultureInfo.InvariantCulture)); //amount

                    cartTotal += orderTax;
                    x++;
                }

                if (cartTotal > postProcessPaymentRequest.Order.OrderTotal)
                {
                    /* Take the difference between what the order total is and what it should be and use that as the "discount".
                     * The difference equals the amount of the gift card and/or reward points used. 
                     */
                    decimal discountTotal = cartTotal - postProcessPaymentRequest.Order.OrderTotal;
                    discountTotal = Math.Round(discountTotal, 2);
                    //gift card or rewared point amount applied to cart in nopCommerce - shows in SSLCommerzEmi as "Product"
                    post.Add(String.Format("cart[{0}][product]", x), "Discount"); //name
                    post.Add(String.Format("cart[{0}][amount]", x), discountTotal.ToString("-0.00", CultureInfo.InvariantCulture)); //amount
                }
            }

            if (!String.IsNullOrEmpty(_SSLCommerzEmiPaymentSettings.PrefferedCardTypes))
                post.Add("multi_card_name", _SSLCommerzEmiPaymentSettings.PrefferedCardTypes);



        



            post.Add("cus_name", HttpUtility.UrlEncode(order.BillingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.BillingAddress.LastName));
            post.Add("cus_email", string.IsNullOrEmpty(order.BillingAddress.Email) ? "" : order.BillingAddress.Email);
            post.Add("cus_add1", HttpUtility.UrlEncode(order.BillingAddress.Address1));
            post.Add("cus_add2", HttpUtility.UrlEncode(order.BillingAddress.Address2));
            post.Add("cus_city", HttpUtility.UrlEncode(order.BillingAddress.City));
            if (order.BillingAddress.StateProvince != null)
                post.Add("cus_state", HttpUtility.UrlEncode(order.BillingAddress.StateProvince.Name));
            else
                post.Add("cus_state", "");
            post.Add("cus_postcode", HttpUtility.UrlEncode(order.BillingAddress.ZipPostalCode));
            if (order.BillingAddress.Country != null)
                post.Add("cus_country", HttpUtility.UrlEncode(order.BillingAddress.Country.Name));
            else
                post.Add("cus_country", "");
            //post.Add("cus_phone", HttpUtility.UrlEncode(order.BillingAddress.PhoneNumber));
            post.Add("cus_fax", HttpUtility.UrlEncode(order.BillingAddress.FaxNumber));

            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired && (order.BillingAddress.FirstName != order.ShippingAddress.FirstName || order.BillingAddress.LastName != order.ShippingAddress.LastName || order.BillingAddress.Address1 != order.ShippingAddress.Address1)) //check whether billing & shipping address are the same
            {
                post.Add("ship_name", HttpUtility.UrlEncode(order.ShippingAddress.FirstName) + " " + HttpUtility.UrlEncode(order.ShippingAddress.LastName));
                post.Add("ship_add1", HttpUtility.UrlEncode(order.ShippingAddress.Address1));
                post.Add("ship_add2", HttpUtility.UrlEncode(order.ShippingAddress.Address2));
                post.Add("ship_city", HttpUtility.UrlEncode(order.ShippingAddress.City));
                if (order.ShippingAddress.StateProvince != null)
                    post.Add("ship_state", HttpUtility.UrlEncode(order.ShippingAddress.StateProvince.Name));
                else
                    post.Add("ship_state", "");
                post.Add("ship_postcode", HttpUtility.UrlEncode(order.ShippingAddress.ZipPostalCode));
                if (order.ShippingAddress.Country != null)
                    post.Add("ship_country", HttpUtility.UrlEncode(order.ShippingAddress.Country.Name));
                else
                    post.Add("ship_country", "");
            }


            post.Add("multi_card_name", "brac_visa,dbbl_visa,city_visa,ebl_visa,sbl_visa,brac_master,dbbl_master,city_master,ebl_master,sbl_master,city_amex");

            //custom values
            post.Add("value_a", "Order GUID: " + order.OrderGuid);
            post.Add("value_b", "Order GUID: " + order.OrderGuid);
            post.Add("value_c", "Order GUID: " + order.OrderGuid);
            post.Add("value_d", "Order GUID: " + order.OrderGuid);

            post.Add("currency", order.CustomerCurrencyCode);

            using (var client = new WebClient())
            {
                var response = client.UploadValues(GetSSLCommerzEmiUrl(), post);
                var responseJson = JObject.Parse(Encoding.UTF8.GetString(response));
                var responseDictionary = responseJson.ToObject<Dictionary<string, object>>();

                foreach (var item in responseDictionary)
                {
                    if (item.Key == "GatewayPageURL")
                    {
                        _httpContext.Response.Redirect(item.Value.ToString());
                    }
                }

            }
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            foreach (var item in cart)
            {
                if (item.Product.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, this.PluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)))
                    return true;

                var vendor = _vendorService.GetVendorById(item.Product.VendorId);
                if (vendor != null)
                {
                    if (vendor.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, this.PluginDescriptor.SystemName, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }
            }
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country

            //subtotal
            decimal orderSubTotalDiscountAmountBase;
            List<Discount> orderSubTotalAppliedDiscounts;
            decimal subTotalWithoutDiscountBase;
            decimal subTotalWithDiscountBase;
            var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
            decimal subtotalBase = subTotalWithoutDiscountBase;
            decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);

            //var SSLCommerzEmiPaymentSettings = _settingService.LoadSetting<SSLCommerzEmiPaymentSettings>(_storeContext.CurrentStore.Id);
            if (subtotal < _SSLCommerzEmiPaymentSettings.HidePaymentMethodForAmountLessThan)
                return true;

            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            var additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFee;
            var _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            var paymentInfoemioption = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedEmiOption,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
            if (paymentInfoemioption == "3")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeThreeMonthOption;
            }
            else if (paymentInfoemioption == "6")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeSixMonthOption;
            }
            else if (paymentInfoemioption == "9")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeNineMonthOption;
            }
            else if (paymentInfoemioption == "12")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeTwelveMonthOption;
            }
            else if (paymentInfoemioption == "18")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeEighteenMonthOption;
            }
            else if (paymentInfoemioption == "24")
            {
                additionalfeeemi = _SSLCommerzEmiPaymentSettings.AdditionalFeeTwentyFourMonthOption;
            }


            var result = this.CalculateAdditionalFee(_orderTotalCalculationService, cart,
                additionalfeeemi, _SSLCommerzEmiPaymentSettings.AdditionalFeePercentage);
            return result;
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            if (order.OrderStatus == OrderStatus.Pending)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentSSLCommerzEmi";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.SSLCommerzEmi.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for payment info
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentSSLCommerzEmi";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.SSLCommerzEmi.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentSSLCommerzEmiController);
        }

        public override void Install()
        {
            //settings
            var settings = new SSLCommerzEmiPaymentSettings()
            {
                UseSandbox = true,
                StoreId = "testbox",
                StorePassword = "qwerty",
                AdditionalFee = 0,
                EnableIpn = true,
            };
            _settingService.SaveSetting(settings);

            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.HidePaymentMethodForAmountLessThan", "Hide this Paymentmethod for Amount less than");
            //locales
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.RedirectionTip", "You will be redirected to SSLCommerzEmi site to complete the order.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.UseSandbox", "Use Test Mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.UseSandbox.Hint", "Mark if you want to test the gateway");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StoreId", "Store ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StoreId.Hint", "Enter Store ID");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StorePassword", "Store Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StorePassword.Hint", "Store Password (Supplied by SSLCommerzEmi)");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PassProductNamesAndTotals", "Pass product names and order totals to SSLCommerzEmi");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PassProductNamesAndTotals.Hint", "Check if product names and order totals should be passed to SSLCommerzEmi.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PrefferedCardTypes", "Card Types");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PrefferedCardTypes.Hint", "Select your preffered banks' Payment Gateway as default choice on the SSLCommerzEmi Gateway Page.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.EnableIpn", "Enable IPN");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.EnableIpn.Hint", "Enable IPN");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFee", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFee.Hint", "Additional fee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeePercentage", "Additional fee. Use percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeePercentage.Hint", "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage", "Return to order details page");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage.Hint", "Enable if a customer should be redirected to the order details page when he clicks \"Cancel Transaction\" link on SSLCommerzEmi site WITHOUT completing a payment");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.PaymentErrorMessage", "There was a problem with your payment. Please click details below and then try to complete the payment again.");

            base.Install();
        }

        public override void Uninstall()
        {
            //locales
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.HidePaymentMethodForAmountLessThan");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.RedirectionTip");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.UseSandbox");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.UseSandbox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StoreId");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StoreId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StorePassword");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.StorePassword.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PassProductNamesAndTotals");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PassProductNamesAndTotals.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PrefferedCardTypes");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.PrefferedCardTypes.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.EnableIpn");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.EnableIpn.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.Fields.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.SSLCommerzEmi.PaymentErrorMessage");

            base.Uninstall();
        }

        #endregion

        #region Properies

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return false;
            }
        }

        #endregion
    }
}

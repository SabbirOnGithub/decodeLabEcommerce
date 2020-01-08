using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Routing;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.EblSkyPay.Controllers;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Payments;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Net.Security;
using Nop.Plugin.Payments.EblSkyPay.Models;
using Nop.Plugin.Payments.EblSkyPay.Services;
using Nop.Services.Orders;
using System.Collections.Specialized;
using Nop.Web.Framework.Menu;
using System.Linq;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.EblSkyPay
{
    /// <summary>
    /// EblSkyPay payment processor
    /// </summary>
    public class EblSkyPayPaymentProcessor : BasePlugin, IPaymentMethod, IAdminMenuPlugin
    {
        #region Fields

        private readonly HttpContextBase _httpContext;
        private readonly IWebHelper _webHelper;
        private readonly ISettingService _settingService;
        private readonly EblSkyPayPaymentSettings _eblSkyPaySettings;
        private readonly ILogger _logService;
        private readonly IStoreContext _storeContext;
        private readonly ISkyPayService _skyPayService;
        private readonly IOrderService _orderService;
        private readonly IVendorService _vendorService;
        #endregion

        #region Ctor

        public EblSkyPayPaymentProcessor(IWebHelper webHelper,
            HttpContextBase httpContext,
            ISettingService settingService,
            EblSkyPayPaymentSettings eblSkyPayPaymentSettings,
            ILogger logService,
            IOrderService orderService,
            ISkyPayService skyPayService,
            IStoreContext storeCotnext,
            IVendorService vendorService)
        {
            _webHelper = webHelper;
            _httpContext = httpContext;
            _settingService = settingService;
            _eblSkyPaySettings = eblSkyPayPaymentSettings;
            _logService = logService;
            _skyPayService = skyPayService;
            _storeContext = storeCotnext;
            _orderService = orderService;
            _vendorService = vendorService;
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (_eblSkyPaySettings.EnableLogging)
            {
                _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.PostProcessing.Start", $" Order Id : {postProcessPaymentRequest.Order.Id}, Uses Http : {_eblSkyPaySettings.UseHttsForRedirection}");
            }

            string formUrl;
            if (_eblSkyPaySettings.UseHttsForRedirection)
                formUrl = string.Format("https://www.othoba.com/Plugins/PaymentEblSkyPay/EblSkyPay?orderId={0}", postProcessPaymentRequest.Order.Id);
            else
                formUrl = string.Format("http://www.othoba.com/Plugins/PaymentEblSkyPay/EblSkyPay?orderId={0}", postProcessPaymentRequest.Order.Id);

            _httpContext.Response.Redirect(formUrl);
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return decimal.Zero;
        }

        #endregion

        #region Methods

        /// <summary>
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

        public bool Authenticate()
        {
            return true;
        }


        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <returns>Additional handling fee</returns>
        public decimal GetAdditionalHandlingFee()
        {
            return 0;//_EblSkyPayPaymentSettings.AdditionalFee;
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

            //payment status should be Pending
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;

            //let's ensure that at least 1 minute passed after order is placed
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return false;

            return true;
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
            controllerName = "PaymentEblSkyPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.EblSkyPay.Controllers" }, { "area", null } };
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
            controllerName = "PaymentEblSkyPay";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.EblSkyPay.Controllers" }, { "area", null } };
            //GetIndexRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        }

        public Type GetControllerType()
        {
            return typeof(PaymentEblSkyPayController);
        }

        public override void Install()
        {
            var settings = new EblSkyPayPaymentSettings()
            {
                DescriptionText = "<p>You can edit this text from admin panel.</p>",
                AdditionalFee = 0,
                AdditionalFeePercentage = false,
                EnableLogging = true,
                MerchantId = "TEST20070005",
                MerchantName = "Priyoshop.com",
                OperatorId = "Priyoshop",
                Password = "Abc12345@",
                UseSandbox = true,
                AutoRedirectEnable = false,
                UseSandBox = false,
                UseProxy = true
            };
            _settingService.SaveSetting(settings);

            this.AddOrUpdatePluginLocaleResource("Payments.EblSkyPay", "EBL Sky Pay");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.DescriptionText", "Description");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.DescriptionText.Hint", "Put description here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFee", "AdditionalFee");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFee.Hint", "Put AdditionalFee here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFeePercentage", "Additional fee Percentage");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFeePercentage.Hint", "Put percentage here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.EnableLogging", "Enable Logging");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.EnableLogging.Hint", "Check to store transaction log");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AutoRedirectEnable", "Auto Redirect to EBL site Enable");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.AutoRedirectEnable.Hint", "Click to enable AutoRedirect");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantId", "MerchantId");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantId.Hint", "Put MerchantId here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantName", "Merchant Name");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantName.Hint", "Put MerchantName here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.OperatorId", "Operator Id");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.OperatorId.Hint", "Put Operator Id here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.Password.Hint", "Put Password here");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.UseSandBox", "UseSandBox");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.UseSandBox.Hint", "Check to Enable SandBox Mode");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.UseHttsForRedirection", "Use Htts For Redirection");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.UseHttsForRedirection.Hint", "Use Htts For Redirection");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.GatewayUrl", "Gateway Url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payment.EblSkyPay.GatewayUrl.Hint", "Gateway Url");

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<EblSkyPayPaymentSettings>();

            //locales
            this.DeletePluginLocaleResource("Payments.EblSkyPay");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.DescriptionText");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.DescriptionText.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFee");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFee.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFeePercentage");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AdditionalFeePercentage.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.EnableLogging");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.EnableLogging.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AutoRedirectEnable");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.AutoRedirectEnable.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantId");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantName");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.MerchantName.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.OperatorId");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.OperatorId.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.Password");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.UseSandBox");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.UseSandBox.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.UseHttsForRedirection");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.UseHttsForRedirection.Hint");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.GatewayUrl");
            this.DeletePluginLocaleResource("Plugins.Payment.EblSkyPay.GatewayUrl.Hint");

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
        /// Skip Payment info
        /// </summary>
        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        public bool EnableDiscount
        {
            get
            {
                return false;
            }
        }
        public decimal GetDiscountAmount()
        {
            return decimal.Zero;
        }

        #endregion

        #region EBL Transaction

        /// <summary>
        /// Process transaction (EBL)
        /// </summary>
        /// <param name="order"></param>
        /// <param name="requestAndSaveSession"></param>
        /// <returns></returns>
        public EblSkyPayConfigurationModel ProcessTransaction(Order order, bool requestAndSaveSession = true)
        {
            var model = new EblSkyPayConfigurationModel();

            //order information
            model.OrderId = order.Id;
            model.Amount = order.OrderTotal;
            model.Currency = "BDT";
            model.Address1 = $"{order.Customer.ShippingAddress.FirstName}, {order.Customer.ShippingAddress.PhoneNumber} ";
            model.Address2 = $"{order.Customer.ShippingAddress.Address1}, {order.Customer.ShippingAddress.Address2}, {order.Customer.ShippingAddress.City}";
            var productDescription = string.Empty;
            foreach (var item in order.OrderItems)
                model.Description += item.Product.Name + ',';

            //128 char max support by EBL so lets concat it
            model.Description = model.Description.Substring(0, Math.Min(model.Description.Length, 100));

            model.DisplayControl = "HIDE";

            //settings
            model.MerchantId = _eblSkyPaySettings.MerchantId;
            model.MerchantName = _eblSkyPaySettings.MerchantName;
            model.UseSandBox = _eblSkyPaySettings.UseSandbox;
            model.AutoRedirectEnable = _eblSkyPaySettings.AutoRedirectEnable;

            //log
            if (_eblSkyPaySettings.EnableLogging)
            {
                _logService.InsertLog(LogLevel.Debug, "EBLSkyPay.Initiated", $"Email : {order.ShippingAddress.Email}");
            }

            model.SuccessUrl = $"{_storeContext.CurrentStore.Url}PaymentEblSkyPay/SuccessOrder?orderId={model.OrderId}";
            model.CancelUrl = $"{_storeContext.CurrentStore.Url}PaymentEblSkyPay/CancelOrder?orderId={model.OrderId}";
            model.FailUrl = $"{_storeContext.CurrentStore.Url}PaymentEblSkyPay/FailOrder?orderId={model.OrderId}";
            model.ErrorCallbackUrl = $"{_storeContext.CurrentStore.Url}PaymentEblSkyPay/ErrorCallback?orderId={model.OrderId}";

            //request for session id and save it to db[eblskypay]
            if (requestAndSaveSession)
            {
                // [Snippet] addMerchantInfo -- start
                StringBuilder data = new StringBuilder();
                data.Append("merchant=" + _eblSkyPaySettings.MerchantId);
                data.Append("&apiUsername=merchant." + _eblSkyPaySettings.MerchantId);
                data.Append("&apiPassword=" + _eblSkyPaySettings.Password);
                data.Append("&apiOperation=CREATE_CHECKOUT_SESSION");
                // [Snippet] addMerchantInfo -- end

                // [Snippet] addOrderInfo-- start
                data.Append("&order.amount=" + order.OrderTotal);
                data.Append("&order.id=" + order.Id);
                data.Append("&order.description=" + model.Description);
                data.Append("&order.currency=BDT");
                data.Append("&interaction.cancelUrl=" + model.CancelUrl);
                data.Append("&interaction.returnUrl=" + model.SuccessUrl);
                data.Append("&interaction.merchant.name=" + _eblSkyPaySettings.MerchantName);
                data.Append("&interaction.merchant.logo=https://priyoshop.com/content/images/thumbs/0072813.png");
                data.Append("&interaction.displayControl.billingAddress=HIDE");
                data.Append("&interaction.displayControl.orderSummary=HIDE");
                // [Snippet] addOrderInfo-- end

                //sent request to EBL
                var response = SendTransaction(data.ToString());

                // [Snippet] howToParseResponse - start
                NameValueCollection responseValues = new NameValueCollection();
                if (response != null && response.Length > 0)
                {
                    string[] responses = response.Split('&');
                    foreach (string responseField in responses)
                    {
                        string[] field = responseField.Split('=');
                        responseValues.Add(HttpUtility.UrlDecode(field[0]), HttpUtility.UrlDecode(field[1]));
                    }
                }
                // [Snippet] howToParseResponse - end

                //parse request and save it to DB
                var entity = new Domain.EblSkyPay();

                //result
                entity.Result = responseValues["result"];
                if (entity.Result.Equals("ERROR"))
                {
                    if (_eblSkyPaySettings.EnableLogging)
                    {
                        _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.CreateOrder.Error", response.ToString());
                    }
                    model.ErrorInCreateOrder = true;
                    entity.Merchant = "ERROR";
                    entity.SessionId = "ERROR";
                    entity.SessionUpdateStatus = "ERROR";
                    entity.SessionVersion = "ERROR";
                    entity.SuccessIndicator = "ERROR";
                    entity.Response = response.ToString();
                }
                else
                {
                    entity.Merchant = responseValues["merchant"];
                    entity.SessionId = responseValues["session.id"];
                    entity.SessionUpdateStatus = responseValues["session.updateStatus"];
                    entity.SessionVersion = responseValues["session.version"];
                    entity.SuccessIndicator = responseValues["successIndicator"];
                }

                entity.OrderId = order.Id;
                entity.CreatedOnUtc = DateTime.UtcNow;
                entity.Active = true;
                entity.PaymentStatusId = (int)PaymentStatus.Pending;

                //set others session status to inactive
                _skyPayService.SetAsInActiveByOrderId(model.OrderId);
                _skyPayService.Insert(entity);

                model.SessionId = entity.SessionId;
            }

            return model;
        }

        /// <summary>
        /// Check Transaction (paid or not)
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public EblSkyPayModel CheckTransaction(Order order, out bool success)
        {
            string result = null;
            string response = null;

            // [Snippet] howToConvertFormData -- start
            StringBuilder data = new StringBuilder();
            data.Append("merchant=" + _eblSkyPaySettings.MerchantId);
            data.Append("&apiUsername=merchant." + _eblSkyPaySettings.MerchantId);
            data.Append("&apiPassword=" + _eblSkyPaySettings.Password);

            data.Append("&apiOperation=RETRIEVE_ORDER");

            data.Append("&order.id=" + order.Id.ToString());

            if (_eblSkyPaySettings.EnableLogging)
            {
                _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.Sent.Order.NVP", data.ToString());
            }

            response = SendTransaction(data.ToString());

            if (_eblSkyPaySettings.EnableLogging)
            {
                _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.Retrive.Order.NVP", response.ToString());
            }

            // [Snippet] howToParseResponse - start
            NameValueCollection respValues = new NameValueCollection();
            if (response != null && response.Length > 0)
            {
                string[] responses = response.Split('&');
                foreach (string responseField in responses)
                {
                    string[] field = responseField.Split('=');
                    respValues.Add(HttpUtility.UrlDecode(field[0]), HttpUtility.UrlDecode(field[1]));
                }
            }
            // [Snippet] howToParseResponse - end

            result = respValues["result"];

            //error string if error is triggered
            if (result != null && result.Equals("ERROR"))
            {
                String errorMessage = null;
                String errorCode = null;

                String failureExplanations = respValues["explanation"];
                String supportCode = respValues["supportCode"];

                if (failureExplanations != null)
                {
                    errorMessage = failureExplanations;
                }
                else if (supportCode != null)
                {
                    errorMessage = supportCode;
                }
                else
                {
                    errorMessage = "Reason unspecified.";
                }

                String failureCode = respValues["failureCode"];
                if (failureCode != null)
                {
                    errorCode = "Error (" + failureCode + ")";
                }
                else
                {
                    errorCode = "Error (UNSPECIFIED)";
                }

                // add to server and redirect to error page
                if (_eblSkyPaySettings.EnableLogging)
                {
                    _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.Retrive.Order.NVP.ParseError: ErrorCode : " + errorCode, errorMessage);
                }

                //redirect to Error Page
                success = false;
                return new EblSkyPayModel();
            }

            //status
            success = true;

            //parse info
            var entity = new EblSkyPayModel();
            entity.OrderRetriveResponse = response;
            entity.Result = respValues["result"];
            entity.OrderId = Convert.ToInt32(respValues["transaction[0].order.id"]);
            entity.TrxDate = respValues["transaction[0].timeOfRecord"];
            entity.Amount = Convert.ToDecimal(respValues["amount"]);
            entity.Status = respValues["status"];
            entity.TrxCurrency = respValues["transaction[0].transaction.currency"];

            return entity;
        }

        #endregion

        #region Main Transaction Maker (v2 from EBL SkyPay gateway authority)

        /// <summary>
        /// Sent a trx request to EBL Gateway
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SendTransaction(string data)
        {
            string body = String.Empty;

            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            // code to validate certificat in an SSL conversation
            ServicePointManager.ServerCertificateValidationCallback += delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                if (_eblSkyPaySettings.IgnoreSslErrors)
                {
                    // allow any old dodgy certificate...
                    return true;
                }
                else
                {
                    return sslPolicyErrors == System.Net.Security.SslPolicyErrors.None;
                }
            };

           // _eblSkyPaySettings.UseProxy = true;

            // [Snippet] howToConfigureProxy - start
            //if (_eblSkyPaySettings.UseProxy)
            //{
            //    WebProxy proxy = new WebProxy(_eblSkyPaySettings.ProxyHost, true);
            //    if (!String.IsNullOrEmpty(_eblSkyPaySettings.ProxyUser))
            //    {
            //        if (String.IsNullOrEmpty(_eblSkyPaySettings.ProxyDomain))
            //        {
            //            proxy.Credentials = new NetworkCredential(_eblSkyPaySettings.ProxyUser, _eblSkyPaySettings.ProxyPassword);
            //        }
            //        else
            //        {
            //            proxy.Credentials = new NetworkCredential(_eblSkyPaySettings.ProxyUser, _eblSkyPaySettings.ProxyPassword, _eblSkyPaySettings.ProxyDomain);
            //        }
            //    }
            //    WebRequest.DefaultWebProxy = proxy;
            //}
            // [Snippet] howToConfigureProxy - end

            // [Snippet] howToSetURL - start
            // Create the web request
            HttpWebRequest request = WebRequest.Create(_eblSkyPaySettings.GatewayUrl) as HttpWebRequest;
            // [Snippet] howToSetURL - end

            // [Snippet] howToPut - start
            // Set type to PUT
            request.Method = "POST";
            // [Snippet] howToPut - end

            // [Snippet] howToSetHeaders - start
            request.ContentType = "application/x-www-form-urlencoded; charset=iso-8859-1";
            // [Snippet] howToSetHeaders - end

            //request.AuthenticationLevel = System.Net.Security.AuthenticationLevel.None;
            //request.Credentials = new NetworkCredential("", apiPassword);
            //request.PreAuthenticate = true;

            // [Snippet] howToSetCredentials - start
            string credentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(_eblSkyPaySettings.MerchantId + ":" + _eblSkyPaySettings.Password));
            request.Headers.Add("Authorization", "Basic " + credentials);
            // [Snippet] howToSetCredentials - end

            // Create a byte array of the data we want to send
            byte[] utf8bytes = Encoding.UTF8.GetBytes(data);
            byte[] iso8859bytes = Encoding.Convert(Encoding.UTF8, Encoding.GetEncoding("iso-8859-1"), utf8bytes);

            // Set the content length in the request headers
            request.ContentLength = iso8859bytes.Length;

            // Ignore format error checks before sending body
            request.ServicePoint.Expect100Continue = false;

            try
            {
                // [Snippet] executeSendTransaction - start
                // Write data
                using (Stream postStream = request.GetRequestStream())
                {
                    postStream.Write(iso8859bytes, 0, iso8859bytes.Length);
                }
                // [Snippet] executeSendTransaction - end

                // Get response
                try
                {
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        // Get the response stream
                        StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                        body = reader.ReadToEnd();
                    }
                }
                catch (WebException wex)
                {
                    StreamReader reader = new StreamReader(wex.Response.GetResponseStream(), Encoding.GetEncoding("iso-8859-1"));
                    body = reader.ReadToEnd();
                }
                return body;
            }
            catch (Exception ex)
            {
                var msg = ex.Message + "\n\naddress:\n" + request.Address.ToString() + "\n\nheader:\n" + request.Headers.ToString() + "data submitted:\n" + data;

                _logService.InsertLog(LogLevel.Debug, "EBLSkypay.Error.SentTransaction", msg);

                return msg;
            }
        }

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
            return false;
        }

        public void ManageSiteMap(Web.Framework.Menu.SiteMapNode rootNode)
        {
            var node = new Web.Framework.Menu.SiteMapNode()
            {
                Title = "EBL SkyPay",
                Visible = true,
                IconClass = "fa-bars"
            };

            var childNode1 = new Web.Framework.Menu.SiteMapNode()
            {
                Title = "Manage",
                Visible = true,
                Url = "/PaymentEblSkyPay/Manage",
                IconClass = "fa-dot-circle-o"
            };

            var childNode2 = new Web.Framework.Menu.SiteMapNode()
            {
                Title = "Settings",
                Visible = true,
                Url = "/Admin/Payment/ConfigureMethod?systemName=Payments.EblSkyPay",
                IconClass = "fa-dot-circle-o"
            };
            node.ChildNodes.Add(childNode1);
            node.ChildNodes.Add(childNode2);
            rootNode.ChildNodes.Add(node);
        }

        #endregion
    }
}
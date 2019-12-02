using Newtonsoft.Json;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.bKashAdvance.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Nop.Plugin.Payments.bKashAdvance.Services
{
    public class BkashAdvanceService : IBkashAdvanceService
    {
        #region URLs
        private static string createTokenSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/token/grant";
        private static string createTokenLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/token/grant";

        private static string createPaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/create";
        private static string createPaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/create";

        private static string executePaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/execute";
        private static string executePaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/execute";

        private static string capturePaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/capture";
        private static string capturePaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta0/checkout/payment/capture";

        private static string refundPaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/refund";
        private static string refundPaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/refund";

        private static string voidPaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/void";
        private static string voidPaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/void";

        private static string searchTransactionSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/search";
        private static string searchTransactionLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/search";

        private static string organizationBalanceSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/organizationBalance";
        private static string organizationBalanceLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/organizationBalance";

        private static string queryPaymentSandboxUrl = "https://checkout.sandbox.bka.sh/v0.40.0/checkout/payment/query";
        private static string queryPaymentLiveUrl = "https://checkout.pay.bka.sh/v1.0.0-beta/checkout/payment/query";
        #endregion

        #region Fields
        private readonly BkashAdvancePaymentSettings _bkashAdvancePaymentSettings;
        private readonly IOrderService _orderService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly HttpContextBase _httpContext;
        #endregion

        #region CTor
        public BkashAdvanceService(BkashAdvancePaymentSettings bkashAdvancePaymentSettings,
            IOrderService orderService,
            ISettingService settingService,
            ILogger logger,
            HttpContextBase httpContext)
        {
            _orderService = orderService;
            _logger = logger;
            _bkashAdvancePaymentSettings = bkashAdvancePaymentSettings;
            _settingService = settingService;
            _httpContext = httpContext;
        }

        #endregion


        #region Token
        private GrantTokenResponseModel GetGrantToken()
        {
            var model = new GrantTokenResponseModel()
            {
                ExpiresIn = _bkashAdvancePaymentSettings.ExpiresInSec,
                IdToken = _bkashAdvancePaymentSettings.IdToken,
                RefreshToken = _bkashAdvancePaymentSettings.RefreshToken,
                TokenType = _bkashAdvancePaymentSettings.TokenType,
                TokenCreateTime = _bkashAdvancePaymentSettings.TokenCreateTime
            };

            if (IsValidToken(model))
                return model;

            var url = "";
            if (_bkashAdvancePaymentSettings.UseSandbox)
                url = createTokenSandboxUrl;
            else
                url = createTokenLiveUrl;

            var postModel = new GrantTokenCreateModel()
            {
                AppSecret = _bkashAdvancePaymentSettings.AppSecret,
                AppKey = _bkashAdvancePaymentSettings.AppKey
            };

            var jsonModel = JsonConvert.SerializeObject(postModel);

            var data = Encoding.ASCII.GetBytes(jsonModel); //postModel.CreateData();

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = data.Length;

            request.Headers.Add("username", _bkashAdvancePaymentSettings.Username);
            request.Headers.Add("password", _bkashAdvancePaymentSettings.Password);

            using (Stream stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

            model = JsonConvert.DeserializeObject<GrantTokenResponseModel>(responseString);

            _bkashAdvancePaymentSettings.RefreshToken = model.RefreshToken;
            _bkashAdvancePaymentSettings.TokenCreateTime = DateTime.UtcNow;
            _bkashAdvancePaymentSettings.TokenType = model.TokenType;
            _bkashAdvancePaymentSettings.IdToken = model.IdToken;
            _bkashAdvancePaymentSettings.ExpiresInSec = model.ExpiresIn;
            _settingService.SaveSetting(_bkashAdvancePaymentSettings);

            return model;
        }

        private bool IsValidToken(GrantTokenResponseModel model)
        {
            if (!string.IsNullOrWhiteSpace(model.TokenType) && model.TokenCreateTime.AddSeconds(model.ExpiresIn).AddMinutes(-10) >= DateTime.UtcNow)
                return true;
            return false;
        }

        #endregion


        #region CreatePayment
        public PaymentResponseModel CreatePayment(Order order)
        {
            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = createPaymentSandboxUrl;
                else
                    url = createPaymentLiveUrl;

                var totalPayableAmount = order.OrderTotal;

                if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
                {
                    var totalPayableFromWallet = Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);

                    if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                    {
                        totalPayableAmount -= totalPayableFromWallet;
                    }
                }

                var postModel = new PaymentCreateModel()
                {
                    Amount = decimal.Round(totalPayableAmount, 2, MidpointRounding.AwayFromZero).ToString(),
                    Currency = "BDT",
                    Intent = _bkashAdvancePaymentSettings.EnableCapture ? "authorization" : "sale",
                    MerchantInvoiceNumber = order.Id.ToString()
                };

                var jsonModel = JsonConvert.SerializeObject(postModel);

                var data = Encoding.ASCII.GetBytes(jsonModel);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //_logger.Information("Create Payment: " + responseString);

                var model = JsonConvert.DeserializeObject<PaymentResponseModel>(responseString);

                order.AuthorizationTransactionCode = model.PaymentID;
                order.AuthorizationTransactionResult = model.TransactionStatus;

                _orderService.UpdateOrder(order);

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new PaymentResponseModel() { ErrorMessage = ex.Message, ErrorCode = "System error" };
            }
        }
        #endregion


        #region ExecutePayment
        public PaymentResponseModel ExecutePayment(Order order)
        {
            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = executePaymentSandboxUrl;
                else
                    url = executePaymentLiveUrl;

                url += ("/" + order.AuthorizationTransactionCode);

                var data = Encoding.ASCII.GetBytes(string.Empty);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //_logger.Information("Execute Payment: " + responseString);

                var model = JsonConvert.DeserializeObject<PaymentResponseModel>(responseString);

                order.AuthorizationTransactionId = model.TransactionId;
                order.AuthorizationTransactionResult = model.TransactionStatus;

                if (model.TransactionStatus == "Completed" || model.TransactionStatus == "Authorized")
                {
                    if (model.TransactionStatus == "Completed")
                        order.PaymentStatus = PaymentStatus.Paid;
                    else
                        order.PaymentStatus = PaymentStatus.Authorized;
                    order.OrderStatus = OrderStatus.Processing;
                }

                _orderService.UpdateOrder(order);

                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new PaymentResponseModel() { ErrorMessage = ex.Message, ErrorCode = "System error" };
            }
        }
        #endregion


        #region CapturePayment
        public CapturePaymentResult CapturePayment(Order order)
        {
            var result = new CapturePaymentResult();
            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = capturePaymentSandboxUrl;
                else
                    url = capturePaymentLiveUrl;

                url += ("/" + order.AuthorizationTransactionCode);

                var data = Encoding.ASCII.GetBytes(string.Empty);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //_logger.Information("Capture Payment: " + responseString);

                var model = JsonConvert.DeserializeObject<PaymentResponseModel>(responseString);

                if (string.IsNullOrWhiteSpace(model.ErrorCode))
                {
                    order.AuthorizationTransactionResult = model.TransactionStatus;

                    if (model.TransactionStatus == "Completed")
                        order.PaymentStatus = PaymentStatus.Paid;

                    _orderService.UpdateOrder(order);

                    result.CaptureTransactionResult = model.TransactionStatus;
                    result.CaptureTransactionId = model.TransactionId;
                    result.NewPaymentStatus = order.PaymentStatus;
                }
                else
                    result.Errors.Add(model.ErrorMessage);

            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        #endregion


        #region RefundPayment
        public RefundPaymentResult RefundPayment(Order order)
        {
            var result = new RefundPaymentResult();
            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = refundPaymentSandboxUrl;
                else
                    url = refundPaymentLiveUrl;

                url += ("/" + order.AuthorizationTransactionId);

                var postModel = new RefundPostModel()
                {
                    Amount = decimal.Round(order.OrderTotal, 2, MidpointRounding.AwayFromZero).ToString(),
                    Currency = "BDT"
                };

                var jsonModel = JsonConvert.SerializeObject(postModel);

                var data = Encoding.ASCII.GetBytes(jsonModel);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //_logger.Information("Refund Payment: " + responseString);

                var model = JsonConvert.DeserializeObject<PaymentResponseModel>(responseString);

                if (string.IsNullOrWhiteSpace(model.ErrorCode))
                {
                    order.AuthorizationTransactionResult = model.TransactionStatus;

                    if (model.TransactionStatus == "Completed")
                        order.PaymentStatus = PaymentStatus.Refunded;

                    _orderService.UpdateOrder(order);

                    result.NewPaymentStatus = order.PaymentStatus;
                }
                else
                    result.Errors.Add(model.ErrorMessage);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        #endregion


        #region VoidPayment
        public VoidPaymentResult VoidPayment(Order order)
        {
            var result = new VoidPaymentResult();
            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = voidPaymentSandboxUrl;
                else
                    url = voidPaymentLiveUrl;

                url += ("/" + order.AuthorizationTransactionCode);

                var data = Encoding.ASCII.GetBytes(string.Empty);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = data.Length;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                //using (Stream stream = request.GetRequestStream())
                //{
                //    stream.Write(data, 0, data.Length);
                //}

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
                //_logger.Information("Void Payment: " + responseString);

                var model = JsonConvert.DeserializeObject<PaymentResponseModel>(responseString);

                if (string.IsNullOrWhiteSpace(model.ErrorCode))
                {
                    order.AuthorizationTransactionResult = model.TransactionStatus;

                    if (model.TransactionStatus == "Cancelled")
                        order.PaymentStatus = PaymentStatus.Voided;

                    _orderService.UpdateOrder(order);

                    result.NewPaymentStatus = order.PaymentStatus;
                }
                else
                    result.Errors.Add(model.ErrorMessage);
            }
            catch (Exception ex)
            {
                result.Errors.Add(ex.Message);
            }
            return result;
        }
        #endregion


        #region GetPayment

        public QueryResponseModel GetPayment(string paymentId)
        {
            var result = new QueryResponseModel();

            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = queryPaymentSandboxUrl;
                else
                    url = queryPaymentLiveUrl;

                url += ("/" + paymentId);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = 0;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                result = JsonConvert.DeserializeObject<QueryResponseModel>(responseString);

                if (!string.IsNullOrWhiteSpace(result.ErrorCode))
                    result.Status = false;
                else
                    result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion


        #region GetBalance
        public QueryResponseModel GetBalance()
        {
            var result = new QueryResponseModel();

            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = organizationBalanceSandboxUrl;
                else
                    url = organizationBalanceLiveUrl;

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = 0;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                result = JsonConvert.DeserializeObject<QueryResponseModel>(responseString);

                if (!string.IsNullOrWhiteSpace(result.ErrorCode))
                    result.Status = false;
                else
                    result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }
        #endregion


        #region GetTransaction
        public QueryResponseModel GetTransaction(string transactionId)
        {
            var result = new QueryResponseModel();

            try
            {
                var tokenResponse = GetGrantToken();

                var url = "";
                if (_bkashAdvancePaymentSettings.UseSandbox)
                    url = searchTransactionSandboxUrl;
                else
                    url = searchTransactionLiveUrl;

                url += ("/" + transactionId);

                var data = Encoding.ASCII.GetBytes(string.Empty);

                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.ContentLength = 0;

                request.Headers.Add("Authorization", tokenResponse.IdToken);
                request.Headers.Add("X-APP-Key", _bkashAdvancePaymentSettings.AppKey);

                var response = (HttpWebResponse)request.GetResponse();
                var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                result = JsonConvert.DeserializeObject<QueryResponseModel>(responseString);

                if (!string.IsNullOrWhiteSpace(result.ErrorCode))
                    result.Status = false;
                else
                    result.Status = true;
            }
            catch (Exception ex)
            {
                result.Status = false;
                result.ErrorMessage = ex.Message;
            }
            return result;
        }

        #endregion
    }
}

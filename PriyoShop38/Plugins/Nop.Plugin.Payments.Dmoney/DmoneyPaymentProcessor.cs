using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Plugins;
using Nop.Plugin.Payments.Dmoney.Controllers;
using Nop.Plugin.Payments.Dmoney.Data;
using Nop.Plugin.Payments.Dmoney.Domains;
using Nop.Plugin.Payments.Dmoney.Services;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Nop.Plugin.Payments.Dmoney
{
    public class DmoneyPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly IVendorService _vendorService;
        private readonly HttpContextBase _httpContext;
        private readonly DmoneyPaymentSettings _dmoneyPaymentSettings;
        private readonly IOrderService _orderService;
        private readonly DmoneyObjectContext _context;
        private readonly IDmoneyTransactionService _dmoneyTransactionService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Const

        public DmoneyPaymentProcessor(IVendorService vendorService,
            HttpContextBase httpContext,
            DmoneyPaymentSettings dmoneyPaymentSettings,
            IOrderService orderService,
            DmoneyObjectContext context,
            IDmoneyTransactionService dmoneyTransactionService,
            IWebHelper webHelper)
        {
            this._vendorService = vendorService;
            this._httpContext = httpContext;
            this._dmoneyPaymentSettings = dmoneyPaymentSettings;
            this._orderService = orderService;
            this._context = context;
            this._dmoneyTransactionService = dmoneyTransactionService;
            this._webHelper = webHelper;
        }

        #endregion

        #region Properies

        public bool SupportCapture
        {
            get
            {
                return false;
            }
        }

        public bool SupportPartiallyRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportRefund
        {
            get
            {
                return false;
            }
        }

        public bool SupportVoid
        {
            get
            {
                return false;
            }
        }

        public RecurringPaymentType RecurringPaymentType
        {
            get
            {
                return RecurringPaymentType.NotSupported;
            }
        }

        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Redirection;
            }
        }

        public bool SkipPaymentInfo
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Methods

        public override void Install()
        {
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.ApproveMessase", "Payment successfully done!");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.CancelMessase", "Payment process is cancelled by user.");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.DeclineMessase", "Payment process is declined by gateway.");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl", "Gateway url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl", "Transaction verification url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode", "Organization code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.Password", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey", "Secret key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.BillerCode", "Biller code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl", "Approve url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl", "Cancel url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl", "Decline url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog", "Enable log");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl.Hint", "Gateway url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl.Hint", "Transaction verification url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode.Hint", "Organization code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.Password.Hint", "Password");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey.Hint", "Secret key");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.BillerCode.Hint", "Biller code");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl.Hint", "Approve url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl.Hint", "Cancel url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl.Hint", "Decline url");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog.Hint", "Enable log");

            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl.Required", "Gateway url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl.Required", "Transaction verification url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode.Required", "Organization code is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.Password.Required", "Password is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey.Required", "Secret key is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl.Required", "Approve url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl.Required", "Cancel url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl.Required", "Decline url is required");
            this.AddOrUpdatePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog.Required", "Enable log is required");

            _context.InstallSchema();

            base.Install();
        }

        public override void Uninstall()
        {
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.ApproveMessase");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.CancelMessase");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.DeclineMessase");

            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.Password");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.BillerCode");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog");
                 
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.Password.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.BillerCode.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl.Hint");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog.Hint");
                 
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.GatewayUrl.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.TransactionVerificationUrl.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.OrgCode.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.Password.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.SecretKey.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.ApproveUrl.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.CancelUrl.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.DeclineUrl.Required");
            this.DeletePluginLocaleResource("Plugins.Payments.Dmoney.EnableLog.Required");

            base.Uninstall();
        }


        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            var result = new CancelRecurringPaymentResult();
            result.AddError("Recurring payment not supported");
            return result;
        }

        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            
            if (order.PaymentStatus != PaymentStatus.Pending)
                return false;
            
            if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes < 1)
                return false;

            return true;
        }

        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            var result = new CapturePaymentResult();
            result.AddError("Capture method not supported");
            return result;
        }

        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return 0;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "PaymentDmoney";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Dmoney.Controllers" }, { "area", null } };
        }

        public Type GetControllerType()
        {
            return typeof(PaymentDmoneyController);
        }

        public void GetPaymentInfoRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PaymentInfo";
            controllerName = "PaymentDmoney";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Payments.Dmoney.Controllers" }, { "area", null } };
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

        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            var order = postProcessPaymentRequest.Order;

            var date = DateTime.UtcNow;
            var trackingNo = Guid.NewGuid() + "__" + order.OrderGuid;

            var approveUrl = _webHelper.GetStoreLocation() + "dmoney/approve/" + WebUtility.UrlEncode(trackingNo);
            var cancelUrl = _webHelper.GetStoreLocation() + "dmoney/cancel/" + WebUtility.UrlEncode(trackingNo);
            var declineUrl = _webHelper.GetStoreLocation() + "dmoney/decline/" + WebUtility.UrlEncode(trackingNo);

            var transaction = new DmoneyTransaction()
            {
                Amount = order.OrderTotal,
                CreatedOnUtc = date,
                LastUpdatedOnUtc = date,
                TransactionStatus = TransactionStatus.Init,
                Status = 0,
                OrderId = order.Id,
                TransactionTrackingNo = trackingNo
            };

            _dmoneyTransactionService.InsertTransaction(transaction);

            var sb = new StringBuilder();
            sb.Append("<html>");
            sb.AppendFormat(@"<body onload='document.forms[""form""].submit()'>");
            sb.AppendFormat("<form name='form' action='{0}' method='post'>", _dmoneyPaymentSettings.GatewayUrl);

            sb.AppendFormat("<input type=\"hidden\" name=\"orgCode\" value=\"" + _dmoneyPaymentSettings.OrgCode + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"password\" value=\"" + _dmoneyPaymentSettings.Password + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"secretKey\" value=\"" + _dmoneyPaymentSettings.SecretKey + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"billerCode\" value=\"" + _dmoneyPaymentSettings.BillerCode + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"billOrInvoiceNo\" value=\"" + order.Id + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"transactionTrackingNo\" value=\"" + trackingNo + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"amount\" value=\"" + order.OrderTotal + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"approvedUrl\" value=\"" + approveUrl + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"cancelUrl\" value=\"" + cancelUrl + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"declineUrl\" value=\"" + declineUrl + "\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"language\" value=\"EN\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"orderType\" value=\"merchantPayment\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"currency\" value=\"050\"/>");
            sb.AppendFormat("<input type=\"hidden\" name=\"description\" value=\"" + GetOrderDescription(order.OrderItems) + "\"/>");

            sb.Append("</form>");
            sb.Append("</body>");
            sb.Append("</html>");
            _httpContext.Response.Write(sb.ToString());
            _httpContext.Response.End();
        }

        private string GetOrderDescription(ICollection<OrderItem> orderItems)
        {
            var sb = new StringBuilder();

            foreach (var item in orderItems)
            {
                sb.AppendLine(item.Product.Name);
            }

            return sb.ToString();
        }

        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            var result = new ProcessPaymentResult { NewPaymentStatus = PaymentStatus.Pending };
            return result;
        }

        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            var result = new RefundPaymentResult();
            result.AddError("Refund method not supported");
            return result;
        }

        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            var result = new VoidPaymentResult();
            result.AddError("Void method not supported");
            return result;
        }

        public void PostProcessMakePayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

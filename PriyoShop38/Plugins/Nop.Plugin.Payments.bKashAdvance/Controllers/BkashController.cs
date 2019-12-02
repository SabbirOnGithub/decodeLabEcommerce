using System;
using System.Collections.Generic;
using System.Web;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.bKashAdvance.Models;
using Nop.Plugin.Payments.bKashAdvance.Services;
using Nop.Services.Orders;
using System.Web.Mvc;
using Nop.Core.Domain.Logging;
using Nop.Services.Logging;

namespace Nop.Plugin.Payments.bKashAdvance.Controllers
{
    public class BkashController : Controller
    {
        private readonly IBkashAdvanceService _bkashAdvanceService;
        private readonly BkashAdvancePaymentSettings _bkashAdvancePaymentSettings;
        private readonly IOrderService _orderService;
        private readonly IWorkContext _workContext;
        private readonly ILogger _logger;
        private readonly HttpContextBase _httpContext;

        public BkashController(IBkashAdvanceService bkashAdvanceService,
            IOrderService orderService,
            IWorkContext workContext,
            BkashAdvancePaymentSettings bkashAdvancePaymentSettings,
            ILogger logger,
            HttpContextBase httpContext)
        {
            _bkashAdvancePaymentSettings = bkashAdvancePaymentSettings;
            _logger = logger;
            _bkashAdvanceService = bkashAdvanceService;
            _orderService = orderService;
            _workContext = workContext;
            _httpContext = httpContext;
        }

        public ActionResult Pay(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            #region wallet payment
            if (_httpContext.Session["OrderWalletPaymentInfo"] is Dictionary<string, object> OrderWalletPaymentInfo && OrderWalletPaymentInfo.ContainsKey("willDeduct"))
            {
                if (Convert.ToBoolean(OrderWalletPaymentInfo["willDeduct"]))
                {
                    order.OrderTotal -= Convert.ToDecimal(OrderWalletPaymentInfo["walletPayAmount"]);
                }
            }

            //_httpContext.Session["OrderWalletPaymentInfo"] = null;

            #endregion

            if (order == null || order.CustomerId != _workContext.CurrentCustomer.Id ||
                order.OrderStatus == OrderStatus.Cancelled || order.OrderStatus == OrderStatus.Complete ||
                order.PaymentStatus == PaymentStatus.Paid || order.PaymentStatus == PaymentStatus.Authorized)
            {
                if (order == null)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Order is null");
                else if (order.CustomerId != _workContext.CurrentCustomer.Id)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Customers don't match");
                else if (order.OrderStatus == OrderStatus.Cancelled)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Order has already been cancelled");
                else if (order.OrderStatus == OrderStatus.Complete)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Order has already been completed");
                else if (order.PaymentStatus == PaymentStatus.Paid)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Payment has already been completed");
                else if (order.PaymentStatus == PaymentStatus.Authorized)
                    _logger.InsertLog(LogLevel.Information, "bKashAdvance | Payment-status is authorized");

                return RedirectToRoute("HomePage");
            }
            var model = new PaymentViewModel()
            {
                Intent = _bkashAdvancePaymentSettings.EnableCapture ? "authorization" : "sale",
                OrderId = order.Id,
                OrderTotal = order.OrderTotal,
                IsSandBox = _bkashAdvancePaymentSettings.UseSandbox
            };

            return View("~/Plugins/Payments.bKashAdvance/Views/BkashPay.cshtml", model);
        }

        [HttpPost]
        public ActionResult CreatePayment(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            if (order == null || //order.CustomerId != _workContext.CurrentCustomer.Id ||
                order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Complete)
            {
                var response = new { status = false, message = "Order not found." };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                var response = new { status = false, message = "Order already paid." };
                return Json(response, JsonRequestBehavior.AllowGet);
            }


            var model = _bkashAdvanceService.CreatePayment(order);

            if (!string.IsNullOrWhiteSpace(model.ErrorCode))
            {
                return Json(new
                {
                    status = false,
                    message = model.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                status = true,
                data = JsonConvert.SerializeObject(model)
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ExecutePayment(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);

            if (order == null || //order.CustomerId != _workContext.CurrentCustomer.Id ||
                order.OrderStatus == OrderStatus.Cancelled ||
                order.OrderStatus == OrderStatus.Complete)
            {
                var response = new { status = false, message = "Order not found." };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            if (order.PaymentStatus == PaymentStatus.Paid)
            {
                var response = new { status = false, message = "Order already paid." };
                return Json(response, JsonRequestBehavior.AllowGet);
            }

            var model = _bkashAdvanceService.ExecutePayment(order);

            if (!string.IsNullOrWhiteSpace(model.ErrorCode))
            {
                return Json(new
                {
                    status = false,
                    message = model.ErrorMessage
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                status = true,
                data = model
            }, JsonRequestBehavior.AllowGet);
        }
    }
}

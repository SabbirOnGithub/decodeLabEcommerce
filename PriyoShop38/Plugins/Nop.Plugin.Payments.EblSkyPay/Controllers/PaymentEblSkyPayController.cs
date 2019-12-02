using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Logging;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.EblSkyPay.Models;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System.Net;
using System.IO;
using System.Web;
using System.Text;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.EblSkyPay.Extensions;
using Nop.Plugin.Payments.EblSkyPay.Services;
using Nop.Services.Security;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Localization;
using Nop.Web.Extensions;

namespace Nop.Plugin.Payments.EblSkyPay.Controllers
{
    public class PaymentEblSkyPayController : BasePaymentController
    {
        #region Field

        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IStoreService _storeService;
        private readonly EblSkyPayPaymentSettings _eblSkyPaySettings;
        private readonly ILogger _logService;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly ISkyPayService _eblSkyPayService;
        private readonly ILocalizationService _localizationService;
        private readonly HttpContextBase _httpContext;
        private readonly EblSkyPayPaymentProcessor _eblSkyPayProcessor;

        #endregion

        #region ctr

        public PaymentEblSkyPayController(IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IWorkContext workContext,
            ISettingService settingService,
            IStoreService storeService,
            EblSkyPayPaymentSettings eblSkyPaySettings,
            ILogger logService,
            IStoreContext storeContext,
            IPermissionService permissionService, 
            ISkyPayService eblSkyPayService,
            ILocalizationService localizationService, 
            HttpContextBase httpContext,
            EblSkyPayPaymentProcessor eblSkyPayProcessor)
        {
            _orderService = orderService;
            _orderProcessingService = orderProcessingService;
            _workContext = workContext;
            _settingService = settingService;
            _storeService = storeService;
            _eblSkyPaySettings = eblSkyPaySettings;
            _logService = logService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _eblSkyPayService = eblSkyPayService;
            _localizationService = localizationService;
            _httpContext = httpContext;
            _eblSkyPayProcessor = eblSkyPayProcessor;
        }

        #endregion

        #region Configure

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var paymentSettings = _settingService.LoadSetting<EblSkyPayPaymentSettings>(storeScope);

            var model = new ConfigurationModel();

            model.DescriptionText = paymentSettings.DescriptionText;
            model.AdditionalFee = paymentSettings.AdditionalFee;
            model.AdditionalFeePercentage = paymentSettings.AdditionalFeePercentage;
            model.EnableLogging = paymentSettings.EnableLogging;
            model.MerchantId = paymentSettings.MerchantId;
            model.MerchantName = paymentSettings.MerchantName;
            model.OperatorId = paymentSettings.OperatorId;
            model.Password = paymentSettings.Password;
            model.AutoRedirectEnable = paymentSettings.AutoRedirectEnable;
            model.UseSandBox = paymentSettings.UseSandbox;
            model.UseHttsForRedirection = paymentSettings.UseHttsForRedirection;
            model.GatewayUrl = paymentSettings.GatewayUrl;

            return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/Configure.cshtml", model);
        }

        /// <summary>
        /// Configure POST
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var paymentSettings = _settingService.LoadSetting<EblSkyPayPaymentSettings>(storeScope);

            //save settings
            paymentSettings.DescriptionText = model.DescriptionText;
            paymentSettings.AdditionalFee = model.AdditionalFee;
            paymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            paymentSettings.EnableLogging = model.EnableLogging;
            paymentSettings.MerchantId = model.MerchantId;
            paymentSettings.MerchantName = model.MerchantName;
            paymentSettings.OperatorId = model.OperatorId;
            paymentSettings.Password = model.Password;
            paymentSettings.AutoRedirectEnable = model.AutoRedirectEnable;
            paymentSettings.UseSandbox = model.UseSandBox;
            paymentSettings.UseHttsForRedirection = model.UseHttsForRedirection;
            paymentSettings.GatewayUrl = model.GatewayUrl;

            _settingService.SaveSetting(paymentSettings);
            
            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
        }

        #endregion

        #region Admin Panel(UI)

        public ActionResult Manage()
        {
            return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/Manage.cshtml");
        }

        public ActionResult ListJsonResponse(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("No access");

            var items = _eblSkyPayService.GetById(id);

            var gridModel = new DataSourceResult()
            {
                Data = new List<EblSkyPayModel>
                {
                    items.ToModel()
                },
                Total = 1
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ListJson(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("No access");

            var items = _eblSkyPayService.GetAllPagedItems(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = items.Select(x =>
                {
                    var m = x.ToModel();

                    m.PaymentStatus = x.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);

                    return m;
                }),
                Total = items.TotalCount
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult GetJsonResponse(FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Unrestricted Access");

            try
            {
                //request to other server
                var req = (HttpWebRequest)WebRequest.Create(form["Url"].ToString());
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";

                string formContent = string.Format(form["PostData"].ToString());
                req.ContentLength = formContent.Length;

                using (var sw = new StreamWriter(req.GetRequestStream(), Encoding.ASCII))
                    sw.Write(formContent);

                string response = null;
                using (var sr = new StreamReader(req.GetResponse().GetResponseStream()))
                    response = HttpUtility.UrlDecode(sr.ReadToEnd());

                return Json(new { Success = true, Response = response });
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Response = e.InnerException });
            }
        }

        #endregion

        #region Public UI

        /// <summary>
        /// Get Payment Info
        /// </summary>
        /// <returns></returns>
        public ActionResult PaymentInfo()
        {
            return Content("No Payment Info Required");            
        }

        /// <summary>
        /// Prepare CheckOut Session
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult EblSkyPay(int orderId = 0)
        {
            //log msg
            if (_eblSkyPaySettings.EnableLogging)
            {
                _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.RequestAtController", $"URL : {_httpContext.Request.Url}, Order Id : {orderId} ");
            }

            //valid order
            if (orderId > 0)
            {
                var order = _orderService.GetOrderById(orderId);
                if(order==null)
                    return Content("Invalid Order Id");

                if (!order.PaymentMethodSystemName.Equals("Payments.EblSkyPay"))
                    return Content("Invalid Order Payment Method");

                //prepare checkout session model
                var model = _eblSkyPayProcessor.ProcessTransaction(order);
                if(model.ErrorInCreateOrder)
                {
                    //error page redirect
                    var failOrderModel = new FailOrderModel
                    {
                        OrderId = order.Id,
                        FailReason = "Unable to connect with EBL Payment gateway/Something went wrong please contact to system administrator or mail to otb3@othoba.com."
                    };
                    failOrderModel.TrxModel = order.ToDataLayerTrxModel("Something went wrong. Unable to connect with ebl gateway");
                    return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/FailOrder.cshtml", failOrderModel);
                }

                model.TrxModel = order.ToDataLayerTrxModel("Init Payment");

                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/EblSkyPay.cshtml", model);
            }
            return Content("Invalid Order Id");
        }

        /// <summary>
        /// Redirect form EBL GateWay
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public ActionResult SuccessOrder(int orderId, string resultIndicator= null, string sessionVersion= null)
        {
            var failOrderModel = new FailOrderModel
            {
                OrderId = orderId
            };

            //in valid order
            var order = _orderService.GetOrderById(orderId);
            if (order == null)
            {
                failOrderModel.FailReason = "Invalid Order Id";
                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
            }

            //invalid payment method
            if (!order.PaymentMethodSystemName.Equals("Payments.EblSkyPay"))
            {
                failOrderModel.FailReason = "Invalid Order Payment Method";
                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
            }

            //invalid session
            var eblEntity = _eblSkyPayService.GetByOrderId(orderId).FirstOrDefault();
            if (eblEntity == null)
            {
                failOrderModel.FailReason = "Session Expired try again.";
                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/FailOrder.cshtml", failOrderModel);
            }
            
            //check status
            bool success = false;
            var responseModel = _eblSkyPayProcessor.CheckTransaction(order, out success);

            //check result
            if (success)
            {
                //save response to db
                var entity = _eblSkyPayService.GetByOrderId(order.Id).LastOrDefault();
                if (entity == null)
                {
                    failOrderModel.TrxModel = order.ToDataLayerTrxModel("Invalid Session/Session Expire. Please try again.");
                    return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
                }

                //update db
                entity.OrderRetriveResponse = responseModel.OrderRetriveResponse;
                _eblSkyPayService.Update(entity);

                //check for order id
                if (responseModel.OrderId == order.Id)
                {
                    //trxcurrency amount and status check
                    if (responseModel.TrxCurrency == "BDT" && responseModel.Amount >= order.OrderTotal && responseModel.Status == "CAPTURED")
                    {
                        //update log
                        if (_eblSkyPaySettings.EnableLogging)
                        {
                            _logService.InsertLog(LogLevel.Debug, "EBL.SkyPay.SuccessOrder", $"Paid {order.Id} / {order.ShippingAddress.Email}");
                        }

                        //add order note
                        order.OrderNotes.Add(new OrderNote()
                        {
                            CreatedOnUtc = DateTime.UtcNow,
                            DisplayToCustomer = false,
                            Note = $"Payment Automatically successfull by // {_workContext.CurrentCustomer.Email} order id // {order.Id}"
                        });
                        _orderService.UpdateOrder(order);

                        if (order.PaymentStatusId != (int) PaymentStatus.Paid)
                        {
                            _orderProcessingService.MarkOrderAsPaid(order);
                        }
                                
                        //update ebl model
                        if (entity != null)
                        {
                            entity.PaymentStatusId = (int)PaymentStatus.Paid;
                            entity.PaymentDate = responseModel.TrxDate;
                            _eblSkyPayService.Update(entity);
                        }

                        //Save OrderId and PaymentMethodSystemName for later use in MiscOthobaOnePageCheckoutController
                        if (_httpContext.Session != null)
                        {
                            _httpContext.Session["PaymentMethodSystemName"] = order.PaymentMethodSystemName;
                            _httpContext.Session["LastOrderId"] = order.Id;
                        }

                        return RedirectToAction("OthobaCheckOutComplete", "MiscOthobaOnePageCheckOut");
                    }
                    failOrderModel.FailReason= "Invalid currency/amount";
                    return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
                }

                failOrderModel.FailReason = "Invalid Order Id";
                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
            }

            failOrderModel.FailReason = "Error connecting with Merchant site/payment process failed.";
            return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", failOrderModel);
        }

        public ActionResult CancelOrder(int orderId=0)
        {
            //valid order
            if (orderId > 0)
            {
                var order = _orderService.GetOrderById(orderId);
                if (order == null)
                    return Content("Invalid Order Id");

                if (!order.PaymentMethodSystemName.Equals("Payments.EblSkyPay"))
                    return Content("Invalid Order Payment Method");

                var model = new FailOrderModel();
                model.OrderId = order.Id;
                model.TrxModel = order.ToDataLayerTrxModel("Cancel Order");
                model.FailReason = "Payment process is not complete/Payment cancel by customer";

                return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/CancelOrder.cshtml", model);
            }
            return Content("Invalid Order Id");
        }

        public ActionResult ErrorCallback(FormCollection form, int orderId = 0)
        {
            var errorModel = new ErrorCallbackModel();

            //parameter
            var param = _httpContext.Request.RawUrl;
            _logService.InsertLog(LogLevel.Error, "EBL.Sky.Pay.ErrorCallback", param);

            Order lastOrder;

            if (orderId > 0)
            {
                lastOrder = _orderService.GetOrderById(orderId);
                if (lastOrder != null)
                    errorModel.OrderId = lastOrder.Id;
            }
            else
            {
                var orders = _orderService.SearchOrders(customerId: _workContext.CurrentCustomer.Id);
                if (orders.Any())
                {
                    lastOrder = orders.Last();
                    errorModel.OrderId = lastOrder.Id;
                }
            }

            return View("~/Plugins/Payments.EblSkyPay/Views/PaymentEblSkyPay/ErrorCallback.cshtml", errorModel);
        }

        #endregion

        #region Non Action (utility)

        [NonAction]
        public override IList<string> ValidatePaymentForm(FormCollection form)
        {
            var warnings = new List<string>();

            return warnings;
        }

        [NonAction]
        public override ProcessPaymentRequest GetPaymentInfo(FormCollection form)
        {
            var paymentInfo = new ProcessPaymentRequest();
            return paymentInfo;
        }

        #endregion
    }
}
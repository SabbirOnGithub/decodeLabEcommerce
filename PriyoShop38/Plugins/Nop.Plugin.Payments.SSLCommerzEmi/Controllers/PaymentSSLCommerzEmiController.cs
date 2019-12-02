using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.SSLCommerzEmi.Domain;
using Nop.Plugin.Payments.SSLCommerzEmi.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Nop.Plugin.Payments.SSLCommerzEmi.Controllers
{
    public class PaymentSSLCommerzEmiController : BasePaymentController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IPaymentService _paymentService;
        private readonly IOrderService _orderService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IStoreContext _storeContext;
        private readonly ILogger _logger;
        private readonly IWebHelper _webHelper;
        private readonly PaymentSettings _paymentSettings;
        private readonly SSLCommerzEmiPaymentSettings _SSLCommerzEmiPaymentSettings;
        private readonly ILocalizationService _localizationService;

        public PaymentSSLCommerzEmiController(
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            IPaymentService paymentService,
            IOrderService orderService,
            IOrderProcessingService orderProcessingService,
            IStoreContext storeContext,
            ILogger logger,
            IWebHelper webHelper,
            PaymentSettings paymentSettings,
            SSLCommerzEmiPaymentSettings SSLCommerzEmiPaymentSettings,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._paymentService = paymentService;
            this._orderService = orderService;
            this._orderProcessingService = orderProcessingService;
            this._storeContext = storeContext;
            this._logger = logger;
            this._webHelper = webHelper;
            this._paymentSettings = paymentSettings;
            this._SSLCommerzEmiPaymentSettings = SSLCommerzEmiPaymentSettings;
            this._localizationService = localizationService;
        }

        [ChildActionOnly]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var SSLCommerzEmiPaymentSettings = _settingService.LoadSetting<SSLCommerzEmiPaymentSettings>(storeScope);

            var model = new ConfigurationModel();

            model.UseSandbox = SSLCommerzEmiPaymentSettings.UseSandbox;
            model.StoreId = SSLCommerzEmiPaymentSettings.StoreId;
            model.StorePassword = SSLCommerzEmiPaymentSettings.StorePassword;
            model.PassProductNamesAndTotals = SSLCommerzEmiPaymentSettings.PassProductNamesAndTotals;            
            model.EnableIpn = SSLCommerzEmiPaymentSettings.EnableIpn;
            model.AdditionalFee = SSLCommerzEmiPaymentSettings.AdditionalFee;
            model.AdditionalFeeThreeMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeThreeMonthOption;
            model.AdditionalFeeSixMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeSixMonthOption;
            model.AdditionalFeeNineMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeNineMonthOption;

            model.AdditionalFeeTwelveMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeTwelveMonthOption;
            model.AdditionalFeeEighteenMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeEighteenMonthOption;
            model.AdditionalFeeTwentyFourMonthOption = SSLCommerzEmiPaymentSettings.AdditionalFeeTwentyFourMonthOption;



            model.AdditionalFeePercentage = SSLCommerzEmiPaymentSettings.AdditionalFeePercentage;
            model.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage = SSLCommerzEmiPaymentSettings.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage;
            model.HidePaymentMethodForAmountLessThan = SSLCommerzEmiPaymentSettings.HidePaymentMethodForAmountLessThan;
            var cardTypes = new CardTypes();
            //Load card names
            string prefferedCardTypes = _SSLCommerzEmiPaymentSettings.PrefferedCardTypes;
            foreach (string cardType in cardTypes.CardNames)
                model.AvailableCardTypes.Add(cardType);

            if (!String.IsNullOrEmpty(prefferedCardTypes))
                foreach (string cardType in cardTypes.CardNames)
                {
                    string cardId = CardTypes.GetCardId(cardType);
                    if (!String.IsNullOrEmpty(cardId) && !String.IsNullOrEmpty(prefferedCardTypes))
                    {
                        if (prefferedCardTypes.Contains(cardId))
                            model.PrefferedCardTypes.Add(cardType);
                    }
                }

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.UseSandbox_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.UseSandbox, storeScope);
                model.StoreId_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.StoreId, storeScope);
                model.StorePassword_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.StorePassword, storeScope);
                model.PassProductNamesAndTotals_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);
                model.PrefferedCardTypes_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.PrefferedCardTypes, storeScope);
                model.EnableIpn_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.EnableIpn, storeScope);
                model.AdditionalFee_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFee,
                    storeScope);

                model.AdditionalFeeThreeMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeThreeMonthOption,
                   storeScope);
                model.AdditionalFeeSixMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeSixMonthOption,
                   storeScope);

                model.AdditionalFeeNineMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeNineMonthOption,
                   storeScope);

                model.AdditionalFeeTwelveMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwelveMonthOption,
                   storeScope);

                model.AdditionalFeeEighteenMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeEighteenMonthOption,
                storeScope);

                model.AdditionalFeeTwentyFourMonthOption_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwentyFourMonthOption,
                storeScope);


                model.AdditionalFeePercentage_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeePercentage, storeScope);
                model.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage, storeScope);

                model.HidePaymentMethodForAmountLessThan_OverrideForStore = _settingService.SettingExists(SSLCommerzEmiPaymentSettings, x => x.HidePaymentMethodForAmountLessThan, storeScope);
            }

            return View("~/Plugins/Payments.SSLCommerzEmi/Views/PaymentSSLCommerzEmi/Configure.cshtml", model);
        }

        [HttpPost]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var SSLCommerzEmiPaymentSettings = _settingService.LoadSetting<SSLCommerzEmiPaymentSettings>(storeScope);

            //save settings
            SSLCommerzEmiPaymentSettings.UseSandbox = model.UseSandbox;
            SSLCommerzEmiPaymentSettings.StoreId = model.StoreId;
            SSLCommerzEmiPaymentSettings.StorePassword = model.StorePassword;
            SSLCommerzEmiPaymentSettings.PassProductNamesAndTotals = model.PassProductNamesAndTotals;
            SSLCommerzEmiPaymentSettings.EnableIpn = model.EnableIpn;
            SSLCommerzEmiPaymentSettings.AdditionalFee = model.AdditionalFee;
            SSLCommerzEmiPaymentSettings.AdditionalFeeThreeMonthOption = model.AdditionalFeeThreeMonthOption;
            SSLCommerzEmiPaymentSettings.AdditionalFeeSixMonthOption = model.AdditionalFeeSixMonthOption;
            SSLCommerzEmiPaymentSettings.AdditionalFeeNineMonthOption = model.AdditionalFeeNineMonthOption;

            SSLCommerzEmiPaymentSettings.AdditionalFeeTwelveMonthOption = model.AdditionalFeeTwelveMonthOption;
            SSLCommerzEmiPaymentSettings.AdditionalFeeEighteenMonthOption = model.AdditionalFeeEighteenMonthOption;
            SSLCommerzEmiPaymentSettings.AdditionalFeeTwentyFourMonthOption = model.AdditionalFeeTwentyFourMonthOption;




            SSLCommerzEmiPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            SSLCommerzEmiPaymentSettings.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage = model.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage;
            SSLCommerzEmiPaymentSettings.HidePaymentMethodForAmountLessThan = model.HidePaymentMethodForAmountLessThan;

            //save selected cards
            var prefferedCards = new StringBuilder();
            int prefferedCardsSelectedCount = 0;
            if (model.CheckedCardTypes != null)
            {
                foreach (var ct in model.CheckedCardTypes)
                {
                    prefferedCardsSelectedCount++;
                    string cardId = CardTypes.GetCardId(ct);
                    if (!String.IsNullOrEmpty(cardId))
                        prefferedCards.AppendFormat("{0},", cardId);
                }
            }
            _SSLCommerzEmiPaymentSettings.PrefferedCardTypes = prefferedCards.ToString();

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.UseSandbox_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.UseSandbox, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.UseSandbox, storeScope);

            if (model.StoreId_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.StoreId, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.StoreId, storeScope);

            if (model.StorePassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.StorePassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.StorePassword, storeScope);

            if (model.PassProductNamesAndTotals_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.PassProductNamesAndTotals, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.PassProductNamesAndTotals, storeScope);

            if (model.PrefferedCardTypes_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.PrefferedCardTypes, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.PrefferedCardTypes, storeScope);

            if (model.EnableIpn_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.EnableIpn, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.EnableIpn, storeScope);

            if (model.AdditionalFee_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFee, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFee, storeScope);


            if (model.AdditionalFeeThreeMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeThreeMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeThreeMonthOption, storeScope);
            if (model.AdditionalFeeSixMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeSixMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeSixMonthOption, storeScope);

            if (model.AdditionalFeeNineMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeNineMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeNineMonthOption, storeScope);


            if (model.AdditionalFeeTwelveMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwelveMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwelveMonthOption, storeScope);


            if (model.AdditionalFeeEighteenMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeEighteenMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeEighteenMonthOption, storeScope);


            if (model.AdditionalFeeTwentyFourMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwentyFourMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwentyFourMonthOption, storeScope);

            if (model.AdditionalFeeTwentyFourMonthOption_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwentyFourMonthOption, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeeTwentyFourMonthOption, storeScope);


            if (model.AdditionalFeePercentage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeePercentage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.AdditionalFeePercentage, storeScope);

            if (model.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage, storeScope);

            if (model.HidePaymentMethodForAmountLessThan_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(SSLCommerzEmiPaymentSettings, x => x.HidePaymentMethodForAmountLessThan, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(SSLCommerzEmiPaymentSettings, x => x.HidePaymentMethodForAmountLessThan, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PaymentInfo()
        {
            var model = new PaymentInfoModel();
            return View("~/Plugins/Payments.SSLCommerzEmi/Views/PaymentSSLCommerzEmi/PaymentInfo.cshtml", model);
        }

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

        [ValidateInput(false)]
        public ActionResult PaymentResult(FormCollection  form)
        {
            var processor = _paymentService.LoadPaymentMethodBySystemName("Payments.SSLCommerzEmi") as SSLCommerzEmiPaymentProcessor;
            if (processor == null || !processor.IsPaymentMethodActive(_paymentSettings) || !processor.PluginDescriptor.Installed)
                throw new NopException("SSLCommerzEmi module cannot be loaded");

            string status = CommonHelper.EnsureNotNull(form["status"]);
            string tranDate = CommonHelper.EnsureNotNull(form["tran_date"]);
            string tranId = CommonHelper.EnsureNotNull(form["tran_id"]);
            string valId = CommonHelper.EnsureNotNull(form["val_id"]);
            string amount = CommonHelper.EnsureNotNull(form["amount"]);
            string storeAmount = CommonHelper.EnsureNotNull(form["store_amount"]);
            string cardType = CommonHelper.EnsureNotNull(form["card_type"]);
            string cardNo = CommonHelper.EnsureNotNull(form["card_no"]);
            string currency = CommonHelper.EnsureNotNull(form["currency"]);
            string bankTranId = CommonHelper.EnsureNotNull(form["bank_tran_id"]);
            string cardIssuer = CommonHelper.EnsureNotNull(form["card_issuer"]);
            string cardBrand = CommonHelper.EnsureNotNull(form["card_brand"]);
            string cardIssuerCountry = CommonHelper.EnsureNotNull(form["card_issuer_country"]);
            string cardIssuerCountryCode = CommonHelper.EnsureNotNull(form["card_issuer_country_code"]);
            string currencyType = CommonHelper.EnsureNotNull(form["currency_type"]);
            string currencyAmount = CommonHelper.EnsureNotNull(form["currency_amount"]);
            string valueA = CommonHelper.EnsureNotNull(form["value_a"]);
            string valueB = CommonHelper.EnsureNotNull(form["value_b"]);
            string valueC = CommonHelper.EnsureNotNull(form["value_c"]);
            string valueD = CommonHelper.EnsureNotNull(form["value_d"]);
            string verifySign = CommonHelper.EnsureNotNull(form["varify_sign"]);
            string verifyKey = CommonHelper.EnsureNotNull(form["verify_key"]);
            string riskLevel = CommonHelper.EnsureNotNull(form["risk_level"]);
            string riskTitle = CommonHelper.EnsureNotNull(form["risk_title"]);
            string error = CommonHelper.EnsureNotNull(form["error"]);

            var sb = new StringBuilder();
            sb.AppendLine("Response:");
            sb.AppendLine("Status: " + status);
            sb.AppendLine("Transaction Date: " + tranDate);
            sb.AppendLine("Transaction Id: " + tranId);
            sb.AppendLine("Validation Id: " + valId);
            sb.AppendLine("Amount: " + amount);
            sb.AppendLine("Store Amount: " + storeAmount);            
            sb.AppendLine("Currency: " + currency);
            sb.AppendLine("Bank Transaction ID: " + bankTranId);
            sb.AppendLine("Card Type: " + cardType);
            sb.AppendLine("Card No: " + cardNo);
            sb.AppendLine("Card Issuer: " + cardIssuer);
            sb.AppendLine("Card Brand: " + cardBrand);
            sb.AppendLine("Card Issuer Country: " + cardIssuerCountry);
            sb.AppendLine("Card Issuer Country Code: " + cardIssuerCountryCode);
            sb.AppendLine("Currency Type: " + currencyType);
            sb.AppendLine("Currency Amount: " + currencyAmount);
            sb.AppendLine(valueA);
            sb.AppendLine("Verify Sign: " + verifySign);
            sb.AppendLine("Verify Key: " + verifyKey);
            sb.AppendLine("Risk Level: " + riskLevel);
            sb.AppendLine("Risk Title: " + riskTitle);

            if (!String.IsNullOrEmpty(error)) {
                sb.AppendLine("Error: " + error);
            }

            var order = _orderService.GetOrderById(Convert.ToInt32(tranId));
            if (order != null)
            {
                //order note
                order.OrderNotes.Add(new OrderNote
                {
                    Note = sb.ToString(),
                    DisplayToCustomer = false,
                    CreatedOnUtc = DateTime.UtcNow
                });
                _orderService.UpdateOrder(order);
            }
            else
            {
                _logger.Error("SSLCommerzEmi Response. Order is not found", new NopException(sb.ToString()));
            }

            var response = string.Empty;
            Dictionary<string, string> values;

            if (_SSLCommerzEmiPaymentSettings.EnableIpn && processor.GetValidationDetails(valId, out values))
            {
                string status2 = string.Empty;
                values.TryGetValue("status", out status2);
                string amount2 = string.Empty;
                values.TryGetValue("currency_amount", out amount2);

                var sb2 = new StringBuilder();
                sb2.AppendLine("SSLCommerzEmi IPN:");
                foreach (KeyValuePair<string, string> kvp in values)
                {
                    sb2.AppendLine(kvp.Key + ": " + kvp.Value);
                }

                if (order != null)
                {
                    //order note
                    order.OrderNotes.Add(new OrderNote
                    {
                        Note = sb2.ToString(),
                        DisplayToCustomer = false,
                        CreatedOnUtc = DateTime.UtcNow
                    });
                    _orderService.UpdateOrder(order);
                }
                else
                {
                    _logger.Error("SSLCommerzEmi Response. Order is not found", new NopException(sb2.ToString()));
                }

                if (status2 == "VALID" && Convert.ToDecimal(amount2) == order.OrderTotal && _orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    _orderProcessingService.MarkOrderAsPaid(order);

                    if (order.IsMakePayment)
                    {
                        return RedirectToRoute("CompletedMakePayment", new { orderId = order.Id });
                    }
                    else
                    {
                        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                    }

                   // return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                }
                else
                {
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
            }
            else if (_SSLCommerzEmiPaymentSettings.EnableIpn)
            {
                _logger.Error("Couldn't reach SSLCommerzEmi Server.", new NopException(sb.ToString()));
                return RedirectToAction("Index", "Home", new { area = "" });
            }
            else
            {
                #region brainstation
                if(status == "VALID" && Convert.ToDecimal(currencyAmount) == order.OrderTotal && _orderProcessingService.CanMarkOrderAsPaid(order))
                {
                    _orderProcessingService.MarkOrderAsPaid(order);
                    if (order.IsMakePayment)
                    {
                        return RedirectToRoute("CompletedMakePayment", new { orderId = order.Id });
                    }
                    else
                    {
                        return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                    }
                }
                
                return RedirectToAction("Index", "Home", new { area = "" });
                #endregion                
            }
        }

        public ActionResult CancelOrder(FormCollection form)
        {
            if (_SSLCommerzEmiPaymentSettings.ReturnFromSSLCommerzEmiWithoutPaymentRedirectsToOrderDetailsPage)
            {
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                    customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order != null)
                {
                    return RedirectToRoute("OrderDetails", new { orderId = order.Id });
                }
            }

            return RedirectToAction("Index", "Home", new { area = "" });
        }
    }
}

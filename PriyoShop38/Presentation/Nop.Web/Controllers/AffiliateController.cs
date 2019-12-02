using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Web.Models.Affiliates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class AffiliateController : BasePublicController
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IWorkContext _workContext;
        private readonly IWebHelper _webHelper;
        private readonly IAffiliateService _affiliateService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly CustomerSettings _customerSettings;
        private readonly ICustomerService _customerService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IOrderService _orderService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ILocalizationService _localizationService;

        #endregion

        #region Ctor

        public AffiliateController(IWorkContext workContext,
            IAffiliateService affiliateService,
            ILogger logger,
            CustomerSettings customerSettings,
            ICountryService countryService,
            IStateProvinceService stateProvinceService,
            IWebHelper webHelper,
            ICustomerService customerService,
            IPriceFormatter priceFormatter,
            IOrderService orderService,
            IDateTimeHelper dateTimeHelper,
            ILocalizationService localizationService)
        {
            this._workContext = workContext;
            this._affiliateService = affiliateService;
            this._logger = logger;
            this._customerSettings = customerSettings;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._webHelper = webHelper;
            this._customerService = customerService;
            this._priceFormatter = priceFormatter;
            this._orderService = orderService;
            this._dateTimeHelper = dateTimeHelper;
            this._localizationService = localizationService;
        }

        #endregion

        #region Utilities

        protected void PrepareOrderListModel(AffiliatedOrderListModel model, AffiliatedOrderSummary summarry)
        {
            if (model == null)
                throw new NopException(nameof(model));
            if (summarry == null)
                throw new NopException(nameof(summarry));

            model.TotalCommission = _priceFormatter.FormatPrice(summarry.TotalCommission);
            model.PayableCommission = _priceFormatter.FormatPrice(summarry.PayableCommission);
            model.PaidCommission = _priceFormatter.FormatPrice(summarry.PaidCommission);
            model.UnpaidCommission = _priceFormatter.FormatPrice(summarry.UnpaidCommission);
            model.TotalRecords = summarry.Orders.TotalCount;
            model.TotalPages = summarry.Orders.TotalPages;
            model.PageSize = summarry.Orders.PageSize;
            model.PageNumber = summarry.Orders.PageIndex + 1;
            model.HasNextPage = summarry.Orders.HasNextPage;
            model.HasPreviousPage = summarry.Orders.HasPreviousPage;

            foreach (var order in summarry.Orders)
            {
                var aom = new AffiliatedOrderModel();
                aom.CommissionPaid = order.IsCommissionPaid;
                aom.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc).ToString("MMM dd, yyyy hh:mm tt");
                aom.Id = order.Id;
                aom.CommissionPaidOn = !order.IsCommissionPaid || !order.CommissionPaidOn.HasValue ? "" :
                    _dateTimeHelper.ConvertToUserTime(order.CommissionPaidOn.Value, DateTimeKind.Utc).ToString("MMM dd, yyyy hh:mm tt");
                aom.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
                aom.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal);
                aom.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
                aom.OrderCommission = _priceFormatter.FormatPrice(order.AffiliateCommission);
                aom.CommissionPaid = order.IsCommissionPaid;
                model.Orders.Add(aom);
            }
        }

        protected void PrepareAffiliateModel(AffiliateInfoModel model, Affiliate affiliate, bool excludeProperties = false)
        {
            if (!excludeProperties)
            {
                if (affiliate != null)
                {
                    model.FirstName = affiliate.Address.FirstName;
                    model.LastName = affiliate.Address.LastName;
                    model.Email = affiliate.Address.Email;
                    model.PhoneNumber = affiliate.Address.PhoneNumber;
                    model.City = affiliate.Address.City;
                    model.Address1 = affiliate.Address.Address1;
                    model.Address2 = affiliate.Address.Address2;
                    model.Company = affiliate.Address.Company;
                    model.ZipPostalCode = affiliate.Address.ZipPostalCode;
                    model.FaxNumber = affiliate.Address.FaxNumber;
                    model.CountryId = affiliate.Address.CountryId;
                    model.StateProvinceId = affiliate.Address.StateProvinceId;

                    model.Url = affiliate.GenerateUrl(_webHelper, affiliate.AffiliateType);
                    model.FriendlyUrlName = affiliate.FriendlyUrlName;
                    model.Id = affiliate.Id;
                    model.Active = affiliate.Active;
                    model.Deleted = affiliate.Deleted;
                    model.Applied = true;
                    model.BKashNumber = affiliate.BKashNumber;
                }
                else
                {
                    var customer = _workContext.CurrentCustomer;

                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                    model.Email = customer.Email;
                    model.PhoneNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                    model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                    model.Address1 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                    model.Address2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                    model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                    model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                    model.FaxNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);
                    model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                    model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                }
            }

            model.AvailableCountries = _countryService.GetAllCountries()
                .Select(x => new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
            model.AvailableCountries.Insert(0, new SelectListItem() { Value = "0", Text = "Select country" });

            if (model.CountryId.HasValue)
            {
                model.AvailableStates = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId.Value)
                    .Select(x => new SelectListItem()
                    {
                        Value = x.Id.ToString(),
                        Text = x.Name
                    }).ToList();
            }
        }

        #endregion

        #region Methods

        // GET: Affiliate
        public ActionResult Index()
        {
            if (!_customerSettings.EnableAffiliate)
                return RedirectToRoute("HomePage");

            return RedirectToAction("Info");
        }

        public ActionResult Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.EnableAffiliate)
                return RedirectToRoute("HomePage");

            var affiliate = _affiliateService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);

            var model = new AffiliateInfoModel();
            PrepareAffiliateModel(model, affiliate);
            return View(model);
        }

        [HttpPost]
        public ActionResult Info(AffiliateInfoModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.EnableAffiliate)
                return RedirectToRoute("HomePage");

            var affiliate = _affiliateService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);

            if (ModelState.IsValid)
            {
                if (affiliate != null)
                {
                    if (affiliate.Deleted)
                        ErrorNotification("Your affiliate account is deleted.");

                    affiliate.FriendlyUrlName = affiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                    affiliate.BKashNumber = model.BKashNumber;
                    affiliate.Address.FirstName = model.FirstName;
                    affiliate.Address.LastName = model.LastName;
                    affiliate.Address.Email = model.Email;
                    affiliate.Address.PhoneNumber = model.PhoneNumber;
                    affiliate.Address.City = model.City;
                    affiliate.Address.Address1 = model.Address1;
                    affiliate.Address.Address2 = model.Address2;
                    affiliate.Address.Company = model.Company;
                    affiliate.Address.CountryId = model.CountryId;
                    affiliate.Address.StateProvinceId = model.StateProvinceId;
                    affiliate.Address.ZipPostalCode = model.ZipPostalCode;
                    affiliate.Address.FaxNumber = model.FaxNumber;

                    _affiliateService.UpdateAffiliate(affiliate);

                    _workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                }
                else
                {
                    var newAffiliate = new Affiliate();
                    newAffiliate.FriendlyUrlName = newAffiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                    newAffiliate.BKashNumber = model.BKashNumber;
                    newAffiliate.CustomerId = _workContext.CurrentCustomer.Id;

                    var address = new Address();
                    address.FirstName = model.FirstName;
                    address.LastName = model.LastName;
                    address.Email = model.Email;
                    address.PhoneNumber = model.PhoneNumber;
                    address.City = model.City;
                    address.Address1 = model.Address1;
                    address.Address2 = model.Address2;
                    address.Company = model.Company;
                    address.CountryId = model.CountryId;
                    address.StateProvinceId = model.StateProvinceId;
                    address.ZipPostalCode = model.ZipPostalCode;
                    address.FaxNumber = model.FaxNumber;
                    address.CreatedOnUtc = DateTime.UtcNow;
                    newAffiliate.Address = address;

                    _affiliateService.InsertAffiliate(newAffiliate);

                    _workContext.CurrentCustomer.AffiliateId = newAffiliate.Id;
                }
                _customerService.UpdateCustomer(_workContext.CurrentCustomer);

                return RedirectToRoute("CustomerAffiliateInfo");
            }
            PrepareAffiliateModel(model, affiliate, true);

            return View(model);
        }

        public ActionResult Orders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.EnableAffiliate)
                return RedirectToRoute("HomePage");

            var affiliate = _affiliateService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);
            if (affiliate == null || affiliate.Deleted)
                return RedirectToRoute("HomePage");

            var model = new AffiliatedOrderListModel();
            model.Active = affiliate.Active;
            model.PageSize = _customerSettings.AffiliatedOrdersPageSize;

            return View(model);
        }

        public ActionResult OrderList(int pageNumber = 1)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return new HttpUnauthorizedResult();

            if (!_customerSettings.EnableAffiliate)
                return Json(new { status = false });

            var affiliate = _affiliateService.GetAffiliateByCustomerId(_workContext.CurrentCustomer.Id);
            if (affiliate == null || affiliate.Deleted)
                return Json(new { status = false });

            pageNumber = pageNumber < 1 ? 1 : pageNumber;

            var pageSize = _customerSettings.AffiliatedOrdersPageSize > 0 ? _customerSettings.AffiliatedOrdersPageSize : 15;
            var summarry = _orderService.GetAffiliatedOrdersSummary(affiliateId: affiliate.Id,
                pageIndex: pageNumber - 1,
                pageSize: pageSize);

            var model = new AffiliatedOrderListModel();
            PrepareOrderListModel(model, summarry);

            var html = this.RenderPartialViewToString("_OrderList", model);

            return Json(new { status = true, html = html }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
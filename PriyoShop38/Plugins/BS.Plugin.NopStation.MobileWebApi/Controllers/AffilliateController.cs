using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Core.Domain.Localization;
using Nop.Web.Models.Media;
using Nop.Core.Domain.Tax;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using Newtonsoft.Json;
using BS.Plugin.NopStation.MobileWebApi.Models;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models.HomePage;
using Newtonsoft.Json.Linq;
using Nop.Core.Domain.Affiliates;
using Nop.Plugin.Widgets.AlgoliaSearch.Factories;
using Nop.Plugin.Widgets.AlgoliaSearch.Models;
using VendorModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorModel;
using VendorNavigationModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorNavigationModel;
using VendorBriefInfoModel = BS.Plugin.NopStation.MobileWebApi.Models.Vendor.VendorBriefInfoModel;
using Nop.Plugin.Widgets.AlgoliaSearch;
using Nop.Services.Affiliates;
using Nop.Services.Helpers;
using Nop.Web.Models.Affiliates;
using Nop.Web.Models.Catalog;
using CatalogPagingFilteringModel = BS.Plugin.NopStation.MobileWebApi.Models.Catalog.CatalogPagingFilteringModel;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class AffilliateController : WebApiController
    {
        #region Fields

        private int pageSize = 6;

        private readonly IAffiliateService _affiliateService;
        private readonly IOrderService _orderService;
        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly ICustomerService _customerService;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ILocalizationService _localizationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IEventBannerService _eventBannerService;

        #endregion

        #region Ctor

        public AffilliateController(ICategoryService categoryService,
            IAffiliateService affiliateService,
            IWorkContext workContext,
            IOrderService orderService,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ICurrencyService currencyService,
            CatalogSettings catalogSettings,
            IPriceFormatter priceFormatter,
            ILocalizationService localizationService,
            IDateTimeHelper dateTimeHelper,
            IEventBannerService eventBannerService, 
            ICustomerService customerService)
        {
            this._categoryService = categoryService;
            this._affiliateService = affiliateService;
            this._workContext = workContext;
            this._orderService = orderService;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._catalogSettings = catalogSettings;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._dateTimeHelper = dateTimeHelper;
            this._eventBannerService = eventBannerService;
            this._customerService = customerService;
        }

        #endregion

        #region Utility


        public static string GetRootUrl()
        {
            var tmpURi = HttpContext.Current.Request.Url;
            var tmpPort = tmpURi.Port > 0 && tmpURi.Port != 80 ? ":" + tmpURi.Port : "";
            var rootUrl = tmpURi.Scheme + "://" + tmpURi.Host + tmpPort + "/";
            return rootUrl;
        }

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
                aom.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc)
                    .ToString("MMM dd, yyyy hh:mm tt");
                aom.Id = order.Id;
                aom.CommissionPaidOn = !order.IsCommissionPaid || !order.CommissionPaidOn.HasValue
                    ? ""
                    : _dateTimeHelper.ConvertToUserTime(order.CommissionPaidOn.Value, DateTimeKind.Utc)
                        .ToString("MMM dd, yyyy hh:mm tt");
                aom.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
                aom.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal);
                aom.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
                aom.OrderCommission = _priceFormatter.FormatPrice(order.AffiliateCommission);
                aom.CommissionPaid = order.IsCommissionPaid;
                model.Orders.Add(aom);
            }
        }

        protected void PrepareAffiliateModel(AffiliateModel model, Affiliate affiliate, bool excludeProperties = false)
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

        }

        #endregion

        #region Action Method

        #region Affiliate

        [Route("api/affilliate/orders")]
        [HttpPost]
        public IHttpActionResult OrderList()
        {
            var result = new GeneralResponseModel<AffiliatedOrderListModel>();

            int pageNumber = 1;
            int pageSize = 15;
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["pageNumber"]))
            {
                pageNumber = Convert.ToInt32(HttpContext.Current.Request["pageNumber"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["pageSize"]))
            {
                pageSize = Convert.ToInt32(HttpContext.Current.Request["pageSize"]);
            }

            if (!string.IsNullOrEmpty(HttpContext.Current.Request["customerId"]))
            {
                int customerId = Convert.ToInt32(HttpContext.Current.Request["customerId"]);
                var affiliate = _affiliateService.GetAffiliateByCustomerId(customerId);
                if (affiliate == null || affiliate.Deleted)
                {
                    result.StatusCode = 400;
                    result.SuccessMessage = "Not Affilliated";
                    return Ok(result);
                }

                pageNumber = pageNumber < 1 ? 1 : pageNumber;
                pageSize = pageSize < 15 ? 15 : pageSize;

                var summarry = _orderService.GetAffiliatedOrdersSummary(affiliateId: affiliate.Id,
                    pageIndex: pageNumber - 1,
                    pageSize: pageSize);

                var model = new AffiliatedOrderListModel();
                PrepareOrderListModel(model, summarry);
                result.Data = model;
            }
            else
            {
                result.StatusCode = 400;
                return Ok(result);
            }

            return Ok(result);
        }

        #endregion


        #region Create Affiliate

        [Route("api/affilliate/addNewAffiliate")]
        [HttpPost]
        public IHttpActionResult AddNewAffiliate(AffiliateModel affiliateModel)
        {
            //            AffiliateModel model = JsonConvert.DeserializeObject<AffiliateModel>(value.ToString());
//            var model = new AffiliateModel();
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["CustomerId"]))
//            {
//                model.CustomerId = Convert.ToInt32(HttpContext.Current.Request["CustomerId"]);
//            }
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["friendlyUrlName"]))
//            {
//                model.FriendlyUrlName = HttpContext.Current.Request["friendlyUrlName"].ToString();
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["firstName"]))
//            {
//                model.FirstName = HttpContext.Current.Request["firstName"].ToString();
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["lastName"]))
//            {
//                model.LastName = HttpContext.Current.Request["lastName"].ToString();
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["email"]))
//            {
//                model.Email = HttpContext.Current.Request["email"].ToString();
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["countryName"]))
//            {
//                model.CountryName = HttpContext.Current.Request["countryName"].ToString();
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["countryId"]))
//            {
//                model.CountryId = Convert.ToInt32(HttpContext.Current.Request["countryId"]);
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["stateProvinceId"]))
//            {
//                model.StateProvinceId = Convert.ToInt32(HttpContext.Current.Request["stateProvinceId"]);
//            }
//
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["address1"]))
//            {
//                model.Address1 = HttpContext.Current.Request["address1"].ToString();
//            }
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["address2"]))
//            {
//                model.Address2 = HttpContext.Current.Request["address2"].ToString();
//            }
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["phoneNumber"]))
//            {
//                model.PhoneNumber = HttpContext.Current.Request["phoneNumber"].ToString();
//            }
//            if (!string.IsNullOrEmpty(HttpContext.Current.Request["BKashNumber"]))
//            {
//                model.BKashNumber = HttpContext.Current.Request["BKashNumber"].ToString();
//            }

            var model = affiliateModel;

            var result = new GeneralResponseModel<AffiliateModel>();
            if (model.CustomerId == 0)
            {
                result.StatusCode = 400;
                return Ok(result);
            }
            var customer = _customerService.GetCustomerById(model.CustomerId);
            var affiliate = _affiliateService.GetAffiliateByCustomerId(model.CustomerId);
            if (affiliate != null)
            {
                if (affiliate.Deleted)
                {
                    result.StatusCode = 400;
                    return Ok(result);
                }

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
                customer.AffiliateId = affiliate.Id;
                
            }
            else
            {
                var newAffiliate = new Affiliate();
                newAffiliate.FriendlyUrlName = newAffiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                newAffiliate.BKashNumber = model.BKashNumber;
                newAffiliate.CustomerId = model.CustomerId;

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
                customer.AffiliateId = newAffiliate.Id;
            }

            _customerService.UpdateCustomer(customer);
            PrepareAffiliateModel(model, affiliate);

            result.Data = model;
            return Ok(result);
        }

        #endregion


        #endregion

    }
}

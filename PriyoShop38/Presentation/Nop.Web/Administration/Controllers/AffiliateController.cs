using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Affiliates;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Shipping;
using Nop.Services;
using Nop.Services.Affiliates;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Directory;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class AffiliateController : BaseAdminController
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWebHelper _webHelper;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IAffiliateService _affiliateService;
        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ICategoryService _categoryService;
        private readonly IVendorService _vendorService;

        #endregion

        #region Constructors

        public AffiliateController(ILocalizationService localizationService,
            IWorkContext workContext, IDateTimeHelper dateTimeHelper, IWebHelper webHelper,
            ICountryService countryService, IStateProvinceService stateProvinceService,
            IPriceFormatter priceFormatter, IAffiliateService affiliateService,
            ICustomerService customerService, IOrderService orderService,
            IPermissionService permissionService, ICategoryService categoryService,
            IVendorService vendorService)
        {
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._dateTimeHelper = dateTimeHelper;
            this._webHelper = webHelper;
            this._countryService = countryService;
            this._stateProvinceService = stateProvinceService;
            this._priceFormatter = priceFormatter;
            this._affiliateService = affiliateService;
            this._customerService = customerService;
            this._orderService = orderService;
            this._permissionService = permissionService;
            this._categoryService = categoryService;
            this._vendorService = vendorService;
        }

        #endregion

        #region Utilities

        protected void UpdateAffiliateTypeJson()
        {
            var affiliateTypes = _affiliateService.GetAllAffiliateTypes();
            var listModel = affiliateTypes.Select(x => new AffiliateTypeAttributeModel()
            {
                IdUrlParameter = x.IdUrlParameter,
                NameUrlParameter = x.NameUrlParameter
            }).ToList();

            var json = JsonConvert.SerializeObject(listModel);

            var nfilePath = CommonHelper.MapPath("~/ApiJson/affiliate-type-json.json");
            if (!System.IO.File.Exists(nfilePath))
                System.IO.File.Create(nfilePath).Dispose();
            System.IO.File.WriteAllText(nfilePath, json);

            nfilePath = CommonHelper.MapPath("~/ApiJson/affiliate-type-backup-json.json");
            if (!System.IO.File.Exists(nfilePath))
                System.IO.File.Create(nfilePath).Dispose();
            System.IO.File.WriteAllText(nfilePath, json);
        }

        [NonAction]
        protected virtual void PrepareAffiliateModel(AffiliateModel model, Affiliate affiliate, bool excludeProperties,
            bool prepareEntireAddressModel = true)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (affiliate != null)
            {
                model.Id = affiliate.Id;
                model.Url = affiliate.GenerateUrl(_webHelper, affiliate.AffiliateType);
                if (!excludeProperties)
                {
                    model.AdminComment = affiliate.AdminComment;
                    model.FriendlyUrlName = affiliate.FriendlyUrlName;
                    model.Active = affiliate.Active;
                    model.Address = affiliate.Address.ToModel();
                    model.AffiliateTypeId = affiliate.AffiliateTypeId.HasValue ? affiliate.AffiliateTypeId.Value : 0;
                    model.BKashNumber = affiliate.BKashNumber;

                    var affiliateTypes = _affiliateService.GetAllAffiliateTypes(showHidden: true);
                    model.AvailableAffiliateTypes = affiliateTypes.Select(x => new SelectListItem()
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    }).ToList();

                    model.AvailableAffiliateTypes.Insert(0, new SelectListItem() { Value = "0", Text = "Select affiliate type" });
                }
                if (affiliate.CustomerId > 0)
                {
                    var customer = _customerService.GetCustomerById(affiliate.CustomerId);
                    model.CustomerName = customer == null ? "" : string.IsNullOrWhiteSpace(customer.GetFullName()) ? customer.Email : customer.GetFullName() + " - " + customer.Email;
                    model.CustomerId = affiliate.CustomerId;
                }
            }

            if (prepareEntireAddressModel)
            {
                model.Address.FirstNameEnabled = true;
                model.Address.FirstNameRequired = true;
                model.Address.LastNameEnabled = true;
                model.Address.LastNameRequired = true;
                model.Address.EmailEnabled = true;
                model.Address.EmailRequired = true;
                model.Address.CompanyEnabled = true;
                model.Address.CountryEnabled = true;
                model.Address.StateProvinceEnabled = true;
                model.Address.CityEnabled = true;
                model.Address.CityRequired = true;
                model.Address.StreetAddressEnabled = true;
                model.Address.StreetAddressRequired = true;
                model.Address.StreetAddress2Enabled = true;
                model.Address.ZipPostalCodeEnabled = true;
                model.Address.ZipPostalCodeRequired = true;
                model.Address.PhoneEnabled = true;
                model.Address.PhoneRequired = true;
                model.Address.FaxEnabled = true;

                //address
                model.Address.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries(showHidden: true))
                    model.Address.AvailableCountries.Add(new SelectListItem { Text = c.Name, Value = c.Id.ToString(), Selected = (affiliate != null && c.Id == affiliate.Address.CountryId) });

                var states = model.Address.CountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(model.Address.CountryId.Value, showHidden: true).ToList() : new List<StateProvince>();
                if (states.Any())
                {
                    foreach (var s in states)
                        model.Address.AvailableStates.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString(), Selected = (affiliate != null && s.Id == affiliate.Address.StateProvinceId) });
                }
                else
                    model.Address.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Address.OtherNonUS"), Value = "0" });
            }
        }

        #endregion

        #region Methods

        #region List / create / update / delete

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var model = new AffiliateListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, AffiliateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliates = _affiliateService.GetAllAffiliates(model.SearchFriendlyUrlName,
                model.SearchFirstName, model.SearchLastName,
                model.LoadOnlyWithOrders, model.OrdersCreatedFromUtc, model.OrdersCreatedToUtc,
                command.Page - 1, command.PageSize, true);

            var gridModel = new DataSourceResult
            {
                Data = affiliates.Select(x =>
                {
                    var m = new AffiliateModel();
                    PrepareAffiliateModel(m, x, false, false);
                    return m;
                }),
                Total = affiliates.TotalCount,
            };
            return Json(gridModel);
        }

        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var model = new AffiliateModel();
            PrepareAffiliateModel(model, null, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Create(AffiliateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var affiliate = new Affiliate();

                affiliate.Active = model.Active;
                affiliate.AdminComment = model.AdminComment;
                affiliate.BKashNumber = model.BKashNumber;
                //validate friendly URL name
                var friendlyUrlName = affiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.Address = model.Address.ToEntity();
                affiliate.Address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (affiliate.Address.CountryId == 0)
                    affiliate.Address.CountryId = null;
                if (affiliate.Address.StateProvinceId == 0)
                    affiliate.Address.StateProvinceId = null;
                if (model.AffiliateTypeId > 0)
                    affiliate.AffiliateTypeId = model.AffiliateTypeId;

                _affiliateService.InsertAffiliate(affiliate);

                affiliate.CustomerId = model.CustomerId;
                SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = affiliate.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareAffiliateModel(model, null, true);
            return View(model);

        }


        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(id);
            if (affiliate == null || affiliate.Deleted)
                //No affiliate found with the specified id
                return RedirectToAction("List");

            var model = new AffiliateModel();
            PrepareAffiliateModel(model, affiliate, false);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired(FormValueRequirement.StartsWith, "save")]
        public ActionResult Edit(AffiliateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(model.Id);
            if (affiliate == null || affiliate.Deleted)
                //No affiliate found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                affiliate.Active = model.Active;
                affiliate.AdminComment = model.AdminComment;
                //validate friendly URL name
                var friendlyUrlName = affiliate.ValidateFriendlyUrlName(model.FriendlyUrlName);
                affiliate.FriendlyUrlName = friendlyUrlName;
                affiliate.BKashNumber = model.BKashNumber;
                affiliate.Address = model.Address.ToEntity(affiliate.Address);
                //some validation
                if (affiliate.Address.CountryId == 0)
                    affiliate.Address.CountryId = null;
                if (affiliate.Address.StateProvinceId == 0)
                    affiliate.Address.StateProvinceId = null;
                if (model.AffiliateTypeId > 0)
                    affiliate.AffiliateTypeId = model.AffiliateTypeId;

                _affiliateService.UpdateAffiliate(affiliate);
                SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Updated"));
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new {id = affiliate.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareAffiliateModel(model, affiliate, true);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveAffiliateCustomer")]
        public ActionResult ChangeAffiliateCustomer(AffiliateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(model.Id);
            if (affiliate == null || affiliate.Deleted)
                //No affiliate found with the specified id
                return RedirectToAction("List");

            var customer = _customerService.GetCustomerById(model.CustomerId);

            if (customer == null || customer.Deleted)
            {
                ErrorNotification("No customer found with the specified id");
                return RedirectToAction("Edit", new { id = affiliate.Id });
            }

            var customerAffiliate = _affiliateService.GetAffiliateByCustomerId(model.CustomerId);
            if (customerAffiliate != null && customerAffiliate.Id != affiliate.Id)
            {
                ErrorNotification("Specified customer is already mapped with another affiliate account");
                return RedirectToAction("Edit", new { id = affiliate.Id });
            }

            if (customerAffiliate == null)
            {
                affiliate.CustomerId = model.CustomerId;
                _affiliateService.UpdateAffiliate(affiliate);
            }

            return RedirectToAction("Edit", new { id = affiliate.Id });
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(id);
            if (affiliate == null)
                //No affiliate found with the specified id
                return RedirectToAction("List");

            _affiliateService.DeleteAffiliate(affiliate);
            SuccessNotification(_localizationService.GetResource("Admin.Affiliates.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Affiliate types

        public ActionResult TypeList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var model = new AffiliateTypeListModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult TypeList(DataSourceRequest command, AffiliateTypeListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliateTypes = _affiliateService.GetAllAffiliateTypes(searchKeyword: model.SearchKeyword,
                pageIndex: command.Page - 1, pageSize: command.PageSize, showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = affiliateTypes.Select(x =>
                {
                    var m = new AffiliateTypeModel()
                    {
                        Active = x.Active,
                        Id = x.Id,
                        IdUrlParameter = x.IdUrlParameter,
                        Name = x.Name,
                        NameUrlParameter = x.NameUrlParameter
                    };
                    return m;
                }),
                Total = affiliateTypes.TotalCount,
            };
            return Json(gridModel);
        }

        //create
        public ActionResult TypeCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var model = new AffiliateTypeModel();
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult TypeCreate(AffiliateTypeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var types = _affiliateService.GetAllAffiliateTypes(model.NameUrlParameter.Trim(), model.IdUrlParameter.Trim());
                if (types.Count() == 0)
                {
                    var affiliateType = new AffiliateType();

                    affiliateType.Active = model.Active;
                    affiliateType.IdUrlParameter = model.IdUrlParameter.Trim();
                    affiliateType.Name = model.Name;
                    affiliateType.NameUrlParameter = model.NameUrlParameter.Trim();

                    _affiliateService.InsertAffiliateType(affiliateType);

                    UpdateAffiliateTypeJson();

                    SuccessNotification(_localizationService.GetResource("Admin.AffiliateTypes.Added"));
                    return continueEditing ? RedirectToAction("TypeEdit", new { id = affiliateType.Id }) : RedirectToAction("TypeList");
                }
                else
                {
                    if (types.Any(x => x.IdUrlParameter.Equals(model.IdUrlParameter, StringComparison.CurrentCultureIgnoreCase)))
                        ErrorNotification("Id url parameter is already added.");
                    else
                        ErrorNotification("Name url parameter is already added.");
                }
            }
            
            return View(model);
        }


        //edit
        public ActionResult TypeEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliateType = _affiliateService.GetAffiliateTypeById(id);
            if (affiliateType == null)
                return RedirectToAction("TypeList");

            var model = new AffiliateTypeModel()
            {
                Active = affiliateType.Active,
                Id = affiliateType.Id,
                IdUrlParameter = affiliateType.IdUrlParameter,
                Name = affiliateType.Name,
                NameUrlParameter = affiliateType.NameUrlParameter
            };
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired(FormValueRequirement.StartsWith, "save")]
        public ActionResult TypeEdit(AffiliateTypeModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliateType = _affiliateService.GetAffiliateTypeById(model.Id);
            if (affiliateType == null)
                return RedirectToAction("TypeList");

            if (ModelState.IsValid)
            {
                var types = _affiliateService.GetAllAffiliateTypes(model.NameUrlParameter.Trim(), model.IdUrlParameter.Trim()).Where(x => x.Id != model.Id);
                if (types.Count() == 0)
                {
                    affiliateType.Active = model.Active;
                    affiliateType.IdUrlParameter = model.IdUrlParameter.Trim();
                    affiliateType.Name = model.Name;
                    affiliateType.NameUrlParameter = model.NameUrlParameter.Trim();

                    _affiliateService.UpdateAffiliateType(affiliateType);

                    UpdateAffiliateTypeJson();

                    SuccessNotification(_localizationService.GetResource("Admin.AffiliateTypes.Updated"));
                    if (continueEditing)
                    {
                        //selected tab
                        SaveSelectedTabName();

                        return RedirectToAction("TypeEdit", new { id = affiliateType.Id });
                    }
                    return RedirectToAction("TypeList");
                }
                else
                {
                    if (types.Any(x => x.IdUrlParameter.Equals(model.IdUrlParameter, StringComparison.CurrentCultureIgnoreCase)))
                        ErrorNotification("Id url parameter is already added.");
                    else
                        ErrorNotification("Name url parameter is already added.");
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult TypeAffiliateList(DataSourceRequest command, int affiliateTypeId)
        {
            var affiliateType = _affiliateService.GetAffiliateTypeById(affiliateTypeId);
            if (affiliateType == null)
                return AccessDeniedView();

            var affiliates = _affiliateService.GetAllAffiliates(pageIndex: command.Page - 1, pageSize: command.PageSize, showHidden: true, affiliateTypeId: affiliateTypeId);

            var gridModel = new DataSourceResult
            {
                Data = affiliates.Select(x =>
                {
                    var m = new AffiliateModel();
                    PrepareAffiliateModel(m, x, false, false);
                    return m;
                }),
                Total = affiliates.TotalCount,
            };
            return Json(gridModel);
        }

        //delete
        [HttpPost]
        public ActionResult TypeDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliateType = _affiliateService.GetAffiliateTypeById(id);
            if (affiliateType == null)
                //No affiliate found with the specified id
                return RedirectToAction("TypeList");

            _affiliateService.DeleteAffiliateType(affiliateType);

            UpdateAffiliateTypeJson();

            SuccessNotification(_localizationService.GetResource("Admin.AffiliateTypes.Deleted"));
            return RedirectToAction("TypeList");
        }

        #endregion

        #region Affiliated orders

        [ChildActionOnly]
        public ActionResult AffiliatedOrderList(int affiliateId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return Content("");

            if (affiliateId == 0)
                throw new Exception("Affliate ID cannot be 0");

            var model = new AffiliatedOrderListModel();
            model.AffliateId = affiliateId;

            //order statuses
            model.AvailableOrderStatuses = OrderStatus.Pending.ToSelectList(false).ToList();
            model.AvailableOrderStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            
            //payment statuses
            model.AvailablePaymentStatuses = PaymentStatus.Pending.ToSelectList(false).ToList();
            model.AvailablePaymentStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            
            //shipping statuses
            model.AvailableShippingStatuses = ShippingStatus.NotYetShipped.ToSelectList(false).ToList();
            model.AvailableShippingStatuses.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //commission payment status
            model.AvailableShippingStatuses.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.AvailableShippingStatuses.Add(new SelectListItem { Text = "Paid only", Value = "1" });
            model.AvailableShippingStatuses.Add(new SelectListItem { Text = "Unpaid only", Value = "2" });

            return PartialView(model);
        }
        [HttpPost]
        public ActionResult AffiliatedOrderListGrid(DataSourceRequest command, AffiliatedOrderListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(model.AffliateId);
            if (affiliate == null)
                throw new ArgumentException("No affiliate found with the specified id");

            DateTime? startDateValue = (model.StartDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.StartDate.Value, _dateTimeHelper.CurrentTimeZone);

            DateTime? endDateValue = (model.EndDate == null) ? null
                            : (DateTime?)_dateTimeHelper.ConvertToUtcTime(model.EndDate.Value, _dateTimeHelper.CurrentTimeZone).AddDays(1);

            var orderStatusIds = model.OrderStatusId > 0 ? new List<int>() { model.OrderStatusId } : null;
            var paymentStatusIds = model.PaymentStatusId > 0 ? new List<int>() { model.PaymentStatusId } : null;
            var shippingStatusIds = model.ShippingStatusId > 0 ? new List<int>() { model.ShippingStatusId } : null;

            bool? affiliateCommissionPaid = null;
            if (model.CommisionPaymentStatusId == 1)
                affiliateCommissionPaid = true;
            else if (model.CommisionPaymentStatusId == 2)
                affiliateCommissionPaid = false;

            var orders = _orderService.SearchOrders(
                createdFromUtc: startDateValue,
                createdToUtc: endDateValue,
                osIds: orderStatusIds,
                psIds: paymentStatusIds,
                ssIds: shippingStatusIds,
                affiliateId: affiliate.Id,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                acps: affiliateCommissionPaid);

            var gridModel = new DataSourceResult
            {
                Data = orders.Select(order =>
                    {
                        var orderModel = new AffiliateModel.AffiliatedOrderModel();
                        orderModel.Id = order.Id;
                        orderModel.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.OrderStatusId = order.OrderStatusId;
                        orderModel.PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
                        orderModel.OrderTotal = _priceFormatter.FormatPrice(order.OrderTotal, true, false);
                        orderModel.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);

                        #region BS-23
                        orderModel.AffiliateCommission = _priceFormatter.FormatPrice(order.AffiliateCommission, true, false);
                        orderModel.AffiliateCommissionValue = order.AffiliateCommission;
                        orderModel.IsCommissionPaid = order.IsCommissionPaid;
                        orderModel.CommissionPaidOn = !order.CommissionPaidOn.HasValue ? (DateTime?)null:
                            _dateTimeHelper.ConvertToUserTime(order.CommissionPaidOn.Value, DateTimeKind.Utc);
                        #endregion

                        return orderModel;
                    }),
                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Affiliated customers

        [HttpPost]
        public ActionResult AffiliatedCustomerList(int affiliateId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageAffiliates))
                return AccessDeniedView();

            var affiliate = _affiliateService.GetAffiliateById(affiliateId);
            if (affiliate == null)
                throw new ArgumentException("No affiliate found with the specified id");
            
            var customers = _customerService.GetAllCustomers(
                affiliateId: affiliate.Id,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = customers.Select(customer =>
                    {
                        var customerModel = new AffiliateModel.AffiliatedCustomerModel();
                        customerModel.Id = customer.Id;
                        customerModel.Name = customer.Email;
                        return customerModel;
                    }),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Commission

        public ActionResult AffiliateCommission()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AffiliateCommissionList(DataSourceRequest command, AffiliateListModel model)
        {
            var affiliates = _affiliateService.GetAllAffiliates(firstName: model.SearchFirstName, 
                lastName: model.SearchLastName,
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize, 
                showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = affiliates.Select(x =>
                {
                    var customer = _customerService.GetCustomerById(x.CustomerId);
                    var m = new AffiliateCommissionModel()
                    {
                        Active = x.Active,
                        CommissionRate = x.CommissionRate,
                        Id = x.Id,
                        Name = x.GetFullName(),
                        CustomerId = customer != null ? x.CustomerId : 0,
                        CustomerName = customer != null ? customer.Email : ""
                    };
                    
                    return m;
                }),
                Total = affiliates.TotalCount,
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult AffiliateCommissionUpdate(AffiliateCommissionModel model)
        {
            var affiliate = _affiliateService.GetAffiliateById(model.Id);
            if (affiliate == null || affiliate.Deleted)
                return AccessDeniedView();

            affiliate.CommissionRate = model.CommissionRate;
            _affiliateService.UpdateAffiliate(affiliate);

            return new NullJsonResult();
        }

        public ActionResult CategoryCommission()
        {
            return View(new CategoryCommissionListModel());
        }

        [HttpPost]
        public ActionResult CategoryCommissionList(DataSourceRequest command, CategoryCommissionListModel model)
        {
            var categories = _categoryService.GetAllCategories(categoryName: model.Name, 
                pageSize: command.PageSize, 
                pageIndex: command.Page - 1, 
                showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var m = new CategoryCommissionModel()
                    {
                        CommissionRate = x.AffiliateCommissionRate,
                        Id = x.Id,
                        Name = x.GetFormattedBreadCrumb(_categoryService),
                        Published = x.Published
                    };
                    return m;
                }),
                Total = categories.TotalCount,
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CategoryCommissionUpdate(CategoryCommissionModel model)
        {
            var category = _categoryService.GetCategoryById(model.Id);
            if (category == null || category.Deleted)
                return AccessDeniedView();

            category.AffiliateCommissionRate = model.CommissionRate;
            _categoryService.UpdateCategory(category);

            return new NullJsonResult();
        }

        public ActionResult VendorCommission()
        {
            return View(new VendorCommissionListModel());
        }

        [HttpPost]
        public ActionResult VendorCommissionList(DataSourceRequest command, VendorCommissionListModel model)
        {
            var vendors = _vendorService.GetAllVendors(name: model.Name,
                pageSize: command.PageSize,
                pageIndex: command.Page - 1,
                showHidden: true);

            var gridModel = new DataSourceResult
            {
                Data = vendors.Select(x =>
                {
                    var m = new VendorCommissionModel()
                    {
                        CommissionRate = x.AffiliateCommissionRate,
                        Id = x.Id,
                        Name = x.Name,
                        Active = x.Active
                    };
                    return m;
                }),
                Total = vendors.TotalCount,
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult VendorCommissionUpdate(VendorCommissionModel model) 
        {
            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                return AccessDeniedView();

            vendor.AffiliateCommissionRate = model.CommissionRate;
            _vendorService.UpdateVendor(vendor);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}

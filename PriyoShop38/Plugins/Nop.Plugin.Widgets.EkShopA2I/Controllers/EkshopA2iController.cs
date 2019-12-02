using Nop.Admin.Controllers;
using Nop.Plugin.Widgets.EkShopA2I.Extensions;
using Nop.Plugin.Widgets.EkShopA2I.Models;
using Nop.Plugin.Widgets.EkShopA2I.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Vendors;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.EkShopA2I.Controllers
{
    public class EkShopA2iController : BaseAdminController
    {
        #region Fields

        private readonly IConfigureService _esConfigureService;
        private readonly IVendorService _vendorService;
        private readonly ICategoryService _categoryService;
        private readonly ISettingService _settingService;
        private readonly ILogger _logger;
        private readonly IPermissionService _permissionService;
        private readonly EkshopSettings _ekShopA2ISettings;
        private readonly ICommissionRateService _commissionRateService;
        private readonly IEkshopOrderService _ekshopOrderService;

        #endregion

        #region Ctor

        public EkShopA2iController(IConfigureService esConfigureService,
            IVendorService vendorService,
            ICategoryService categoryService,
            ISettingService settingService,
            ILogger logger,
            IPermissionService permissionService,
            EkshopSettings ekShopA2ISettings,
            ICommissionRateService commissionRateService,
            IEkshopOrderService ekshopOrderService)
        {
            this._esConfigureService = esConfigureService;
            this._vendorService = vendorService;
            this._categoryService = categoryService;
            this._settingService = settingService;
            this._logger = logger;
            this._permissionService = permissionService;
            this._ekShopA2ISettings = ekShopA2ISettings;
            this._commissionRateService = commissionRateService;
            this._ekshopOrderService = ekshopOrderService;
        }

        #endregion

        #region Configure

        [ChildActionOnly]
        public ActionResult Settings()
        {
            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/Settings.cshtml");
        }

        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopConfigure))
                return AccessDeniedView();
            
            var model = new EsConfigureModel()
            {
                AccessToken = _ekShopA2ISettings.AccessToken,
                ApiKey = _ekShopA2ISettings.ApiKey,
                Authorization = _ekShopA2ISettings.Authorization,
                UdcCommission = _ekShopA2ISettings.UdcCommission,
                ShowUdcCommissionOnProductBox = _ekShopA2ISettings.ShowUdcCommissionOnProductBox,
                ShowUdcCommissionOnProductDetails = _ekShopA2ISettings.ShowUdcCommissionOnProductDetails,
                EnableLog = _ekShopA2ISettings.EnableLog,
                MinimumCartValue = _ekShopA2ISettings.MinimumCartValue,
                EnableFreeShipping = _ekShopA2ISettings.EnableFreeShipping,
                ShippingCharge = _ekShopA2ISettings.ShippingCharge
            };

            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/Configure.cshtml", model);
        }

        [HttpPost]
        public ActionResult Configure(EsConfigureModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopConfigure))
                return AccessDeniedView();

            var oldApiKey = _ekShopA2ISettings.ApiKey;

            _ekShopA2ISettings.AccessToken = model.AccessToken;
            _ekShopA2ISettings.ApiKey = model.ApiKey;
            _ekShopA2ISettings.Authorization = model.Authorization;
            _ekShopA2ISettings.UdcCommission = model.UdcCommission;
            _ekShopA2ISettings.ShowUdcCommissionOnProductDetails = model.ShowUdcCommissionOnProductDetails;
            _ekShopA2ISettings.ShowUdcCommissionOnProductBox = model.ShowUdcCommissionOnProductBox;
            _ekShopA2ISettings.EnableLog = model.EnableLog;

            _ekShopA2ISettings.EnableFreeShipping = model.EnableFreeShipping;
            _ekShopA2ISettings.MinimumCartValue = model.MinimumCartValue;
            _ekShopA2ISettings.ShippingCharge = model.ShippingCharge;

            _settingService.SaveSetting(_ekShopA2ISettings);

            _esConfigureService.UpdateA2iCustomer(_ekShopA2ISettings.ApiKey, oldApiKey);

            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/Configure.cshtml", model);
        }

        public ActionResult RestrictVendor()
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopManageVendor))
                return AccessDeniedView();

            var model = new VendorRestrictModel();

            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/RestrictVendor.cshtml", model);
        }

        [HttpPost]
        public ActionResult VendorList(DataSourceRequest command, VendorRestrictModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopManageVendor))
                return new NullJsonResult();

            var vendors = _vendorService.GetAllVendors(name: model.SearchVendorName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);

            var restrictedVendors = _esConfigureService.GetRestrictedVendors();

            var gridModel = new DataSourceResult()
            {
                Data = vendors.Select(x => new VendorModel()
                {
                    Active = x.Active,
                    Id = x.Id,
                    VendorName = x.Name,
                    Restricted = restrictedVendors.Contains(x.Id)
                }),
                Total = vendors.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdateVendorRestriction(VendorModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopManageVendor))
                return new NullJsonResult();

            var restrictedVendors = _esConfigureService.GetRestrictedVendors();
            if (model.Restricted && !restrictedVendors.Contains(model.Id))
            {
                restrictedVendors.Add(model.Id);
                _esConfigureService.UpdateRestrictedVendors(restrictedVendors);
            }
            else if (!model.Restricted && restrictedVendors.Contains(model.Id))
            {
                restrictedVendors.Remove(model.Id);
                _esConfigureService.UpdateRestrictedVendors(restrictedVendors);
            }

            return new NullJsonResult();
        }

        #endregion

        #region Orders

        public ActionResult OrderList()
        {
            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/OrderList.cshtml", new EkshopOrderSearchModel());
        }

        [HttpPost]
        public ActionResult OrderList(DataSourceRequest command, EkshopOrderSearchModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopViewOrders))
                return new NullJsonResult();

            var orders = _ekshopOrderService.GetEkshopOrders(model.OrderCode, 
                model.LpCode, 
                model.LpContactNumber,
                model.StartDate,
                model.EndDate, 
                command.Page - 1, 
                command.PageSize);

            var gridModel = new DataSourceResult()
            {
                Data = orders.Select(x => x.ToModel()),
                Total = orders.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult OrderDetails(int id)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopViewOrders))
                return AccessDeniedView();

            var esOrder = _ekshopOrderService.GetEkshopOrderById(id);
            if (esOrder == null)
                return RedirectToAction("OrderList");

            var model = esOrder.ToModel();

            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/Details.cshtml", model);
        }

        #endregion

        #region Commission rate

        public ActionResult CommissionRate()
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopViewCommission))
                return AccessDeniedView();

            var model = new UdcCommissionRateModel();
            model.CanManageCommission = _permissionService.Authorize(EkshopPermissionProvider.EkShopManageCommission);

            return View("~/Plugins/Widgets.EkShopA2I/Views/EkShopA2I/CommissionRate.cshtml", model);
        }

        [HttpPost]
        public ActionResult CategoryCommissionRateList(DataSourceRequest command, UdcCommissionRateModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopViewCommission))
                return new NullJsonResult();

            var categories = _categoryService.GetAllCategories(categoryName: model.SearchCategoryName, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize, 
                showHidden: true);

            var rateListModel = new List<UdcCommissionRateModel>();

            foreach (var category in categories)
            {
                var rate = _commissionRateService.GetCommissionRateByEntityId(category.Id, Domain.EntityType.Category);
                var rateModel = new UdcCommissionRateModel()
                {
                    EntityName = category.GetFormattedBreadCrumb(_categoryService),
                    EntityId = category.Id,
                    EntityActive = category.Published,
                    EntityTypeId = (int)Domain.EntityType.Category
                };

                if (rate != null)
                {
                    rateModel.CommissionRate = rate.CommissionRate;
                    rateModel.RateMappingId = rate.Id;
                    rateModel.MappedCommissionRate = true;
                }

                rateListModel.Add(rateModel);
            }

            var gridModel = new DataSourceResult()
            {
                Data = rateListModel,
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult VendorCommissionRateList(DataSourceRequest command, UdcCommissionRateModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopViewCommission))
                return new NullJsonResult();

            var vendors = _vendorService.GetAllVendors(name: model.SearchVendorName, 
                pageIndex: command.Page - 1, 
                pageSize: command.PageSize, 
                showHidden: true);

            var rateListModel = new List<UdcCommissionRateModel>();

            foreach (var vendor in vendors)
            {
                var rate = _commissionRateService.GetCommissionRateByEntityId(vendor.Id, Domain.EntityType.Vendor);
                var rateModel = new UdcCommissionRateModel()
                {
                    EntityName = vendor.Name,
                    EntityId = vendor.Id,
                    EntityActive = vendor.Active,
                    EntityTypeId = (int)Domain.EntityType.Vendor
                };

                if (rate != null)
                {
                    rateModel.CommissionRate = rate.CommissionRate;
                    rateModel.RateMappingId = rate.Id;
                    rateModel.MappedCommissionRate = true;
                }

                rateListModel.Add(rateModel);
            }

            var gridModel = new DataSourceResult()
            {
                Data = rateListModel,
                Total = vendors.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UpdateCommissionRate(UdcCommissionRateModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopManageCommission))
                return new NullJsonResult();

            var rate = _commissionRateService.GetCommissionRateByEntityId(model.EntityId, (Domain.EntityType)model.EntityTypeId);
            if (rate == null)
            {
                var newRate = new Domain.EsUdcCommissionRate()
                {
                    CommissionRate = model.CommissionRate,
                    EntityId = model.EntityId,
                    Type = (Domain.EntityType)model.EntityTypeId
                };
                _commissionRateService.InsertCommissionRate(newRate);
            }
            else
            {
                rate.CommissionRate = model.CommissionRate;
                _commissionRateService.UpdateCommissionRate(rate);
            }

            return new NullJsonResult();
        }
        
        public ActionResult DeleteCommissionRate(UdcCommissionRateModel model)
        {
            if (!_permissionService.Authorize(EkshopPermissionProvider.EkShopManageCommission))
                return AccessDeniedView();

            var rate = _commissionRateService.GetCommissionRateById(model.RateMappingId);
            if (rate != null)
                _commissionRateService.DeleteCommissionRate(rate);

            return new NullJsonResult();
        }

        #endregion
    }
}

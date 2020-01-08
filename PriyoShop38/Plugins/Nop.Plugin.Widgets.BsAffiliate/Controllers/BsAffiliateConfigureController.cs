using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Customers;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Extensions;
using Nop.Plugin.Widgets.BsAffiliate.Models;
using Nop.Plugin.Widgets.BsAffiliate.Services;
using Nop.Services.Customers;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Xml;

namespace Nop.Plugin.Widgets.BsAffiliate.Controllers
{
    public class BsAffiliateConfigureController : BasePluginController
    {
        private readonly IAffiliateConfigureService _affiliateConfigureService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IAffiliateCustomerMapService _acmService;
        private readonly IAffiliateCommissionRateService _acrService;
        private readonly IPermissionService _permissionService;
        private readonly IRepository<AffiliateType> _atRepository;
        private readonly ILogger _logger;

        public BsAffiliateConfigureController(IAffiliateConfigureService affiliateConfigureService,
            IStoreService storeService, 
            ICustomerService customerService,
            IAffiliateCustomerMapService acmService,
            IAffiliateCommissionRateService acrService,
            IPermissionService permissionService,
            IRepository<AffiliateType> atRepository,
            ILogger logger)
        {
            _affiliateConfigureService = affiliateConfigureService;
            _storeService = storeService;
            _customerService = customerService;
            _acmService = acmService;
            _acrService = acrService;
            _permissionService = permissionService;
            _atRepository = atRepository;
            _logger = logger;
        }

        [ChildActionOnly]
        public ActionResult Settings()
        {
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/Settings.cshtml");
        }

        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateConfigure))
                return AccessDeniedView();

            var model = _affiliateConfigureService.LoadSettings();
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult Configure(AffiliateConfigureModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateConfigure))
                return AccessDeniedView();

            _affiliateConfigureService.UpdateSettings(model);
            return Configure();
        }

        public ActionResult UserCommission()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewUserCommission))
                return AccessDeniedView();

            ViewBag.CanManage = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission);
            ViewBag.CanMap = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateCustomerMap);

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/UserCommission.cshtml");
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult GetCommissions(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewUserCommission))
                return Json(new EmptyResult());

            var query = _affiliateConfigureService.GetAllCommissions().AsQueryable();

            var gridModel = new DataSourceResult
            {
                Data = query.PagedForCommand(command).ToList(),
                Total = query.Count()
            };

            return Json(gridModel);
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult CommissionUpdate(AffiliateUserCommissionModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission))
                return Json(new EmptyResult());

            _affiliateConfigureService.UpdateCommission(model);
            return Json(new EmptyResult());
        }

        public ActionResult CommissionReset(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageUserCommission))
                return AccessDeniedView();

            _affiliateConfigureService.ResetCommission(id);
            return RedirectToAction("UserCommission");
        }

        public ActionResult AffiliatedOrder()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewOrder) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageOrder))
                return AccessDeniedView();

            var stores = _storeService.GetAllStores();
            var model = new AffiliatedOrderModel();

            model.AvailableStores.Add(new SelectListItem() { Text = "All", Value = "0" });
            foreach (var item in stores)
            {
                model.AvailableStores.Add(new SelectListItem()
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.CanManage = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageOrder);

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AffiliatedOrder.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult GetOrders(DataSourceRequest command, AffiliatedOrderModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewOrder) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageOrder))
                return Json(new EmptyResult());

            var gridModel = _affiliateConfigureService.GetAllOrders(command, model);

            return Json(gridModel);
        }

        public ActionResult MarkAsPaid(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageOrder))
                return AccessDeniedView();

            _affiliateConfigureService.MarkAsPaid(id);
            return RedirectToAction("AffiliatedOrder");
        }

        public ActionResult CustomerMap(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateCustomerMap))
                return AccessDeniedView();

            var model = _acmService.GetCustomerByAffiliateId(id);
            if (model == null)
                return RedirectToAction("UserCommission");

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/CustomerMap.cshtml", model);
        }

        [HttpPost]
        [AdminAntiForgery(true)]
        public ActionResult CustomerMap(AffiliateCustomerMapModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateCustomerMap))
                return AccessDeniedView();

            var result = _acmService.SaveAffiliateCustomer(model);
            if (result.Status)
            {
                SuccessNotification("Data saved successfully");
                return RedirectToAction("UserCommission");
            }
            else
            {
                ErrorNotification(result.Message);
                model.AvailableAffiliateTypes = _acmService.GetAvailableAffiliateTypes();
                return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/CustomerMap.cshtml", model);
            }
        }

        public ActionResult GetCustomers(string query)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateCustomerMap))
                return Json(new EmptyResult());

            if (string.IsNullOrWhiteSpace(query) || query.Length < 3)
                return Json(new NullJsonResult());

            var customers = _acmService.GetCustomer(query);

            return Json(customers, JsonRequestBehavior.AllowGet);
        }

        public ActionResult VendorCommission()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewVendorCommission))
                return AccessDeniedView();

            ViewBag.CanManage = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission);

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/VendorCommission.cshtml");
        }

        public ActionResult CommissionRateList(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission) &&
                !_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewVendorCommission))
                return Json(new EmptyResult());

            var data = _acrService.GetAffiliateCommissions(command);

            var model = new DataSourceResult()
            {
                Data = data,
                Total = data.TotalCount
            };
            return Json(model);
        }

        public ActionResult AddVendorCommission()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission))
                return AccessDeniedView();

            var model = new AffiliateCommissionRateModel();
            model.CategoryList = _acrService.GetCategoryList();
            model.VendorList = _acrService.GetVendorList();
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AddVendorCommission.cshtml", model);
        }

        [HttpPost]
        public ActionResult AddVendorCommission(AffiliateCommissionRateModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var response = _acrService.AddCommission(model);
                if (response.Status)
                {
                    SuccessNotification(response.Message);
                    return RedirectToAction("CommissionRate");
                }
                ErrorNotification(response.Message);
            }
            model.CategoryList = _acrService.GetCategoryList();
            model.VendorList = _acrService.GetVendorList();

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AddVendorCommission.cshtml", model);
        }

        public ActionResult EditVendorCommission(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission))
                return AccessDeniedView();

            var model = _acrService.GetCommissionRate(id);
            if (model == null)
            {
                ErrorNotification("Data not found.");
                return RedirectToAction("CommissionRate");
            }
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/EditVendorCommission.cshtml", model);
        }

        [HttpPost]
        public ActionResult EditVendorCommission(AffiliateCommissionRateModel model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                SaveResponseModel response = _acrService.EditCommission(model);
                if (response.Status)
                {
                    SuccessNotification(response.Message);
                    return RedirectToAction("VendorCommission");
                }
                ErrorNotification(response.Message);
            }
            model.CategoryList = _acrService.GetCategoryList();
            model.VendorList = _acrService.GetVendorList();
            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/EditVendorCommission.cshtml", model);
        }

        public ActionResult RateDelete(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageVendorCommission))
                return AccessDeniedView();

            _acrService.DeleteRate(id);
            return RedirectToAction("VendorCommission");
        }

        protected ActionResult AccessDeniedView()
        {
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl });
        }

        public ActionResult AffiliateType()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateViewAffiliateType))
                return AccessDeniedView();

            ViewBag.CanManage = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType);

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AffiliateType.cshtml");
        }

        [HttpPost]
        public ActionResult GetAffiliateTypes(DataSourceRequest command)
        {
            var list = new List<AffiliateTypeModels>();
            var types = _atRepository.Table;

            var result = new DataSourceResult()
            {
                Data = types.PagedForCommand(command).ToList(),
                Total = types.Count()
            };
            return Json(result);
        }

        public ActionResult AddAffiliateType()
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
                return AccessDeniedView();

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AddAffiliateType.cshtml", new AffiliateTypeModels());
        }

        [HttpPost]
        public ActionResult AddAffiliateType(AffiliateTypeModels model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
                return AccessDeniedView();

            if (ModelState.IsValid)
                if (SaveOrUpdateType(model))
                    return RedirectToAction("AffiliateType");

            ViewBag.CanManage = _permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType);

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/AddAffiliateType.cshtml", model);
        }

        public ActionResult EditAffiliateType(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
                return AccessDeniedView();

            var type = _atRepository.GetById(id);
            if(type == null)
                return AccessDeniedView();

            var model = new AffiliateTypeModels()
            {
                Active = type.Active,
                Id = type.Id,
                IdUrlParameter = type.IdUrlParameter,
                Name = type.Name,
                NameUrlParameter = type.NameUrlParameter
            };

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/EditAffiliateType.cshtml", model);
        }

        [HttpPost]
        public ActionResult EditAffiliateType(AffiliateTypeModels model)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/EditAffiliateType.cshtml", model);

            if (SaveOrUpdateType(model))
                return RedirectToAction("AffiliateType");

            return View("~/Plugins/Widgets.BsAffiliate/Views/BsAffiliateConfigure/EditAffiliateType.cshtml", model);
        }
        
        public ActionResult DeleteAffiliateType(int id)
        {
            if (!_permissionService.Authorize(BsAffiliatePermissionProvider.BsAffiliateManageAffiliateType))
                return AccessDeniedView();

            var type = _atRepository.GetById(id);
            _atRepository.Delete(type);
            UpdateXml();
            return RedirectToAction("AffiliateType");
        }

        [NonAction]
        private bool SaveOrUpdateType(AffiliateTypeModels model)
        {
            bool res = false;
            try
            {
                if (model.Id == 0)
                {
                    var type = new AffiliateType()
                    {
                        Active = model.Active,
                        IdUrlParameter = model.IdUrlParameter,
                        NameUrlParameter = model.NameUrlParameter,
                        Name = model.Name
                    };
                    _atRepository.Insert(type);
                    res = true;
                }
                else
                {
                    var type = _atRepository.GetById(model.Id);
                    if (type != null)
                    {
                        type.IdUrlParameter = model.IdUrlParameter;
                        type.Name = model.Name;
                        type.NameUrlParameter = model.NameUrlParameter;
                        type.Active = model.Active;

                        _atRepository.Update(type);
                        res = true;
                    }
                }

                if (res)
                    UpdateXml();
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return res;
        }

        private void UpdateXml()
        {
            var types = _atRepository.Table.Where(x => x.Active).ToList();
            if (types != null)
            {
                var filePath = CommonHelper.MapPath("~/Plugins/Widgets.BsAffiliate/AffiliateTypes.xml");

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                XmlWriter writer = XmlWriter.Create(filePath, settings);

                writer.WriteStartDocument();

                writer.WriteComment("This file is generated by the program.");
                writer.WriteStartElement("Types");

                foreach (var item in types)
                {
                    writer.WriteStartElement("Type");
                    writer.WriteAttributeString("ID", item.Id.ToString());
                    writer.WriteElementString("Name", item.Name);
                    writer.WriteElementString("NameUrlParameter", item.NameUrlParameter);
                    writer.WriteElementString("IdUrlParameter", item.IdUrlParameter);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
            }
        }
    }
}

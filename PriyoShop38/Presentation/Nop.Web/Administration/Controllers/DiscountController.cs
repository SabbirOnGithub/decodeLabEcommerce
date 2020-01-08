using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Discounts;
using Nop.Core;
using Nop.Core.Caching; 
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Directory;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class DiscountController : BaseAdminController
    {
        #region Fields

        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ICurrencyService _currencyService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly CurrencySettings _currencySettings;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly IVendorService _vendorService;
        private readonly IOrderService _orderService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;

        #endregion

        #region Constructors

        public DiscountController(IDiscountService discountService,
            ILocalizationService localizationService,
            ICurrencyService currencyService,
            ICategoryService categoryService,
            IProductService productService,
            IWebHelper webHelper,
            IDateTimeHelper dateTimeHelper,
            ICustomerActivityService customerActivityService,
            CurrencySettings currencySettings,
            IPermissionService permissionService,
            IWorkContext workContext,
            IManufacturerService manufacturerService,
            IStoreService storeService,
            IVendorService vendorService,
            IOrderService orderService,
            IPriceFormatter priceFormatter,
            ICacheManager cacheManager,
            IEventPublisher eventPublisher)
        {
            this._discountService = discountService;
            this._localizationService = localizationService;
            this._currencyService = currencyService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._webHelper = webHelper;
            this._dateTimeHelper = dateTimeHelper;
            this._customerActivityService = customerActivityService;
            this._currencySettings = currencySettings;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._manufacturerService = manufacturerService;
            this._storeService = storeService;
            this._vendorService = vendorService;
            this._orderService = orderService;
            this._priceFormatter = priceFormatter;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual string GetRequirementUrlInternal(IDiscountRequirementRule discountRequirementRule, Discount discount, int? discountRequirementId)
        {
            if (discountRequirementRule == null)
                throw new ArgumentNullException("discountRequirementRule");

            if (discount == null)
                throw new ArgumentNullException("discount");

            string url = string.Format("{0}{1}", _webHelper.GetStoreLocation(), discountRequirementRule.GetConfigurationUrl(discount.Id, discountRequirementId));
            return url;
        }

        [NonAction]
        protected virtual void PrepareDiscountModel(DiscountModel model, Discount discount)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
            model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Promotions.Discounts.Requirements.DiscountRequirementType.Select"), Value = "" });
            var discountRules = _discountService.LoadAllDiscountRequirementRules();
            foreach (var discountRule in discountRules)
                model.AvailableDiscountRequirementRules.Add(new SelectListItem { Text = discountRule.PluginDescriptor.FriendlyName, Value = discountRule.PluginDescriptor.SystemName });

            if (discount != null)
            {
                //requirements
                foreach (var dr in discount.DiscountRequirements.OrderBy(dr => dr.Id))
                {
                    var drr = _discountService.LoadDiscountRequirementRuleBySystemName(dr.DiscountRequirementRuleSystemName);
                    if (drr != null)
                    {
                        model.DiscountRequirementMetaInfos.Add(new DiscountModel.DiscountRequirementMetaInfo
                        {
                            DiscountRequirementId = dr.Id,
                            RuleName = drr.PluginDescriptor.FriendlyName,
                            ConfigurationUrl = GetRequirementUrlInternal(drr, discount, dr.Id)
                        });
                    }
                }
            }
        }

        #endregion

        #region Discounts

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountListModel();
            model.AvailableDiscountTypes = DiscountType.AssignedToOrderTotal.ToSelectList(false).ToList();
            model.AvailableDiscountTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DiscountListModel model, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            DiscountType? discountType = null;
            if (model.SearchDiscountTypeId > 0)
                discountType = (DiscountType)model.SearchDiscountTypeId;
            var discounts = _discountService.GetAllDiscounts(discountType,
                model.SearchDiscountCouponCode,
                model.SearchDiscountName,
                true);

            if (model.BulkCouponOnly)
            {
                discounts = discounts.Where(x => !string.IsNullOrWhiteSpace(x.BulkTrack)).ToList();
            }
            else if (!model.IncludeBulkCoupon)
            {
                discounts = discounts.Where(x => string.IsNullOrWhiteSpace(x.BulkTrack)).ToList();
            }

            var gridModel = new DataSourceResult
            {
                Data = discounts.PagedForCommand(command).Select(x =>
                {
                    var discountModel = x.ToModel();
                    discountModel.DiscountTypeName = x.DiscountType.GetLocalizedEnum(_localizationService, _workContext);
                    discountModel.PrimaryStoreCurrencyCode = _currencyService.GetCurrencyById(_currencySettings.PrimaryStoreCurrencyId).CurrencyCode;
                    discountModel.TimesUsed = _discountService.GetAllDiscountUsageHistory(x.Id, pageSize: 1).TotalCount;
                    return discountModel;
                }),
                Total = discounts.Count
            };

            return Json(gridModel);
        }

        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel();
            PrepareDiscountModel(model, null);
            //default values
            model.LimitationTimes = 1;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(DiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var discount = model.ToEntity();
                _discountService.InsertDiscount(discount);

                //activity log
                _customerActivityService.InsertActivity("AddNewDiscount", _localizationService.GetResource("ActivityLog.AddNewDiscount"), discount.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Added"));
                
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = discount.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, null);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            var model = discount.ToModel();
            PrepareDiscountModel(model, discount);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(DiscountModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.Id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                _cacheManager.RemoveByPattern(Nop.Services.Catalog.Cache.PriceCacheEventConsumer.PRODUCT_PRICE_PATTERN_KEY);

                var prevDiscountType = discount.DiscountType;
                discount = model.ToEntity(discount);
                _discountService.UpdateDiscount(discount);

                //clean up old references (if changed) and update "HasDiscountsApplied" properties
                if (prevDiscountType == DiscountType.AssignedToCategories
                    && discount.DiscountType != DiscountType.AssignedToCategories)
                {
                    //applied to categories
                    discount.AppliedToCategories.Clear();
                    _discountService.UpdateDiscount(discount);
                }
                if (prevDiscountType == DiscountType.AssignedToManufacturers
                    && discount.DiscountType != DiscountType.AssignedToManufacturers)
                {
                    //applied to manufacturers
                    discount.AppliedToManufacturers.Clear();
                    _discountService.UpdateDiscount(discount);
                }
                if (prevDiscountType == DiscountType.AssignedToVendors
                    && discount.DiscountType != DiscountType.AssignedToVendors)
                {
                    //applied to manufacturers
                    discount.AppliedToVendors.Clear();
                    _discountService.UpdateDiscount(discount);
                }
                if (prevDiscountType == DiscountType.AssignedToSkus
                    && discount.DiscountType != DiscountType.AssignedToSkus)
                {
                    //applied to products
                    var products = discount.AppliedToProducts.ToList();
                    discount.AppliedToProducts.Clear();
                    _discountService.UpdateDiscount(discount);
                    //update "HasDiscountsApplied" property
                    foreach (var p in products)
                        _productService.UpdateHasDiscountsApplied(p);
                }

                //activity log
                _customerActivityService.InsertActivity("EditDiscount", _localizationService.GetResource("ActivityLog.EditDiscount"), discount.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Updated"));
                
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = discount.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, discount);
            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(id);
            if (discount == null)
                //No discount found with the specified id
                return RedirectToAction("List");

            //applied to products
            var products = discount.AppliedToProducts.ToList();

            _discountService.DeleteDiscount(discount);

            //update "HasDiscountsApplied" properties
            foreach (var p in products)
                _productService.UpdateHasDiscountsApplied(p);

            //activity log
            _customerActivityService.InsertActivity("DeleteDiscount", _localizationService.GetResource("ActivityLog.DeleteDiscount"), discount.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Deleted"));
            
            return RedirectToAction("List");
        }

        #endregion

        #region Discount requirements

        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult GetDiscountRequirementConfigurationUrl(string systemName, int discountId, int? discountRequirementId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (String.IsNullOrEmpty(systemName))
                throw new ArgumentNullException("systemName");

            var discountRequirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(systemName);
            if (discountRequirementRule == null)
                throw new ArgumentException("Discount requirement rule could not be loaded");

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            string url = GetRequirementUrlInternal(discountRequirementRule, discount, discountRequirementId);
            return Json(new { url = url }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDiscountRequirementMetaInfo(int discountRequirementId, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId);
            if (discountRequirement == null)
                throw new ArgumentException("Discount requirement could not be loaded");

            var discountRequirementRule = _discountService.LoadDiscountRequirementRuleBySystemName(discountRequirement.DiscountRequirementRuleSystemName);
            if (discountRequirementRule == null)
                throw new ArgumentException("Discount requirement rule could not be loaded");

            string url = GetRequirementUrlInternal(discountRequirementRule, discount, discountRequirementId);
            string ruleName = discountRequirementRule.PluginDescriptor.FriendlyName;
            return Json(new { url = url, ruleName = ruleName }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteDiscountRequirement(int discountRequirementId, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            var discountRequirement = discount.DiscountRequirements.FirstOrDefault(dr => dr.Id == discountRequirementId);
            if (discountRequirement == null)
                throw new ArgumentException("Discount requirement could not be loaded");

            _discountService.DeleteDiscountRequirement(discountRequirement);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public ActionResult ProductList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var products = discount
                .AppliedToProducts
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = products.Select(x => new DiscountModel.AppliedToProductModel
                {
                    ProductId = x.Id,
                    ProductName = x.Name
                }),
                Total = products.Count
            };

            return Json(gridModel);
        }

        public ActionResult ProductDelete(int discountId, int productId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var product = _productService.GetProductById(productId);
            if (product == null)
                throw new Exception("No product found with the specified id");

            //remove discount
            if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                product.AppliedDiscounts.Remove(discount);

            _productService.UpdateProduct(product);
            _productService.UpdateHasDiscountsApplied(product);
            
            return new NullJsonResult();
        }

        public ActionResult ProductAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddProductToDiscountModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = SelectListHelper.GetCategoryList(_categoryService, _cacheManager, true);
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            //manufacturers
            model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
                model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            return View(model);
        }

        [HttpPost]
        public ActionResult ProductAddPopupList(DataSourceRequest command, DiscountModel.AddProductToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var gridModel = new DataSourceResult();
            var products = _productService.SearchProducts(
                includeEkshopProducts: true,
                activeVendorOnly: false,
                categoryIds: new List<int> { model.SearchCategoryId },
                manufacturerId: model.SearchManufacturerId,
                storeId: model.SearchStoreId,
                vendorId: model.SearchVendorId,
                productType: model.SearchProductTypeId > 0 ? (ProductType?)model.SearchProductTypeId : null,
                keywords: model.SearchProductName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = products.Select(x => x.ToModel());
            gridModel.Total = products.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult ProductAddPopup(string btnId, string formId, DiscountModel.AddProductToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            product.AppliedDiscounts.Add(discount);

                        _productService.UpdateProduct(product);
                        _productService.UpdateHasDiscountsApplied(product);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Applied to categories

        [HttpPost]
        public ActionResult CategoryList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var categories = discount
                .AppliedToCategories
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x => new DiscountModel.AppliedToCategoryModel
                {
                    CategoryId = x.Id,
                    CategoryName = x.GetFormattedBreadCrumb(_categoryService)
                }),
                Total = categories.Count
            };

            return Json(gridModel);
        }

        public ActionResult CategoryDelete(int discountId, int categoryId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null)
                throw new Exception("No category found with the specified id");

            //remove discount
            if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                category.AppliedDiscounts.Remove(discount);

            _categoryService.UpdateCategory(category);

            return new NullJsonResult();
        }

        public ActionResult CategoryAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddCategoryToDiscountModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryAddPopupList(DataSourceRequest command, DiscountModel.AddCategoryToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                0, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = categories.Select(x =>
                {
                    var categoryModel = x.ToModel();
                    categoryModel.Breadcrumb = x.GetFormattedBreadCrumb(_categoryService);
                    return categoryModel;
                }),
                Total = categories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult CategoryAddPopup(string btnId, string formId, DiscountModel.AddCategoryToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedCategoryIds != null)
            {
                foreach (int id in model.SelectedCategoryIds)
                {
                    var category = _categoryService.GetCategoryById(id);
                    if (category != null)
                    {
                        if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            category.AppliedDiscounts.Add(discount);

                        _categoryService.UpdateCategory(category);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Applied to manufacturers

        [HttpPost]
        public ActionResult ManufacturerList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var manufacturers = discount
                .AppliedToManufacturers
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => new DiscountModel.AppliedToManufacturerModel
                {
                    ManufacturerId = x.Id,
                    ManufacturerName = x.Name
                }),
                Total = manufacturers.Count
            };

            return Json(gridModel);
        }

        public ActionResult ManufacturerDelete(int discountId, int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                throw new Exception("No manufacturer found with the specified id");

            //remove discount
            if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                manufacturer.AppliedDiscounts.Remove(discount);

            _manufacturerService.UpdateManufacturer(manufacturer);

            return new NullJsonResult();
        }

        public ActionResult ManufacturerAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddManufacturerToDiscountModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ManufacturerAddPopupList(DataSourceRequest command, DiscountModel.AddManufacturerToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var manufacturers = _manufacturerService.GetAllManufacturers(model.SearchManufacturerName,
                0, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => x.ToModel()),
                Total = manufacturers.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult ManufacturerAddPopup(string btnId, string formId, DiscountModel.AddManufacturerToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedManufacturerIds != null)
            {
                foreach (int id in model.SelectedManufacturerIds)
                {
                    var manufacturer = _manufacturerService.GetManufacturerById(id);
                    if (manufacturer != null)
                    {
                        if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            manufacturer.AppliedDiscounts.Add(discount);

                        _manufacturerService.UpdateManufacturer(manufacturer);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Discount usage history

        [HttpPost]
        public ActionResult UsageHistoryList(int discountId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetAllDiscountUsageHistory(discount.Id, null, null, command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = duh.Select(x => {
                    var order = _orderService.GetOrderById(x.OrderId);
                    var duhModel = new DiscountModel.DiscountUsageHistoryModel
                    {
                        Id = x.Id,
                        DiscountId = x.DiscountId,
                        OrderId = x.OrderId,
                        OrderTotal = order != null ? _priceFormatter.FormatPrice(order.OrderTotal, true, false) : "",
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                    return duhModel;
                }),
                Total = duh.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UsageHistoryDelete(int discountId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new ArgumentException("No discount found with the specified id");

            var duh = _discountService.GetDiscountUsageHistoryById(id);
            if (duh != null)
                _discountService.DeleteDiscountUsageHistory(duh);

            return new NullJsonResult();
        }

        #endregion

        #region Applied to vendors

        [HttpPost]
        public ActionResult VendorList(DataSourceRequest command, int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var vendors = discount
                .AppliedToVendors
                .Where(x => !x.Deleted)
                .ToList();
            var gridModel = new DataSourceResult
            {
                Data = vendors.Select(x => new DiscountModel.AppliedToVendorModel
                {
                    VendorId = x.Id,
                    VendorName = x.Name
                }),
                Total = vendors.Count
            };

            return Json(gridModel);
        }

        public ActionResult VendorDelete(int discountId, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(discountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new Exception("No vendor found with the specified id");

            //remove discount
            if (vendor.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                vendor.AppliedDiscounts.Remove(discount);

            _vendorService.UpdateVendor(vendor);

            return new NullJsonResult();
        }

        public ActionResult VendorAddPopup(int discountId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel.AddVendorToDiscountModel();

            return View(model);
        }

        [HttpPost]
        public ActionResult VendorAddPopupList(DataSourceRequest command, DiscountModel.AddVendorToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var gridModel = new DataSourceResult();
            var vendors = _vendorService.GetAllVendors(
                name: model.SearchVendorName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
                );
            gridModel.Data = vendors.Select(x => x.ToModel());
            gridModel.Total = vendors.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult VendorAddPopup(string btnId, string formId, DiscountModel.AddVendorToDiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var discount = _discountService.GetDiscountById(model.DiscountId);
            if (discount == null)
                throw new Exception("No discount found with the specified id");

            if (model.SelectedVendorIds != null)
            {
                foreach (int id in model.SelectedVendorIds)
                {
                    var vendor = _vendorService.GetVendorById(id);
                    if (vendor != null)
                    {
                        if (vendor.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            vendor.AppliedDiscounts.Add(discount);

                        _vendorService.UpdateVendor(vendor);
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Bulk create


        //create
        public ActionResult BulkCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new DiscountModel();
            model.IsBulkCreate = true;
            PrepareDiscountModel(model, null);
            //default values
            model.LimitationTimes = 1;
            return View(model);
        }

        [HttpPost]
        public ActionResult BulkCreate(DiscountModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                for (int i = 1; i <= model.TotalCoupons; i++)
                {
                    var discount = new Discount
                    {
                        BulkTrack = model.BulkTrack,
                        StartDateUtc = model.StartDateUtc,
                        CouponCode = GenerateCouponCode(model, i),
                        DiscountAmount = model.DiscountAmount,
                        DiscountLimitationId = model.DiscountLimitationId,
                        DiscountPercentage = model.DiscountPercentage,
                        DiscountTypeId = model.DiscountTypeId,
                        EndDateUtc = model.EndDateUtc,
                        IsCumulative = model.IsCumulative,
                        LimitationTimes = model.LimitationTimes,
                        Name = model.Name,
                        UsePercentage = model.UsePercentage,
                        RequiresCouponCode = model.RequiresCouponCode,
                        MaximumDiscountedQuantity = model.MaximumDiscountedQuantity,
                        MaximumDiscountAmount = model.MaximumDiscountAmount
                    };

                    _discountService.InsertDiscount(discount);

                    //activity log
                    _customerActivityService.InsertActivity("AddNewDiscount", _localizationService.GetResource("ActivityLog.AddNewDiscount"), discount.Name);

                    if (model.DiscountTypeId == 2)
                    {
                        if (!string.IsNullOrWhiteSpace(model.ProductIds))
                        {
                            foreach (int id in model.ProductIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                            {
                                var product = _productService.GetProductById(id);
                                if (product != null)
                                {
                                    if (product.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                                        product.AppliedDiscounts.Add(discount);

                                    _productService.UpdateProduct(product);
                                    _productService.UpdateHasDiscountsApplied(product);
                                }
                            }
                        }
                    }
                    else if (model.DiscountTypeId == 5)
                    {
                        if (!string.IsNullOrWhiteSpace(model.CategoryIds))
                        {
                            foreach (int id in model.CategoryIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                            {
                                var category = _categoryService.GetCategoryById(id);
                                if (category != null)
                                {
                                    if (category.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                                        category.AppliedDiscounts.Add(discount);

                                    _categoryService.UpdateCategory(category);
                                }
                            }
                        }
                    }
                    else if (model.DiscountTypeId == 6)
                    {
                        if (!string.IsNullOrWhiteSpace(model.ManufacturerIds))
                        {
                            foreach (int id in model.ManufacturerIds.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse))
                            {
                                var manufacturer = _manufacturerService.GetManufacturerById(id);
                                if (manufacturer != null)
                                {
                                    if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                                        manufacturer.AppliedDiscounts.Add(discount);

                                    _manufacturerService.UpdateManufacturer(manufacturer);
                                }
                            }
                        }
                    }
                }

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.Discounts.Added"));

                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            PrepareDiscountModel(model, null);
            return View(model);
        }

        private string GenerateCouponCode(DiscountModel model, int i)
        {
            if (!model.RequiresCouponCode)
                return null;

            int preFixLength = 0;
            if (!string.IsNullOrWhiteSpace(model.Prefix))
                preFixLength = model.Prefix.Length;

            if (model.PostfixType == PostfixType.Numeric)
            {
                var arr = Enumerable.Repeat(0, model.CouponLength - preFixLength).ToArray();
                return string.Format("{0}{1}", model.Prefix, i.ToString(string.Join("", arr)));
            }
            else if (model.PostfixType == PostfixType.AlphaNumeric)
            {
                var hex = i.ToString("X");
                var arr = Enumerable.Repeat(0, model.CouponLength - (preFixLength + hex.Length)).ToArray();
                return string.Format("{0}{1}{2}", model.Prefix, string.Join("", arr), hex);
            }
            else
            {
                var str = "";
                var n = i;
                if (n > 0)
                {
                    while (n > 0)
                    {
                        var x = n % 26;
                        n /= 26;
                        str = Convert.ToChar(x + 65).ToString() + str;
                    }
                }
                else
                    str = "A";

                if (str.Length < (model.CouponLength - preFixLength))
                {
                    var arr = Enumerable.Repeat("A", model.CouponLength - (preFixLength + str.Length)).ToArray();
                    str = string.Format("{0}{1}", string.Join("", arr), str);
                }
                return string.Format("{0}{1}", model.Prefix, str);
            }

        }
        #endregion
    }
}

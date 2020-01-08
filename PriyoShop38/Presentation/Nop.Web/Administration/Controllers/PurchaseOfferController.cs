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
using Nop.Services.Media;
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
    public class PurchaseOfferController : BaseAdminController
    {
        #region Fields

        private readonly IPurchaseOfferService _purchaseOfferService;
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
        private readonly IPictureService _pictureService;

        #endregion

        #region Constructors

        public PurchaseOfferController(IPurchaseOfferService purchaseOfferService,
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
            IEventPublisher eventPublisher,
            IPictureService pictureService)
        {
            this._purchaseOfferService = purchaseOfferService;
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
            this._pictureService = pictureService;
        }

        #endregion

        #region Utilities

        protected void PreparePurchaseOfferModel(PurchaseOfferModel model, PurchaseOffer purchaseOffer, bool preparePictureUrl = false)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (purchaseOffer != null)
            {
                if (purchaseOffer.GiftProduct != null && !purchaseOffer.GiftProduct.Deleted)
                {
                    model.GiftProductName = purchaseOffer.GiftProduct.Name;

                    if (preparePictureUrl)
                    {
                        var defaultProductPicture = _pictureService.GetPicturesByProductId(purchaseOffer.GiftProductId, 1).FirstOrDefault();
                        model.PictureUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
                    }
                }
                else
                {
                    model.GiftProductId = 0;
                    model.PictureUrl = _pictureService.GetDefaultPictureUrl(75);
                }
            }
        }

        #endregion

        #region Methods

        #region Purchase offers

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            return View();
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var pageIndex = command.Page < 1 ? 0 : command.Page - 1;
            var purchaseOffers = _purchaseOfferService.GetAllPurchaseOffers(true, PurchaseOfferSortingEnum.Default,
                pageIndex, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = purchaseOffers.PagedForCommand(command).Select(x =>
                {
                    var purchaseOfferModel = x.ToModel();
                    PreparePurchaseOfferModel(purchaseOfferModel, x, true);
                    return purchaseOfferModel;
                }),
                Total = purchaseOffers.TotalCount
            };

            return Json(gridModel);
        }

        //create
        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var model = new PurchaseOfferModel();
            PreparePurchaseOfferModel(model, null);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(PurchaseOfferModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var product = _productService.GetProductById(model.GiftProductId);
            if (product == null || product.Deleted)
                ModelState.AddModelError("GiftProductId", "Product is either not available or deleted with this specific id.");

            if (ModelState.IsValid)
            {
                var purchaseOffer = model.ToEntity();
                _purchaseOfferService.InsertPurchaseOffer(purchaseOffer);

                //activity log
                _customerActivityService.InsertActivity("AddNewPurchaseOffer", _localizationService.GetResource("ActivityLog.AddNewPurchaseOffer"));

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.PurchaseOffers.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = purchaseOffer.Id });
                }
                return RedirectToAction("List");
            }

            PreparePurchaseOfferModel(model, null);
            return View(model);
        }

        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(id);
            if (purchaseOffer == null)
                //No purchaseOffer found with the specified id
                return RedirectToAction("List");

            var model = purchaseOffer.ToModel();
            PreparePurchaseOfferModel(model, purchaseOffer);
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired(FormValueRequirement.StartsWith, "save")]
        public ActionResult Edit(PurchaseOfferModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.Id);
            if (purchaseOffer == null)
                //No purchaseOffer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var prevGiftProductId = purchaseOffer.GiftProductId;

                purchaseOffer = model.ToEntity(purchaseOffer);
                purchaseOffer.GiftProductId = prevGiftProductId;
                _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);

                //activity log
                _customerActivityService.InsertActivity("EditPurchaseOffer", _localizationService.GetResource("ActivityLog.EditPurchaseOffer"));

                SuccessNotification(_localizationService.GetResource("Admin.Promotions.PurchaseOffers.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = purchaseOffer.Id });
                }
                return RedirectToAction("List");
            }

            PreparePurchaseOfferModel(model, purchaseOffer);
            return View(model);
        }

        [HttpPost, ActionName("Edit")]
        [FormValueRequired("btnSaveGiftProduct")]
        public ActionResult ChangeGiftProduct(PurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.Id);
            if (purchaseOffer == null)
                return RedirectToAction("List");

            var product = _productService.GetProductById(model.GiftProductId);

            if (product == null || product.Deleted)
            {
                ErrorNotification("Product is either not available or deleted with this specific id.");
                return RedirectToAction("Edit", new { id = purchaseOffer.Id });
            }

            purchaseOffer.GiftProductId = product.Id;
            _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);

            return RedirectToAction("Edit", new { id = purchaseOffer.Id });
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(id);
            if (purchaseOffer == null)
                //No purchaseOffer found with the specified id
                return RedirectToAction("List");

            _purchaseOfferService.DeletePurchaseOffer(purchaseOffer);

            //activity log
            _customerActivityService.InsertActivity("DeletePurchaseOffer", _localizationService.GetResource("ActivityLog.DeletePurchaseOffer"));

            SuccessNotification(_localizationService.GetResource("Admin.Promotions.PurchaseOffers.Deleted"));

            return RedirectToAction("List");
        }

        #endregion

        #region Applied to products

        [HttpPost]
        public ActionResult ProductList(DataSourceRequest command, int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var products = purchaseOffer.AppliedToProducts.ToList();
            var gridModel = new DataSourceResult
            {
                Data = products.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).Select(x => new PurchaseOfferModel.AppliedToProductModel
                {
                    Id = x.Id,
                    ProductId = x.ProductId,
                    ProductName = x.Product.Name
                }),
                Total = products.Count
            };

            return Json(gridModel);
        }

        public ActionResult ProductDelete(int purchaseOfferId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var purchaseOfferProduct = purchaseOffer.AppliedToProducts.FirstOrDefault(x => x.Id == id);
            if (purchaseOfferProduct == null)
                throw new Exception("No product found with the specified id");
            
            _purchaseOfferService.DeleteAppliedProduct(purchaseOfferProduct);

            return new NullJsonResult();
        }

        public ActionResult ProductAddPopup(int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var model = new PurchaseOfferModel.AddProductToPurchaseOfferModel();
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
        public ActionResult ProductAddPopupList(DataSourceRequest command, PurchaseOfferModel.AddProductToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
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
        public ActionResult ProductAddPopup(string btnId, string formId, PurchaseOfferModel.AddProductToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.PurchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        if (purchaseOffer.AppliedToProducts.Count(d => d.ProductId == id) == 0)
                        {
                            var pop = new PurchaseOfferProduct()
                            {
                                ProductId = id,
                                PurchaseOfferId = purchaseOffer.Id
                            };

                            purchaseOffer.AppliedToProducts.Add(pop);
                            _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);
                        }
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
        public ActionResult CategoryList(DataSourceRequest command, int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var categories = purchaseOffer.AppliedToCategories.ToList();
            var gridModel = new DataSourceResult
            {
                Data = categories.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).Select(x => new PurchaseOfferModel.AppliedToCategoryModel
                {
                    Id = x.Id,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category.Name
                }),
                Total = categories.Count
            };

            return Json(gridModel);
        }

        public ActionResult CategoryDelete(int purchaseOfferId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var purchaseOfferCategory = purchaseOffer.AppliedToCategories.FirstOrDefault(x => x.Id == id);
            if (purchaseOfferCategory == null)
                throw new Exception("No category found with the specified id");
            
            _purchaseOfferService.DeleteAppliedCategory(purchaseOfferCategory);

            return new NullJsonResult();
        }

        public ActionResult CategoryAddPopup(int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var model = new PurchaseOfferModel.AddCategoryToPurchaseOfferModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryAddPopupList(DataSourceRequest command, PurchaseOfferModel.AddCategoryToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
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
        public ActionResult CategoryAddPopup(string btnId, string formId, PurchaseOfferModel.AddCategoryToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.PurchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            if (model.SelectedCategoryIds != null)
            {
                foreach (int id in model.SelectedCategoryIds)
                {
                    var category = _categoryService.GetCategoryById(id);
                    if (category != null)
                    {
                        if (purchaseOffer.AppliedToCategories.Count(d => d.CategoryId == id) == 0)
                        {
                            var poc = new PurchaseOfferCategory()
                            {
                                CategoryId = id,
                                PurchaseOfferId = purchaseOffer.Id
                            };

                            purchaseOffer.AppliedToCategories.Add(poc);
                            _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);
                        }
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
        public ActionResult ManufacturerList(DataSourceRequest command, int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var manufacturers = purchaseOffer.AppliedToManufacturers.ToList();
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).Select(x => new PurchaseOfferModel.AppliedToManufacturerModel
                {
                    Id = x.Id,
                    ManufacturerId = x.ManufacturerId,
                    ManufacturerName = x.Manufacturer.Name
                }),
                Total = manufacturers.Count
            };

            return Json(gridModel);
        }

        public ActionResult ManufacturerDelete(int purchaseOfferId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var purchaseOfferManufacturer = purchaseOffer.AppliedToManufacturers.FirstOrDefault(x => x.Id == id);
            if (purchaseOfferManufacturer == null)
                throw new Exception("No manufacturer found with the specified id");
            
            _purchaseOfferService.DeleteAppliedManufacturer(purchaseOfferManufacturer);

            return new NullJsonResult();
        }

        public ActionResult ManufacturerAddPopup(int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var model = new PurchaseOfferModel.AddManufacturerToPurchaseOfferModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult ManufacturerAddPopupList(DataSourceRequest command, PurchaseOfferModel.AddManufacturerToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
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
        public ActionResult ManufacturerAddPopup(string btnId, string formId, PurchaseOfferModel.AddManufacturerToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.PurchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchaseOffer found with the specified id");

            if (model.SelectedManufacturerIds != null)
            {
                foreach (int id in model.SelectedManufacturerIds)
                {
                    var manufacturer = _manufacturerService.GetManufacturerById(id);
                    if (manufacturer != null)
                    {
                        if (purchaseOffer.AppliedToManufacturers.Count(d => d.ManufacturerId == id) == 0)
                        {
                            var poc = new PurchaseOfferManufacturer()
                            {
                                ManufacturerId = id,
                                PurchaseOfferId = purchaseOffer.Id
                            };

                            purchaseOffer.AppliedToManufacturers.Add(poc);
                            _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Applied to vendors

        [HttpPost]
        public ActionResult VendorList(DataSourceRequest command, int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var vendors = purchaseOffer.AppliedToVendors.ToList();
            var gridModel = new DataSourceResult
            {
                Data = vendors.Skip((command.Page - 1) * command.PageSize).Take(command.PageSize).Select(x => new PurchaseOfferModel.AppliedToVendorModel
                {
                    Id = x.Id,
                    VendorId = x.VendorId,
                    VendorName = x.Vendor.Name
                }),
                Total = vendors.Count
            };

            return Json(gridModel);
        }

        public ActionResult VendorDelete(int purchaseOfferId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            var purchaseOfferVendor = purchaseOffer.AppliedToVendors.FirstOrDefault(x => x.Id == id);
            if (purchaseOfferVendor == null)
                throw new Exception("No vendor found with the specified id");

            _purchaseOfferService.DeleteAppliedVendor(purchaseOfferVendor);

            return new NullJsonResult();
        }

        public ActionResult VendorAddPopup(int purchaseOfferId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var model = new PurchaseOfferModel.AddVendorToPurchaseOfferModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult VendorAddPopupList(DataSourceRequest command, PurchaseOfferModel.AddVendorToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var gridModel = new DataSourceResult();
            var vendors = _vendorService.GetAllVendors(
                name: model.SearchVendorName,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true);
            gridModel.Data = vendors.Select(x => x.ToModel());
            gridModel.Total = vendors.TotalCount;

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult VendorAddPopup(string btnId, string formId, PurchaseOfferModel.AddVendorToPurchaseOfferModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(model.PurchaseOfferId);
            if (purchaseOffer == null)
                throw new Exception("No purchase offer found with the specified id");

            if (model.SelectedVendorIds != null)
            {
                foreach (int id in model.SelectedVendorIds)
                {
                    var vendor = _vendorService.GetVendorById(id);
                    if (vendor != null)
                    {
                        if (purchaseOffer.AppliedToVendors.Count(d => d.VendorId == id) == 0)
                        {
                            var pov = new PurchaseOfferVendor()
                            {
                                VendorId = id,
                                PurchaseOfferId = purchaseOffer.Id
                            };

                            purchaseOffer.AppliedToVendors.Add(pov);
                            _purchaseOfferService.UpdatePurchaseOffer(purchaseOffer);
                        }
                    }
                }
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Purchase offer usage history

        [HttpPost]
        public ActionResult UsageHistoryList(int purchaseOfferId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePurchaseOffers))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new ArgumentException("No purchase offer found with the specified id");

            var duh = _purchaseOfferService.GetPurchaseOfferUsageHistory(purchaseOffer.Id, command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = duh.Select(x => {
                    var duhModel = new PurchaseOfferModel.PurchaseOfferHistoryModel
                    {
                        Id = x.Id,
                        PurchaseOfferId = x.PurchaseOfferId,
                        OrderId = x.Order != null ? x.Order.Id : 0,
                        OrderTotal = x.Order != null ? _priceFormatter.FormatPrice(x.Order.OrderTotal, true, false) : "",
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(x.CreatedOnUtc, DateTimeKind.Utc)
                    };
                    return duhModel;
                }),
                Total = duh.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult UsageHistoryDelete(int purchaseOfferId, int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var purchaseOffer = _purchaseOfferService.GetPurchaseOfferById(purchaseOfferId);
            if (purchaseOffer == null)
                throw new ArgumentException("No purchase offer found with the specified id");

            var pouh = _purchaseOfferService.GetPurchaseOfferUsageHistoryById(id);
            if (pouh != null)
                _purchaseOfferService.DeletePurchaseOffeUsageHistory(pouh);

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}
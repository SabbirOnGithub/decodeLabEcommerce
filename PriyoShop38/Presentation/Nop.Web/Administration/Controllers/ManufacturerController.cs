﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Helpers;
using Nop.Admin.Models.Catalog;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Discounts;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class ManufacturerController : BaseAdminController
    {
        #region Fields

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IManufacturerTemplateService _manufacturerTemplateService;
        private readonly IProductService _productService;
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IPictureService _pictureService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IExportManager _exportManager;
        private readonly IDiscountService _discountService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IVendorService _vendorService;
        private readonly IAclService _aclService; 
        private readonly IPermissionService _permissionService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IWorkContext _workContext;
        private readonly IImportManager _importManager;
        private readonly ICacheManager _cacheManager;
        private readonly IEventPublisher _eventPublisher;
        private readonly IDateTimeHelper _dateTimeHelper;

        #endregion

        #region Constructors

        public ManufacturerController(ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            IManufacturerTemplateService manufacturerTemplateService,
            IProductService productService,
            ICustomerService customerService, 
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService, 
            IPictureService pictureService,
            ILanguageService languageService, 
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService, 
            IExportManager exportManager,
            IDiscountService discountService,
            ICustomerActivityService customerActivityService, 
            IVendorService vendorService,
            IAclService aclService,
            IPermissionService permissionService,
            CatalogSettings catalogSettings,
            IWorkContext workContext,
            IImportManager importManager, 
            ICacheManager cacheManager,
            IEventPublisher eventPublisher,
            IDateTimeHelper dateTimeHelper)
        {
            this._categoryService = categoryService;
            this._manufacturerTemplateService = manufacturerTemplateService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._customerService = customerService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._pictureService = pictureService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._exportManager = exportManager;
            this._discountService = discountService;
            this._customerActivityService = customerActivityService;
            this._vendorService = vendorService;
            this._aclService = aclService;
            this._permissionService = permissionService;
            this._catalogSettings = catalogSettings;
            this._workContext = workContext;
            this._importManager = importManager;
            this._cacheManager = cacheManager;
            this._eventPublisher = eventPublisher;
            this._dateTimeHelper = dateTimeHelper;
        }

        #endregion
        
        #region Utilities

        [NonAction]
        protected virtual void UpdateLocales(Manufacturer manufacturer, ManufacturerModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.ShortDescription,
                                                           localized.ShortDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(manufacturer,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = manufacturer.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(manufacturer, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Manufacturer manufacturer)
        {
            var picture = _pictureService.GetPictureById(manufacturer.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(manufacturer.Name));
        }

        [NonAction]
        protected virtual void PrepareTemplatesModel(ManufacturerModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _manufacturerTemplateService.GetAllManufacturerTemplates();
            foreach (var template in templates)
            {
                model.AvailableManufacturerTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void PrepareDiscountModel(ManufacturerModel model, Manufacturer manufacturer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && manufacturer != null)
                model.SelectedDiscountIds = manufacturer.AppliedDiscounts.Select(d => d.Id).ToList();

            foreach (var discount in _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true))
            {
                model.AvailableDiscounts.Add(new SelectListItem
                {
                    Text = discount.Name,
                    Value = discount.Id.ToString(),
                    Selected = model.SelectedDiscountIds.Contains(discount.Id)
                });
            }
        }

        [NonAction]
        protected virtual void PrepareAclModel(ManufacturerModel model, Manufacturer manufacturer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && manufacturer != null)
                model.SelectedCustomerRoleIds = _aclService.GetCustomerRoleIdsWithAccess(manufacturer).ToList();

            var allRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var role in allRoles)
            {
                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveManufacturerAcl(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.SubjectToAcl = model.SelectedCustomerRoleIds.Any();

            var existingAclRecords = _aclService.GetAclRecords(manufacturer);
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            foreach (var customerRole in allCustomerRoles)
            {
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                {
                    //new role
                    if (existingAclRecords.Count(acl => acl.CustomerRoleId == customerRole.Id) == 0)
                        _aclService.InsertAclRecord(manufacturer, customerRole.Id);
                }
                else
                {
                    //remove role
                    var aclRecordToDelete = existingAclRecords.FirstOrDefault(acl => acl.CustomerRoleId == customerRole.Id);
                    if (aclRecordToDelete != null)
                        _aclService.DeleteAclRecord(aclRecordToDelete);
                }
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(ManufacturerModel model, Manufacturer manufacturer, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (!excludeProperties && manufacturer != null)
                model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(manufacturer).ToList();

            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                model.AvailableStores.Add(new SelectListItem
                {
                    Text = store.Name,
                    Value = store.Id.ToString(),
                    Selected = model.SelectedStoreIds.Contains(store.Id)
                });
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(Manufacturer manufacturer, ManufacturerModel model)
        {
            manufacturer.LimitedToStores = model.SelectedStoreIds.Any();

            var existingStoreMappings = _storeMappingService.GetStoreMappings(manufacturer);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(manufacturer, store.Id);
                }
                else
                {
                    //remove store
                    var storeMappingToDelete = existingStoreMappings.FirstOrDefault(sm => sm.StoreId == store.Id);
                    if (storeMappingToDelete != null)
                        _storeMappingService.DeleteStoreMapping(storeMappingToDelete);
                }
            }
        }

        #endregion
        
        #region List

        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var model = new ManufacturerListModel();
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ManufacturerListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var manufacturers = _manufacturerService.GetAllManufacturers(model.SearchManufacturerName,
                model.SearchStoreId, command.Page - 1, command.PageSize, true);
            var gridModel = new DataSourceResult
            {
                Data = manufacturers.Select(x => x.ToModel()),
                Total = manufacturers.TotalCount
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var model = new ManufacturerModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //templates
            PrepareTemplatesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, false);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //default values
            model.PageSize = _catalogSettings.DefaultManufacturerPageSize;
            model.PageSizeOptions = _catalogSettings.DefaultManufacturerPageSizeOptions;
            model.Published = true;
            model.AllowCustomersToSelectPageSize = true;
            
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(ManufacturerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var manufacturer = model.ToEntity();
                manufacturer.CreatedOnUtc = DateTime.UtcNow;
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;
                _manufacturerService.InsertManufacturer(manufacturer);
                _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
                {
                    ManufacturerId = manufacturer.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Description = "Manufacturer created."
                });
                //search engine name
                model.SeName = manufacturer.ValidateSeName(model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);
                //locales
                UpdateLocales(manufacturer, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                        manufacturer.AppliedDiscounts.Add(discount);
                }
                _manufacturerService.UpdateManufacturer(manufacturer);
                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);
                //ACL (customer roles)
                SaveManufacturerAcl(manufacturer, model);
                //Stores
                SaveStoreMappings(manufacturer, model);

                //activity log
                _customerActivityService.InsertActivity("AddNewManufacturer", _localizationService.GetResource("ActivityLog.AddNewManufacturer"), manufacturer.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = manufacturer.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //discounts
            PrepareDiscountModel(model, null, true);
            //ACL
            PrepareAclModel(model, null, true);
            //Stores
            PrepareStoresMappingModel(model, null, true);

            return View(model);
        }

        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null || manufacturer.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            var model = manufacturer.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = manufacturer.GetLocalized(x => x.Name, languageId, false, false);
                locale.Description = manufacturer.GetLocalized(x => x.Description, languageId, false, false);
                locale.ShortDescription = manufacturer.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.MetaKeywords = manufacturer.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = manufacturer.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = manufacturer.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = manufacturer.GetSeName(languageId, false, false);
            });
            //templates
            PrepareTemplatesModel(model);
            //discounts
            PrepareDiscountModel(model, manufacturer, false);
            //ACL
            PrepareAclModel(model, manufacturer, false);
            //Stores
            PrepareStoresMappingModel(model, manufacturer, false);

            model.IsAdmin = _workContext.CurrentCustomer.IsAdmin();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(ManufacturerModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(model.Id);
            if (manufacturer == null || manufacturer.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                int prevPictureId = manufacturer.PictureId;
                manufacturer = model.ToEntity(manufacturer);
                manufacturer.UpdatedOnUtc = DateTime.UtcNow;

                #region BS-23

                manufacturer.RecentlyUpdated = true;
                manufacturer.LastUpdatedBy = _workContext.CurrentCustomer.Id;
                _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
                {
                    ManufacturerId = manufacturer.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Description = "Manufacturer updated."
                });

                #endregion

                _manufacturerService.UpdateManufacturer(manufacturer);
                //search engine name
                model.SeName = manufacturer.ValidateSeName(model.SeName, manufacturer.Name, true);
                _urlRecordService.SaveSlug(manufacturer, model.SeName, 0);
                //locales
                UpdateLocales(manufacturer, model);
                //discounts
                var allDiscounts = _discountService.GetAllDiscounts(DiscountType.AssignedToManufacturers, showHidden: true);
                foreach (var discount in allDiscounts)
                {
                    if (model.SelectedDiscountIds != null && model.SelectedDiscountIds.Contains(discount.Id))
                    {
                        //new discount
                        if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) == 0)
                            manufacturer.AppliedDiscounts.Add(discount);
                    }
                    else
                    {
                        //remove discount
                        if (manufacturer.AppliedDiscounts.Count(d => d.Id == discount.Id) > 0)
                            manufacturer.AppliedDiscounts.Remove(discount);
                    }
                }
                _manufacturerService.UpdateManufacturer(manufacturer);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != manufacturer.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(manufacturer);
                //ACL
                SaveManufacturerAcl(manufacturer, model);
                //Stores
                SaveStoreMappings(manufacturer, model);

                //activity log
                _customerActivityService.InsertActivity("EditManufacturer", _localizationService.GetResource("ActivityLog.EditManufacturer"), manufacturer.Name);

                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = manufacturer.Id});
                }
                return RedirectToAction("List");
            }


            //If we got this far, something failed, redisplay form
            //templates
            PrepareTemplatesModel(model);
            //discounts
            PrepareDiscountModel(model, manufacturer, true);
            //ACL
            PrepareAclModel(model, manufacturer, true);
            //Stores
            PrepareStoresMappingModel(model, manufacturer, true);

            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(id);
            if (manufacturer == null)
                //No manufacturer found with the specified id
                return RedirectToAction("List");


            #region BS-23

            manufacturer.RecentlyUpdated = true;
            manufacturer.UpdatedOnUtc = DateTime.UtcNow;
            manufacturer.LastUpdatedBy = _workContext.CurrentCustomer.Id;
            _manufacturerService.UpdateManufacturer(manufacturer);
            _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
            {
                ManufacturerId = manufacturer.Id,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                Description = "Manufacturer deleted."
            });

            #endregion

            _manufacturerService.DeleteManufacturer(manufacturer);

            //activity log
            _customerActivityService.InsertActivity("DeleteManufacturer", _localizationService.GetResource("ActivityLog.DeleteManufacturer"), manufacturer.Name);

            SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Deleted"));

            return RedirectToAction("List");
        }
        
        #endregion

        #region Export / Import

        public ActionResult ExportXml()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var manufacturers = _manufacturerService.GetAllManufacturers(showHidden: true);
                var xml = _exportManager.ExportManufacturersToXml(manufacturers);
                return new XmlDownloadResult(xml, "manufacturers.xml");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        public ActionResult ExportXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            try
            {
                var bytes = _exportManager.ExportManufacturersToXlsx(_manufacturerService.GetAllManufacturers(showHidden: true).Where(p=>!p.Deleted));
                 
                return File(bytes, MimeTypes.TextXlsx, "manufacturers.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost]
        public ActionResult ImportFromXlsx()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            //a vendor cannot import manufacturers
            if (_workContext.CurrentVendor != null)
                return AccessDeniedView();

            try
            {
                var file = Request.Files["importexcelfile"];
                if (file != null && file.ContentLength > 0)
                {
                    _importManager.ImportManufacturersFromXlsx(file.InputStream);
                }
                else
                {
                    ErrorNotification(_localizationService.GetResource("Admin.Common.UploadFile"));
                    return RedirectToAction("List");
                }
                SuccessNotification(_localizationService.GetResource("Admin.Catalog.Manufacturers.Imported"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Products

        [HttpPost]
        public ActionResult ProductList(DataSourceRequest command, int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var productManufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(manufacturerId,
                command.Page - 1, command.PageSize, true);

            var gridModel = new DataSourceResult
            {
                Data = productManufacturers
                .Select(x => new ManufacturerModel.ManufacturerProductModel
                {
                    Id = x.Id,
                    ManufacturerId = x.ManufacturerId,
                    ProductId = x.ProductId,
                    ProductName = _productService.GetProductById(x.ProductId).Name,
                    IsFeaturedProduct = x.IsFeaturedProduct,
                    DisplayOrder = x.DisplayOrder
                }),
                Total = productManufacturers.TotalCount
            };


            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ProductUpdate(ManufacturerModel.ManufacturerProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var productManufacturer = _manufacturerService.GetProductManufacturerById(model.Id);
            if (productManufacturer == null)
                throw new ArgumentException("No product manufacturer mapping found with the specified id");

            #region BS-23

            var product = _productService.GetProductById(productManufacturer.ProductId);
            product.RecentlyUpdated = true;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.LastUpdatedBy = _workContext.CurrentCustomer.Id;
            _productService.UpdateProduct(product);

            _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
            {
                ManufacturerId = productManufacturer.ManufacturerId,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                Description = "Manufacturer product updated."
            });

            #endregion

            productManufacturer.IsFeaturedProduct = model.IsFeaturedProduct;
            productManufacturer.DisplayOrder = model.DisplayOrder;
            _manufacturerService.UpdateProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult ProductDelete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var productManufacturer = _manufacturerService.GetProductManufacturerById(id);
            if (productManufacturer == null)
                throw new ArgumentException("No product manufacturer mapping found with the specified id");

            #region BS-23

            var product = _productService.GetProductById(productManufacturer.ProductId);
            product.RecentlyUpdated = true;
            product.UpdatedOnUtc = DateTime.UtcNow;
            product.LastUpdatedBy = _workContext.CurrentCustomer.Id;
            _productService.UpdateProduct(product);

            _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
            {
                ManufacturerId = productManufacturer.ManufacturerId,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                Description = "Manufacturer product deleted."
            });

            #endregion

            //var manufacturerId = productManufacturer.ManufacturerId;
            _manufacturerService.DeleteProductManufacturer(productManufacturer);

            return new NullJsonResult();
        }

        public ActionResult ProductAddPopup(int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            var model = new ManufacturerModel.AddManufacturerProductModel();
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
        public ActionResult ProductAddPopupList(DataSourceRequest command, ManufacturerModel.AddManufacturerProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
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
        public ActionResult ProductAddPopup(string btnId, string formId, ManufacturerModel.AddManufacturerProductModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return AccessDeniedView();

            if (model.SelectedProductIds != null)
            {
                foreach (int id in model.SelectedProductIds)
                {
                    var product = _productService.GetProductById(id);
                    if (product != null)
                    {
                        var existingProductmanufacturers = _manufacturerService.GetProductManufacturersByManufacturerId(model.ManufacturerId, showHidden: true);
                        if (existingProductmanufacturers.FindProductManufacturer(id, model.ManufacturerId) == null)
                        {
                            _manufacturerService.InsertProductManufacturer(
                                new ProductManufacturer
                                {
                                    ManufacturerId = model.ManufacturerId,
                                    ProductId = id,
                                    IsFeaturedProduct = false,
                                    DisplayOrder = 1
                                });
                        }
                    }
                    #region BS-23
                    
                    product.RecentlyUpdated = true;
                    product.UpdatedOnUtc = DateTime.UtcNow;
                    product.LastUpdatedBy = _workContext.CurrentCustomer.Id;
                    _productService.UpdateProduct(product);

                    #endregion
                }

                _manufacturerService.InsertManufacturerHistory(new ManufacturerHistory()
                {
                    ManufacturerId = model.ManufacturerId,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Description = "Manufacturer product added."
                });
            }

            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View(model);
        }

        #endregion

        #region Manufacturer History

        [HttpPost]
        public ActionResult ManufacturerHistoryList(DataSourceRequest command, int manufacturerId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);

            if (manufacturer == null || manufacturer.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            var manufacturerHistoryModels = new List<ManufacturerHistoryModel>();

            var manufacturerHistories = _manufacturerService.GetManufacturerHistoriesByManufacturerId(manufacturerId, pageIndex: command.Page - 1, pageSize: command.PageSize);

            foreach (var ph in manufacturerHistories)
            {
                manufacturerHistoryModels.Add(new ManufacturerHistoryModel
                {
                    Id = ph.Id,
                    CustomerId = ph.CustomerId,
                    CustomerEmail = _customerService.GetCustomerById(ph.CustomerId).Email,
                    ManufacturerId = ph.ManufacturerId,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(ph.CreatedOnUtc, DateTimeKind.Utc),
                    Description = ph.Description
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = manufacturerHistoryModels,
                Total = manufacturerHistories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ManufacturerHistoryDelete(int id, int manufacturerId)
        {
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();

            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null)
                throw new ArgumentException("No manufacturer found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = manufacturerId });

            var manufacturerHistory = _manufacturerService.GetManufacturerHistoryById(id);
            if (manufacturerHistory == null)
                throw new ArgumentException("No manufacturer history found with the specified id");
            _manufacturerService.DeleteManufacturerHistory(manufacturerHistory);

            return new NullJsonResult();
        }

        #endregion
    }
}

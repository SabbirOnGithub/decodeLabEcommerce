using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Vendors;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Payments;
using Nop.Core.Domain.Vendors;
using Nop.Core.Plugins;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.ExportImport;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Controllers
{
    public partial class VendorController : BaseAdminController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IPermissionService _permissionService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPictureService _pictureService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly VendorSettings _vendorSettings;
        private readonly IEventPublisher _eventPublisher;

        #region brainstation
        private readonly IProductService _productService; 
        private readonly IPaymentService _paymentService;
        private readonly PaymentSettings _paymentSettings;
        private readonly IWebHelper _webHelper;
        private readonly IExportManager _exportManager;
        private readonly IWorkContext _workContext;

        #endregion

        #endregion

        #region Constructors

        public VendorController(ICustomerService customerService, 
            ILocalizationService localizationService,
            IVendorService vendorService, 
            IPermissionService permissionService,
            IUrlRecordService urlRecordService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IPictureService pictureService,
            IDateTimeHelper dateTimeHelper,
            VendorSettings vendorSettings,
            IProductService productService,
            IPaymentService paymentService,
            PaymentSettings paymentSettings,
            IWebHelper webHelper,
            IExportManager exportManager,
            IWorkContext workContext,
            IEventPublisher eventPublisher)
        {
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._vendorService = vendorService;
            this._permissionService = permissionService;
            this._urlRecordService = urlRecordService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._pictureService = pictureService;
            this._dateTimeHelper = dateTimeHelper;
            this._vendorSettings = vendorSettings;
            this._productService = productService;
            this._paymentService = paymentService;
            this._paymentSettings = paymentSettings;
            this._webHelper = webHelper;
            this._exportManager = exportManager;
            this._workContext = workContext;
            this._eventPublisher = eventPublisher;
        }

        #endregion

        #region Utilities

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Vendor vendor)
        {
            var picture = _pictureService.GetPictureById(vendor.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(vendor.Name));
        }

        [NonAction]
        protected virtual void UpdateLocales(Vendor vendor, VendorModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(vendor,
                                                               x => x.Name,
                                                               localized.Name,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.ShortDescription,
                                                           localized.ShortDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.Description,
                                                           localized.Description,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(vendor,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = vendor.ValidateSeName(localized.SeName, localized.Name, false);
                _urlRecordService.SaveSlug(vendor, seName, localized.LanguageId);
            }
        }

        #endregion

        #region Vendors

        //list
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var model = new VendorListModel()
            {
                AvailableStatuses = new List<SelectListItem>()
            };

            Enum.GetValues(typeof(VendorStatus))
                .Cast<VendorStatus>()
                .Select(vs => new SelectListItem {Text = vs.ToString(), Value = ((int) vs).ToString()})
                .ToList()
                .ForEach(item => model.AvailableStatuses.Add(item));
            
            return View(model);
        }

        [HttpPost]
        public ActionResult VendorList(DataSourceRequest command, VendorListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendors = _vendorService.GetAllVendors(model.SearchVendorStatus, model.SearchName, command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = vendors.Select(x =>
                {
                    var vendorModel = x.ToModel();
                    return vendorModel;
                }),
                Total = vendors.TotalCount,
            };

            return Json(gridModel);
        }

        //create

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();


            var model = new VendorModel();
            //locales
            AddLocales(_languageService, model.Locales);
            //default values
            model.PageSize = 6;
            model.Active = true;
            model.AllowCustomersToSelectPageSize = true;
            model.PageSizeOptions = _vendorSettings.DefaultVendorPageSizeOptions;

            //default value
            model.Active = true;
            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Create(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var vendor = model.ToEntity();
                vendor.UpdatedOnUtc = DateTime.UtcNow;
                _vendorService.InsertVendor(vendor);
                _vendorService.InsertVendorHistory(new VendorHistory()
                {
                    VendorId = vendor.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Description = "Vendor created."
                });
                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);
                //locales
                UpdateLocales(vendor, model);
                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Added"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit", new { id = vendor.Id });
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }


        //edit
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var model = vendor.ToModel();
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Name = vendor.GetLocalized(x => x.Name, languageId, false, false);
                locale.ShortDescription = vendor.GetLocalized(x => x.ShortDescription, languageId, false, false);
                locale.Description = vendor.GetLocalized(x => x.Description, languageId, false, false);
                locale.MetaKeywords = vendor.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = vendor.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = vendor.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = vendor.GetSeName(languageId, false, false);
            });
            //associated customer emails
            model.AssociatedCustomers = _customerService
                .GetAllCustomers(vendorId: vendor.Id)
                .Select(c => new VendorModel.AssociatedCustomerInfo()
                {
                    Id = c.Id,
                    Email = c.Email
                })
                .ToList();

            model.IsAdmin = _workContext.CurrentCustomer.IsAdmin();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(VendorModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(model.Id);
            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                var oldActiveStatus = vendor.Active;

                int prevPictureId = vendor.PictureId;
                vendor = model.ToEntity(vendor);

                #region BS-23

                vendor.LastUpdatedBy = _workContext.CurrentCustomer.Id;
                vendor.RecentlyUpdated = true;
                vendor.UpdatedOnUtc = DateTime.UtcNow;

                #endregion

                _vendorService.UpdateVendor(vendor);
                _vendorService.InsertVendorHistory(new VendorHistory()
                {
                    VendorId = vendor.Id,
                    CreatedOnUtc = DateTime.UtcNow,
                    CustomerId = _workContext.CurrentCustomer.Id,
                    Description = "Vendor updated."
                });

                //search engine name
                model.SeName = vendor.ValidateSeName(model.SeName, vendor.Name, true);
                _urlRecordService.SaveSlug(vendor, model.SeName, 0);
                //locales
                UpdateLocales(vendor, model);
                //delete an old picture (if deleted or updated)
                if (prevPictureId > 0 && prevPictureId != vendor.PictureId)
                {
                    var prevPicture = _pictureService.GetPictureById(prevPictureId);
                    if (prevPicture != null)
                        _pictureService.DeletePicture(prevPicture);
                }
                //update picture seo file name
                UpdatePictureSeoNames(vendor);

                SuccessNotification(_localizationService.GetResource("Admin.Vendors.Updated"));

                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabName();

                    return RedirectToAction("Edit",  new {id = vendor.Id});
                }
                return RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form

            //associated customer emails
            model.AssociatedCustomers = _customerService
                .GetAllCustomers(vendorId: vendor.Id)
                .Select(c => new VendorModel.AssociatedCustomerInfo()
                {
                    Id = c.Id,
                    Email = c.Email
                })
                .ToList();

            return View(model);
        }

        //delete
        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(id);
            if (vendor == null)
                //No vendor found with the specified id
                return RedirectToAction("List");

            //clear associated customer references
            var associatedCustomers = _customerService.GetAllCustomers(vendorId: vendor.Id);
            foreach (var customer in associatedCustomers)
            {
                customer.VendorId = 0;
                _customerService.UpdateCustomer(customer);
            }

            //#region brainstation
            ////Delete associated products
            //var associatedProducts = _productService.SearchProducts(vendorId: id, showHidden: true);
            //if (associatedProducts.Count > 0)
            //{
            //    _productService.DeleteProducts(associatedProducts);

            //    foreach (var item in associatedProducts)
            //    {
            //        _eventPublisher.Publish(new ProductEvent(item));
            //    }
            //}
            //#endregion


            #region BS-23

            vendor.LastUpdatedBy = _workContext.CurrentCustomer.Id;
            vendor.RecentlyUpdated = true;
            vendor.UpdatedOnUtc = DateTime.UtcNow;

            _vendorService.UpdateVendor(vendor);

            #endregion


            //delete a vendor
            _vendorService.DeleteVendor(vendor);
            _vendorService.InsertVendorHistory(new VendorHistory()
            {
                VendorId = vendor.Id,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                Description = "Vendor deleted."
            });

            SuccessNotification(_localizationService.GetResource("Admin.Vendors.Deleted"));
            return RedirectToAction("List");
        }

        #endregion

        #region Vendor notes

        [HttpPost]
        public ActionResult VendorNotesSelect(int vendorId, DataSourceRequest command)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            var vendorNoteModels = new List<VendorModel.VendorNote>();
            foreach (var vendorNote in vendor.VendorNotes
                .OrderByDescending(vn => vn.CreatedOnUtc))
            {
                vendorNoteModels.Add(new VendorModel.VendorNote
                {
                    Id = vendorNote.Id,
                    VendorId = vendorNote.VendorId,
                    Note = vendorNote.FormatVendorNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(vendorNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = vendorNoteModels,
                Total = vendorNoteModels.Count
            };

            return Json(gridModel);
        }

        [ValidateInput(false)]
        public ActionResult VendorNoteAdd(int vendorId, string message)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return Json(new { Result = false }, JsonRequestBehavior.AllowGet);

            var vendorNote = new VendorNote
            {
                Note = message,
                CreatedOnUtc = DateTime.UtcNow,
            };
            vendor.VendorNotes.Add(vendorNote);
            _vendorService.UpdateVendor(vendor);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult VendorNoteDelete(int id, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            var vendorNote = vendor.VendorNotes.FirstOrDefault(vn => vn.Id == id);
            if (vendorNote == null)
                throw new ArgumentException("No vendor note found with the specified id");
            _vendorService.DeleteVendorNote(vendorNote);

            return new NullJsonResult();
        }

        #endregion

        #region Payment method restriction

        [HttpPost]
        public ActionResult PaymentMethodRestrictList(DataSourceRequest command, int vendorId)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                return Json(new EmptyResult());

            var paymentMethodsModel = new List<VendorRestrictedPaymentMethodModel>();
            var paymentMethods = _paymentService.LoadAllPaymentMethods();
            foreach (var paymentMethod in paymentMethods)
            {
                var tmp = paymentMethod.ToModel();
                var tmp1 = new VendorRestrictedPaymentMethodModel
                {
                    IsActive = paymentMethod.IsPaymentMethodActive(_paymentSettings),
                    LogoUrl = paymentMethod.PluginDescriptor.GetLogoUrl(_webHelper),
                    FriendlyName = tmp.FriendlyName,
                    SystemName = tmp.SystemName,
                    IsRestricted = vendor.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, tmp.SystemName, StringComparison.OrdinalIgnoreCase)),
                    VendorId = vendorId
                };
                paymentMethodsModel.Add(tmp1);
            }

            var gridModel = new DataSourceResult
            {
                Data = paymentMethodsModel,
                Total = paymentMethodsModel.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult PaymentMethodRestrictUpdate(VendorRestrictedPaymentMethodModel model)
        {
            var vendor = _vendorService.GetVendorById(model.VendorId);
            if (vendor == null)
                return new NullJsonResult();

            if (model.IsRestricted)
            {
                if (!vendor.RestrictedPaymentMethods.Any(x => string.Equals(x.SystemName, model.SystemName, StringComparison.OrdinalIgnoreCase)))
                {
                    var method = new VendorRestrictedPaymentMethod()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        VendorId = model.VendorId,
                        SystemName = model.SystemName,
                    };
                    _vendorService.AddRestrictedPaymentMethod(method);
                }
            }
            else
            {
                var method = vendor.RestrictedPaymentMethods.FirstOrDefault(x => string.Equals(x.SystemName, model.SystemName, StringComparison.OrdinalIgnoreCase));
                if (method != null)
                    _vendorService.DeleteRestrictedPaymentMethod(method);
            }
            _vendorService.InsertVendorHistory(new VendorHistory()
            {
                VendorId = vendor.Id,
                CreatedOnUtc = DateTime.UtcNow,
                CustomerId = _workContext.CurrentCustomer.Id,
                Description = "Vendor payment restriction updated."
            });

            return new NullJsonResult();
        }
        #endregion

        #region Export

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-all")]
        public ActionResult ExportExcelAll()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendors = _vendorService.GetAllVendors("", 0, int.MaxValue, true);            

            try
            {
                var bytes = _exportManager.ExportVendorsToXlsx(vendors);
                return File(bytes, MimeTypes.TextXlsx, "vendors.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        [HttpPost, ActionName("List")]
        [FormValueRequired("exportexcel-based-on-search-fields")]
        public ActionResult ExportToExcelBasedOnSearchFields(DataSourceRequest command, VendorListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                return AccessDeniedView();

            var vendors = _vendorService.GetAllVendors(model.SearchVendorStatus, model.SearchName, 0, int.MaxValue);

            try
            {
                var bytes = _exportManager.ExportVendorsToXlsx(vendors);
                return File(bytes, MimeTypes.TextXlsx, "vendors.xlsx");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("List");
            }
        }

        #endregion

        #region Vendor History

        [HttpPost]
        public ActionResult VendorHistoryList(DataSourceRequest command, int vendorId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);

            if (vendor == null || vendor.Deleted)
                //No vendor found with the specified id
                return RedirectToAction("List");

            var vendorHistoryModels = new List<VendorHistoryModel>();

            var vendorHistories = _vendorService.GetVendorHistoriesByVendorId(vendorId, pageIndex: command.Page - 1, pageSize: command.PageSize);

            foreach (var ph in vendorHistories)
            {
                vendorHistoryModels.Add(new VendorHistoryModel
                {
                    Id = ph.Id,
                    CustomerId = ph.CustomerId,
                    CustomerEmail = _customerService.GetCustomerById(ph.CustomerId).Email,
                    VendorId = ph.VendorId,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(ph.CreatedOnUtc, DateTimeKind.Utc),
                    Description = ph.Description
                });
            }

            var gridModel = new DataSourceResult
            {
                Data = vendorHistoryModels,
                Total = vendorHistories.TotalCount
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult VendorHistoryDelete(int id, int vendorId)
        {
            if (!_workContext.CurrentCustomer.IsAdmin())
                return AccessDeniedView();

            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null)
                throw new ArgumentException("No vendor found with the specified id");

            //a vendor does not have access to this functionality
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("Edit", "Order", new { id = vendorId });

            var vendorHistory = _vendorService.GetVendorHistoryById(id);
            if (vendorHistory == null)
                throw new ArgumentException("No vendor history found with the specified id");
            _vendorService.DeleteVendorHistory(vendorHistory);

            return new NullJsonResult();
        }

        #endregion
    }
}

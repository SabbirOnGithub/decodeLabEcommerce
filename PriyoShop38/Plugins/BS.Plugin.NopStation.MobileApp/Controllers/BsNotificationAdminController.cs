using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Home;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Core.Domain.Vendors;
using BS.Plugin.NopStation.MobileApp.Extensions;
using System.Collections.Generic;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework;
//using Nop.Plugin.NopStation.MobileWebApi.Services;
using BS.Plugin.NopStation.MobileWebApi.Services;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    [AdminAuthorize]
    public class BsNotificationAdminController : BasePluginController
    {

        #region Field

       private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly IVendorService _vendorService;
        private readonly ICustomerService _customerService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWorkContext _workContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly AdminAreaSettings _adminAreaSettings;
        private readonly ISmartGroupService _smartGroupsService;
        private readonly IManufacturerService _manufacturerService;
        //private readonly DateTimeHelper _dateTimeHelper;

        private readonly IScheduledNotificationService _scheduledNotificationService;
        private readonly IQueuedNotificationService _queuedNotificationService;
        private readonly IDeviceService _deviceService;
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly INotificationMessageTemplateService _notificationMessageTemplateService;
        private readonly IStoreContext _storeContext;
        #endregion

        #region Ctor
        public BsNotificationAdminController(ISmartGroupService instagramOfferService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            IVendorService vendorService,
            ICustomerService customerService,
            IUrlRecordService urlRecordService,
            IWorkContext workContext,
            IStoreService storeService,
            ISettingService settingService,
            AdminAreaSettings adminAreaSettings,
            ISmartGroupService smartGroupsService,
            IScheduledNotificationService scheduledNotificationService,
            IDeviceService deviceService,
            IQueuedNotificationService queuedNotificationService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService, INotificationMessageTemplateService notificationMessageTemplateService, IStoreContext storeContext) 
        {
            this._localizationService = localizationService;
            this._permissionService = permissionService;
            this._workContext = workContext;
            this._vendorService = vendorService;
            
            this._customerService = customerService;
            this._urlRecordService = urlRecordService;
            this._settingService = settingService;
            this._storeService = storeService;
            this._adminAreaSettings = adminAreaSettings;
            this._smartGroupsService = smartGroupsService;
            //this._dateTimeHelper = dateTimeHelper;

            this._scheduledNotificationService = scheduledNotificationService;
            this._queuedNotificationService = queuedNotificationService;
            this._deviceService = deviceService;
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            _notificationMessageTemplateService = notificationMessageTemplateService;
            _storeContext = storeContext;
        }
        #endregion

        #region Utility
        [NonAction]
        protected virtual string GetcustomerFullName(int customerId)
        {
            var customer = _customerService.GetCustomerById(customerId);
            return customer != null ? customer.GetFullName() : null;

        }

        [NonAction]
        protected virtual string GetDeviceType(int  deviceTypeId)
        {
            try
            {
                var deviceType= Enum.GetName(typeof (DeviceType), deviceTypeId);
                return deviceType;
            }
            catch (Exception)
            {
                
                return null;
            }
        }

        [NonAction]
        protected virtual string GetNotificationType(int notificationId)
        {
            try
            {
                var notificationType = Enum.GetName(typeof(NotificationType), notificationId);
                return notificationType;
            }
            catch (Exception)
            {

                return null;
            }
        }
        #endregion

        #region Action Method
        #region common
        /// <summary>
        /// Access denied view
        /// </summary>
        /// <returns>Access denied view</returns>
        protected ActionResult AccessDeniedView()
        {
            //return new HttpUnauthorizedResult();
            return RedirectToAction("AccessDenied", "Security", new { pageUrl = this.Request.RawUrl,area="Admin" });
        }
        #endregion

        #region configure
        [ChildActionOnly]
        public ActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return Content("Access denied");

            

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<NotificationSettings>(storeScope);

            var model = new ConfigurationModel();
            model.AppleCertFileNameWithPath = settings.AppleCertFileNameWithPath;
            model.ApplePassword = settings.ApplePassword;
            model.GoogleConsoleAPIAccess_KEY = settings.GoogleConsoleAPIAccess_KEY;
            model.IsAppleProductionMode = settings.IsAppleProductionMode;
            model.GoogleProject_Number = settings.GoogleProject_Number;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.AppleCertFileNameWithPath_OverrideForStore = _settingService.SettingExists(settings, x => x.AppleCertFileNameWithPath, storeScope);
                model.ApplePassword_OverrideForStore = _settingService.SettingExists(settings, x => x.ApplePassword, storeScope);
                model.GoogleConsoleAPIAccess_KEY_OverrideForStore = _settingService.SettingExists(settings, x => x.GoogleConsoleAPIAccess_KEY, storeScope);
            }
            
            return View("~/Plugins/NopStation.MobileApp/Views/BsNotification/Configure.cshtml", model);
        }
       
        [HttpPost]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }


            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var settings = _settingService.LoadSetting<NotificationSettings>(storeScope);

            //save settings
            settings.AppleCertFileNameWithPath = model.AppleCertFileNameWithPath;
            settings.ApplePassword = model.ApplePassword;
            settings.GoogleConsoleAPIAccess_KEY = model.GoogleConsoleAPIAccess_KEY;
            settings.IsAppleProductionMode = model.IsAppleProductionMode;
            settings.GoogleProject_Number = model.GoogleProject_Number;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            if (model.AppleCertFileNameWithPath_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.AppleCertFileNameWithPath, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.AppleCertFileNameWithPath, storeScope);

            if (model.ApplePassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.ApplePassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.ApplePassword, storeScope);


            if (model.GoogleConsoleAPIAccess_KEY_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(settings, x => x.GoogleConsoleAPIAccess_KEY, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(settings, x => x.GoogleConsoleAPIAccess_KEY, storeScope);
            
            _settingService.SaveSetting(settings,x=>x.IsAppleProductionMode);
            _settingService.SaveSetting(settings, x => x.GoogleProject_Number);
            //now clear settings cache
            _settingService.ClearCache();

            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();

        }

        #endregion

        #region Device
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Schedules the list.
        /// </summary>
        /// <returns></returns>

        public ActionResult Device()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            return View("DeviceList");
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Device the list.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeviceList(DataSourceRequest command)
        {
            var devices = _deviceService.GetAllDevice(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = devices.Select(x =>
                {

                    DeviceModel m = x.ToModel();
                    m.DeviceType =GetDeviceType(m.DeviceTypeId);
                    m.CustomerName = GetcustomerFullName(m.CustomerId);

                    return m;
                }),

                Total = devices.TotalCount

            };
            return Json(gridModel);
        }

      
        #endregion

        #region Group

        public ActionResult Index()
        {
            return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups"); ;
        }

        public ActionResult Group()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            return View();
        }
        
        
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Groups the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GroupList(DataSourceRequest command)
        {
            var groups = _smartGroupsService.GetAllSmartGroup(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = groups.Select(x =>
                {

                    CriteriaModel m = x.ToCriteriaModel();

                    return m;
                }),

                Total = groups.TotalCount

            };
            return Json(gridModel);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Smarts the group.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        [AdminAuthorize]
        public ActionResult SmartGroup(int id)
        {
            ViewBag.SmartGroupId = id;
            return View();
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Smarts the group ajax.
        /// </summary>
        /// <param name="smartGroupId">The smart group id.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SmartGroupList(int smartGroupId, DataSourceRequest command)
        {

            var contacts = _smartGroupsService.GetContacts(smartGroupId, command.Page - 1, command.PageSize);
            IEnumerable<SmartContactModel> test = contacts;
            var GridModel = new DataSourceResult
            {
                Data = contacts.Select(x =>
                {

                    SmartContactModel m = x;
                    m.FullName = _customerService.GetCustomerById(m.CustomerId) != null
                        ? _customerService.GetCustomerById(m.CustomerId).GetFullName()
                        : m.FirstName+' '+m.LastName;
                    return m;
                }),

                Total = contacts.TotalCount

            };
           
            return Json(GridModel);

        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateGroup()
        {
            var model = new CriteriaModel();
            return View(model);
            //return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the group.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateGroup(CriteriaModel model)
        {

            if (ModelState.IsValid)
            {
                var criteria = model.ToSmartGroupEntity();
                criteria.CreatedOnUtc = DateTime.UtcNow;
                criteria.UpdatedOnUtc = DateTime.UtcNow;
                _smartGroupsService.InsertSmartGroup(criteria);

                //SuccessNotification(_localizationService.GetResource("Admin.Promotions.Campaigns.Added"));
                /*return continueEditing ? RedirectToAction("Edit", new
                {
                    id = campaign.Id
                }) : RedirectToAction("Campaign");*/
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups");
            }

            //If we got this far, something failed, redisplay form
            //model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfCampaignAllowedTokens());
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Edits the group.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult EditGroup(int id)
        {
            var smartGroup = _smartGroupsService.GetSmartGroupById(id);
            if (smartGroup == null)
                //No campaign found with the specified id
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups"); 

            var model = smartGroup.ToCriteriaModel();
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Edits the group.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        
        public ActionResult EditGroup(CriteriaModel model)
        {


            var smartGroup = _smartGroupsService.GetSmartGroupById(model.Id);
            if (smartGroup == null)
                //No campaign found with the specified id
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups"); ;

            if (ModelState.IsValid)
            {
                model.ToSmartGroupEntity(smartGroup);
                smartGroup.UpdatedOnUtc = DateTime.UtcNow;
                _smartGroupsService.UpdateSmartGroup(smartGroup);


                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups"); 
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the group.
        /// </summary>
        /// <param name="id">The selected id</param>
        /// <returns></returns>
        /// 
        [HttpPost]
        public ActionResult DeleteGroup(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var group = _smartGroupsService.GetSmartGroupById(id);
            if (group == null)
                throw new ArgumentException("No resource found with the specified id");
           
            _smartGroupsService.DeleteSmartGroup(group); 

            return new NullJsonResult();
        }
      
        #endregion

        #region Schedule

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Schedules the list.
        /// </summary>
        /// <returns></returns>
        
        public ActionResult Schedule()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            return View("ScheduleList");
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Schedules the list.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
         [HttpPost]
        public ActionResult ScheduleList(DataSourceRequest command)
        {
            var groups = _scheduledNotificationService.GetAllSchedule(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = groups.Select(x =>
                {

                    ScheduledNotificationModel m = x.ToModel();
                    m.GroupName = _smartGroupsService.GetGruopName(x.GroupId);
                    m.NotificationType = GetNotificationType(m.NotificationTypeId);
                    return m;
                }),

                Total = groups.TotalCount

            };
            return Json(gridModel);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the schedule.
        /// </summary>
        /// <param name="campaignId">The campaign id.</param>
        /// <returns></returns>
        public ActionResult CreateSchedule()
        {
            var model = new ScheduledNotificationModel();
         
           var groups = _smartGroupsService.GetAllSmartGroup();
            var messageTemplates = _notificationMessageTemplateService.GetAllNotificationMessageTemplates(_storeContext.CurrentStore.Id);

            foreach (var messageTemplate in messageTemplates)
            {
                model.AvailableMessageTemplates.Add(new SelectListItem
                {
                    Text = messageTemplate.Name,
                    Value = messageTemplate.Id.ToString()
                });
            }
           model.AvailableGroups = new SelectList(groups, "Id", "Name");
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Creates the schedule.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateSchedule(ScheduledNotificationModel model)
        {
            if (ModelState.IsValid)
            {
                //var timeOffset = DateTime.Now - DateTime.UtcNow;


                var schedule = model.ToEntity();
                var messageTemplate =
                    _notificationMessageTemplateService.GetNotificationMessageTemplateById(
                        schedule.NotificationMessageTemplateId);

                schedule.Message = messageTemplate != null? messageTemplate.Body : null;
                schedule.Subject = messageTemplate != null ? messageTemplate.Subject : null;

                schedule.CreatedOnUtc = DateTime.UtcNow;
                _scheduledNotificationService.InsertSchedule(schedule);
                //_scheduleService.InsertSchedule(schedule);

                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList");
                
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Edits the schedule.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public ActionResult EditSchedule(int id)
        {
            var schedule = _scheduledNotificationService.GetScheduleById(id);
            //var timeZoneConverted = TimeZoneInfo.ConvertTimeToUtc(schedule.ScheduledTime).AddHours(timeDiffer);

            if (schedule == null)
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList");

            var model = schedule.ToModel();
            var groups = _smartGroupsService.GetAllSmartGroup();
            model.AvailableGroups = new SelectList(groups, "Id", "Name");
            var messageTemplates = _notificationMessageTemplateService.GetAllNotificationMessageTemplates(_storeContext.CurrentStore.Id);

            foreach (var messageTemplate in messageTemplates)
            {
                model.AvailableMessageTemplates.Add(new SelectListItem
                {
                    Text = messageTemplate.Name,
                    Value = messageTemplate.Id.ToString()
                });
            }

            return View( model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Edits the schedule.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditSchedule(ScheduledNotificationModel model)
        {
            var schedule = _scheduledNotificationService.GetScheduleById(model.Id);
            
            if (schedule == null)
                //No emailTemplate found with the specified id
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList");
            model.ItemId = schedule.ItemId;
            model.PictureId = schedule.PictureId;
            if (ModelState.IsValid)
            {
                
                schedule = model.ToEntity(schedule);
                var messageTemplate =
                   _notificationMessageTemplateService.GetNotificationMessageTemplateById(
                       schedule.NotificationMessageTemplateId);
                schedule.Message = messageTemplate != null ? messageTemplate.Body : null;
                schedule.Subject = messageTemplate != null ? messageTemplate.Subject : null;
                schedule.IsQueued = false;
                _scheduledNotificationService.UpdateSchedule(schedule);
                
                return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Deletes the schedule.
        /// </summary>
        /// <param name="id">The selected ids.</param>
        /// <returns></returns>
        
        public ActionResult DeleteSchedule(int id)
        {
            
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            var schedule = _scheduledNotificationService.GetScheduleById(id);
            if (schedule == null)
                throw new ArgumentException("No resource found with the specified id");

            _scheduledNotificationService.DeleteSchedule(schedule);

            return RedirectToRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList");

        }

        #endregion

        #region Queued notification
        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Schedules the list.
        /// </summary>
        /// <returns></returns>

        public ActionResult Queued()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePlugins))
                return AccessDeniedView();

            return View("QueuedList");
        }

        ///--------------------------------------------------------------------------------------------
        /// <summary>
        /// Queued the list.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult QueuedList(DataSourceRequest command)
        {
            var groups = _queuedNotificationService.GetAllQueuedNotification(command.Page - 1, command.PageSize);
            var gridModel = new DataSourceResult
            {
                Data = groups.Select(x =>
                {

                    QueuedNotificationModel m = x.ToModel();
                    m.GroupName = _smartGroupsService.GetGruopName(x.GroupId);
                    m.ToCustomerName = GetcustomerFullName(m.ToCustomerId);
                    m.NotificationType = GetNotificationType(m.NotificationTypeId);
                    return m;
                }),

                Total = groups.TotalCount

            };
            return Json(gridModel);
        }

        
        #endregion

        #region Add Category
        public ActionResult CategoryAddPopup(int scheduleId)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            var model = new CategoryForNotificationModel();
            model.ScheduleIdId = scheduleId;
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            return View(model);
        }

        [HttpPost]
        public ActionResult CategoryAddPopupList(DataSourceRequest command, CategoryForNotificationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageDiscounts))
                return AccessDeniedView();

            //var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
            //    command.Page - 1, command.PageSize, true);
            var categories = _categoryService.GetAllCategories(model.SearchCategoryName,
                 model.SearchStoreId, command.Page - 1, command.PageSize, true);  //change3.8
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
        public ActionResult SetCatOrProdId(int scheduleId, int catOrProdId)
        {
           
            var schedule = _scheduledNotificationService.GetScheduleById(scheduleId);
            schedule.ItemId = catOrProdId;
            _scheduledNotificationService.UpdateSchedule(schedule);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Add Product
        public ActionResult ProductAddPopup(int scheduleId)
        {
            var model = new ProductForNotificationModel();
            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

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
            model.ScheduleIdId = scheduleId;
            return View(model);
            
            
        }

        [HttpPost]
        public ActionResult ProductAddPopupList(DataSourceRequest command, ProductForNotificationModel model)
        {
            

            var gridModel = new DataSourceResult();
            var products = _productService.SearchProducts(
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

        
        #endregion


        #endregion
        
        #region  Test notification

        [HttpGet]
        public ActionResult TestPushNotification()
        {
            var model = new QueuedNotificationModel();
            
            return View(model);
        }
        [HttpPost]
        public ActionResult TestPushNotification(QueuedNotificationModel model)
        {
            if (model.DeviceTypeId==(int)DeviceType.iPhone)
            {
                try
                {
                    var notification = new QueuedNotification()
                    {
                        DeviceType = DeviceType.iPhone,
                        SubscriptionId = model.SubscriptionId,
                        Message =model.Message,
                        NotificationTypeId = model.NotificationTypeId,
                        ItemId = model.ItemId,
                        Image = model.Image

                    };
                  
                    _queuedNotificationService.SendTestNotication(notification);
                    SuccessNotification("Sent Notification successfully!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error:", ex.Message);
                   
                } 
            }
            else if (model.DeviceTypeId == (int)DeviceType.Android)
            {
                try
                {
                    var notification = new QueuedNotification()
                    {
                        DeviceType = DeviceType.Android,
                        SubscriptionId = model.SubscriptionId,
                        Message = model.Message,
                        NotificationTypeId = model.NotificationTypeId,
                        ItemId = model.ItemId,
                        Image = model.Image

                    };

                    _queuedNotificationService.SendTestNotication(notification);
                    SuccessNotification("Sent Notification successfully!");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Error:", ex.Message);

                } 
            }
            else
            {
                ModelState.AddModelError("Error:","UnKnown Device");
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult PushTestNoticationToIPhone()
        {

            try
            {
                var notification = new QueuedNotification()
                {
                 DeviceType = DeviceType.iPhone,
                 SubscriptionId = "a014b1c41ee42ad643540ee5f499ace36836f2d2f8f7699cb627d5bc2061b620",
                 Message = "Hello, iPhone.....This is  push notification testing from NopCommerce Team! ",

                };
                _queuedNotificationService.SendTestNotication(notification);
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }
            
            return Content("Sent successfully!");
        }

        [HttpGet]
        public ActionResult PushTestNoticationToAndroid()
        {

            try
            {
                var notification = new QueuedNotification()
                {
                    DeviceType = DeviceType.Android,
                    SubscriptionId = "a014b1c41ee42ad643540ee5f499ace36836f2d2f8f7699cb627d5bc2061b620",
                    Message = "Hello, Android.....This is  push notification testing from NopCommerce Team! ",

                };
                _queuedNotificationService.SendTestNotication(notification);
            }
            catch (Exception ex)
            {

                return Content(ex.Message);
            }

            return Content("Sent successfully!");
        }
        #endregion
    }
}

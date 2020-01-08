using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Messages;
using Nop.Core.Domain.Messages;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Extensions;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.Services;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    [AdminAuthorize]
    public partial class BsNotificationMessageTemplateController : BasePluginController
    {
        #region Fields

        private readonly INotificationMessageTemplateService _notificationMessageTemplateService;
        private readonly IEmailAccountService _emailAccountService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ILocalizationService _localizationService;
        private readonly IMessageTokenProvider _messageTokenProvider;
        private readonly IPermissionService _permissionService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly EmailAccountSettings _emailAccountSettings;

        #endregion Fields

        #region Constructors

        public BsNotificationMessageTemplateController(INotificationMessageTemplateService messageTemplateService, 
            IEmailAccountService emailAccountService,
            ILanguageService languageService, 
            ILocalizedEntityService localizedEntityService,
            ILocalizationService localizationService, 
            IMessageTokenProvider messageTokenProvider, 
            IPermissionService permissionService,
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            IWorkflowMessageService workflowMessageService,
            EmailAccountSettings emailAccountSettings)
        {
            this._notificationMessageTemplateService = messageTemplateService;
            this._emailAccountService = emailAccountService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._localizationService = localizationService;
            this._messageTokenProvider = messageTokenProvider;
            this._permissionService = permissionService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._workflowMessageService = workflowMessageService;
            this._emailAccountSettings = emailAccountSettings;
        }

        #endregion
        
        #region Utilities

        /// <summary>
        /// Save selected TAB index
        /// </summary>
        /// <param name="index">Idnex to save; null to automatically detect it</param>
        /// <param name="persistForTheNextRequest">A value indicating whether a message should be persisted for the next request</param>
        protected void SaveSelectedTabIndex(int? index = null, bool persistForTheNextRequest = true)
        {
            //keep this method synchronized with
            //"GetSelectedTabIndex" method of \Nop.Web.Framework\ViewEngines\Razor\WebViewPage.cs
            if (!index.HasValue)
            {
                int tmp;
                if (int.TryParse(this.Request.Form["selected-tab-index"], out tmp))
                {
                    index = tmp;
                }
            }
            if (index.HasValue)
            {
                string dataKey = "nop.selected-tab-index";
                if (persistForTheNextRequest)
                {
                    TempData[dataKey] = index;
                }
                else
                {
                    ViewData[dataKey] = index;
                }
            }
        }


        private string FormatTokens(string[] tokens)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < tokens.Length; i++)
            {
                string token = tokens[i];
                sb.Append(token);
                if (i != tokens.Length - 1)
                    sb.Append(", ");
            }

            return sb.ToString();
        }

        [NonAction]
        protected virtual void UpdateLocales(NotificationMessageTemplate mt, NotificationMessageTemplateModel model)
        {
            foreach (var localized in model.Locales)
            {
             
                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Subject,
                                                           localized.Subject,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(mt,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                }
        }


        [NonAction]
        protected virtual void PrepareStoresMappingModel(NotificationMessageTemplateModel model, NotificationMessageTemplate messageTemplate, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            model.AvailableStores = _storeService
                .GetAllStores()
                .Select(s => s.ToModel())
                .ToList();
            if (!excludeProperties)
            {
                if (messageTemplate != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(messageTemplate);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(NotificationMessageTemplate messageTemplate, NotificationMessageTemplateModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(messageTemplate);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(messageTemplate, store.Id);
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
        
        #region Methods

        public ActionResult Index()
        {
            return RedirectToAction("NotificationMessageTemplateList");
        }

        public ActionResult NotificationMessageTemplateList()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

            var model = new NotificationMessageTemplateListModel();
            //stores
            model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var s in _storeService.GetAllStores())
                model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });
            
            return View(model);
        }

        [HttpPost]
        public ActionResult NotificationMessageTemplateList(DataSourceRequest command, MessageTemplateListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

            var messageTemplates = _notificationMessageTemplateService.GetAllNotificationMessageTemplates(model.SearchStoreId);
            var gridModel = new DataSourceResult
            {
                Data = messageTemplates.Select(x =>
                {
                    var templateModel = x.ToModel();
                    PrepareStoresMappingModel(templateModel, x, false);
                    var stores = _storeService
                            .GetAllStores()
                            .Where(s => !x.LimitedToStores || templateModel.SelectedStoreIds.Contains(s.Id))
                            .ToList();
                    for (int i = 0; i < stores.Count; i++)
                    {
                        templateModel.ListOfStores += stores[i].Name;
                        if (i != stores.Count - 1)
                            templateModel.ListOfStores += ", ";
                    }
                    return templateModel;
                }),
                Total = messageTemplates.Count
            };

            return Json(gridModel);
        }


        public ActionResult NotificationMessageTemplateCreate()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

          

            var model = new NotificationMessageTemplateModel();
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());

            //Store
            PrepareStoresMappingModel(model, null, false);
           

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult NotificationMessageTemplateCreate(NotificationMessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {
               var  messageTemplate = model.ToEntity();
                //attached file
               if (!model.HasAttachedDownload)
                   messageTemplate.AttachedDownloadId = 0;
               _notificationMessageTemplateService.InsertNotificationMessageTemplate(messageTemplate);
               //Stores
               SaveStoreMappings(messageTemplate, model);
               //locales
               UpdateLocales(messageTemplate, model);

               SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));

               if (continueEditing)
               {
                   //selected tab
                   SaveSelectedTabIndex();

                   return RedirectToAction("NotificationMessageTemplateEdit", new { id = messageTemplate.Id, area = "" });
               }
                return RedirectToAction("NotificationMessageTemplateList",new {area=""});
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
            //Store
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

     

        public ActionResult NotificationMessageTemplateEdit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("NotificationMessageTemplateList",new {area=""});
            
            var model = messageTemplate.ToModel();
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
           
            //Store
            PrepareStoresMappingModel(model, messageTemplate, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
              
                locale.Subject = messageTemplate.GetLocalized(x => x.Subject, languageId, false, false);
                locale.Body = messageTemplate.GetLocalized(x => x.Body, languageId, false, false);
            });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult NotificationMessageTemplateEdit(NotificationMessageTemplateModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult();

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("NotificationMessageTemplateList", new { area = "" });
            
            if (ModelState.IsValid)
            {
                messageTemplate = model.ToEntity(messageTemplate);
                //attached file
                if (!model.HasAttachedDownload)
                    messageTemplate.AttachedDownloadId = 0;
                _notificationMessageTemplateService.UpdateNotificationMessageTemplate(messageTemplate);
                //Stores
                SaveStoreMappings(messageTemplate, model);
                //locales
                UpdateLocales(messageTemplate, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Updated"));
                
                if (continueEditing)
                {
                    //selected tab
                    SaveSelectedTabIndex();

                    return RedirectToAction("NotificationMessageTemplateEdit", new { id = messageTemplate.Id });
                }
                return RedirectToAction("NotificationMessageTemplateList", new { area = "" });
            }


            //If we got this far, something failed, redisplay form
            model.HasAttachedDownload = model.AttachedDownloadId > 0;
            model.AllowedTokens = FormatTokens(_messageTokenProvider.GetListOfAllowedTokens());
          
            //Store
            PrepareStoresMappingModel(model, messageTemplate, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult(); 

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("NotificationMessageTemplateList", new { area = "" });

            _notificationMessageTemplateService.DeleteNotificationMessageTemplate(messageTemplate);
            
            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.MessageTemplates.Deleted"));
            return RedirectToAction("NotificationMessageTemplateList", new { area = "" });
        }

        [HttpPost, ActionName("NotificationMessageTemplateEdit")]
        [FormValueRequired("message-template-copy")]
        public ActionResult CopyTemplate(NotificationMessageTemplateModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageMessageTemplates))
                return new HttpUnauthorizedResult(); ;

            var messageTemplate = _notificationMessageTemplateService.GetNotificationMessageTemplateById(model.Id);
            if (messageTemplate == null)
                //No message template found with the specified id
                return RedirectToAction("NotificationMessageTemplateList", new { area = "" });

            try
            {
                var newMessageTemplate = _notificationMessageTemplateService.CopyNotificationMessageTemplate(messageTemplate);
                SuccessNotification("The message template has been copied successfully");
                return RedirectToAction("NotificationMessageTemplateEdit", new { id = newMessageTemplate.Id, area = "" });
            }
            catch (Exception exc)
            {
                ErrorNotification(exc.Message);
                return RedirectToAction("NotificationMessageTemplateEdit", new { id = model.Id, area = "" });
            }
        }

       

        #endregion
    }
}

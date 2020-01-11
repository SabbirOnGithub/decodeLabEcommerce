using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Plugins;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Services.Media;
using Nop.Services.Vendors;
using Nop.Core.Domain.Catalog;
using Nop.Services.Seo;
using BS.Plugin.NopStation.MobileWebApi.Models.NstSettingsModel;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using BS.Plugin.NopStation.MobileWebApi.Models;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Services.Events;
using BS.Plugin.NopStation.MobileWebApi.Models.HomePage;
using Nop.Core.Caching;
using BS.Plugin.NopStation.MobileWebApi.Models.Slider;
using Newtonsoft.Json;
using Nop.Core.Domain.Media;
using Nop.Services.Directory;
using Nop.Services.Tax;
using Nop.Web.Models.Media;
using MappingExtensions = BS.Plugin.NopStation.MobileWebApi.Extensions.MappingExtensions;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{

    public class MobileWebApiConfigurationController : BasePluginController
    {
        #region Fields

        private const string IconFilePath = "Plugins/NopStation.MobileWebApi/Content/IconPackage";

        private readonly IWorkContext _workContext;
        private readonly IBsNopMobilePluginService _nopMobileService;
        private readonly IPluginFinder _pluginFinder;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly MobileWebApiSettings _webApiSettings;
        private readonly ISettingService _settingService;
        private readonly IPermissionService _permissionService;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IContentManagementTemplateService _contentManagementTemplateService;
        private readonly IContentManagementService _contentManagementService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IWebHelper _webHelper;
        private readonly ICategoryIconService _categoryIconService;
        private readonly IBS_SliderService _bsSliderService;
        private readonly ApiSettings _apiSettings;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheManager _cacheManager;
        private readonly IBS_HomePageCategoryService _homePageCategoryService;
        private readonly IBS_SliderService _sliderService;
        private readonly MediaSettings _mediaSettings;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly CatalogSettings _catalogSettings;

        #endregion

        #region Ctor

        public MobileWebApiConfigurationController(IWorkContext workContext,
            IBsNopMobilePluginService nopMobileService,
            IWebHelper webHelper,
            IProductService productService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IPluginFinder pluginFinder,
            MobileWebApiSettings webApiSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            IStoreContext storeContext,
            ICurrencyService currencyService,
            ILocalizationService localizationService,
            IStoreService storeService,
            CatalogSettings catalogSettings,
            IVendorService vendorService,
            IPictureService pictureService,
            IContentManagementTemplateService contentManagementTemplateService,
            IContentManagementService contentManagementService,
            ILanguageService languageService,
            ILocalizedEntityService localizedEntityService,
            IStoreMappingService storeMappingService,
            IUrlRecordService urlRecordService,
            ICategoryIconService categoryIconService,
            IPriceCalculationService priceCalculationService,
            IBS_SliderService bsSliderService,
            ApiSettings apiSettings,
            IEventPublisher eventPublisher,
            IPriceFormatter priceFormatter,
            ICacheManager cacheManager,
            IBS_HomePageCategoryService homePageCategoryService,
            IBS_SliderService sliderService,
            ITaxService taxService,
            MediaSettings mediaSettings)
        {
            this._categoryService = categoryService;
            this._vendorService = vendorService;
            this._manufacturerService = manufacturerService;
            this._workContext = workContext;
            this._webHelper = webHelper;
            this._nopMobileService = nopMobileService;
            this._pluginFinder = pluginFinder;
            this._webApiSettings = webApiSettings;
            this._localizationService = localizationService;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._priceCalculationService = priceCalculationService;
            this._storeContext = storeContext;
            this._currencyService = currencyService;
            this._storeService = storeService;
            this._pictureService = pictureService;
            this._productService = productService;
            this._catalogSettings = catalogSettings;
            this._contentManagementTemplateService = contentManagementTemplateService;
            this._contentManagementService = contentManagementService;
            this._languageService = languageService;
            this._localizedEntityService = localizedEntityService;
            this._storeMappingService = storeMappingService;
            this._urlRecordService = urlRecordService;
            this._categoryIconService = categoryIconService;
            this._bsSliderService = bsSliderService;
            this._apiSettings = apiSettings;
            this._eventPublisher = eventPublisher;
            this._cacheManager = cacheManager;
            this._homePageCategoryService = homePageCategoryService;
            this._sliderService = sliderService;
            this._mediaSettings = mediaSettings;
            this._priceFormatter = priceFormatter;
            this._taxService = taxService;
        }

        #endregion

        #region Utilities

        public void UpdateHomepageItems()
        {
            var homePageModel = new HomePageResponseModel();
            homePageModel.Language = PrepareLanguageModel();
            homePageModel.Banners = PrepareBannerModel();// ok
            homePageModel.EventBanners = PrepareEventBannerModel();
            homePageModel.BannerIsEnabled = homePageModel.Banners.Any(); //ok
            homePageModel.Categories = PrepareCategories(); // ok
            homePageModel.CategoriesWithProducts = PrepareCategoriesWithProductsModel(); //ok
            homePageModel.CategoryListIcon = _pictureService.GetPictureUrl(_apiSettings.CategoryListIconId);
            homePageModel.Icons = PrepareIconModel();
            homePageModel.ManufacturerListIcon = _pictureService.GetPictureUrl(_apiSettings.ManufacturerListIconId);
            homePageModel.Manufacturers = PrepareManufacturersModel();

            WriteHomePageJson(homePageModel);
            WriteCurrencyJson();
        }

        protected void WriteHomePageJson(HomePageResponseModel homePageModel)
        {
            var json = JsonConvert.SerializeObject(homePageModel);

            var nfilePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-json.json");
            if (!System.IO.File.Exists(nfilePath))
                System.IO.File.Create(nfilePath).Dispose();
            System.IO.File.WriteAllText(nfilePath, json);

            nfilePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-backup-json.json");
            if (!System.IO.File.Exists(nfilePath))
                System.IO.File.Create(nfilePath).Dispose();
            System.IO.File.WriteAllText(nfilePath, json);

        }

        protected void WriteCurrencyJson()
        {
            var model = new List<CurrencyNavModel>();
            var currencies = _currencyService.GetAllCurrencies();
            foreach (var currency in currencies)
            {
                var cm = new CurrencyNavModel();
                cm.CurrencySymbol = currency.CurrencyCode;
                cm.Id = currency.Id;
                cm.Name = currency.Name;

                model.Add(cm);
            }

            var curJson = JsonConvert.SerializeObject(model);

            var filePath = CommonHelper.MapPath("~/ApiJson/currency-json.json");
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Dispose();
            System.IO.File.WriteAllText(filePath, curJson);

            filePath = CommonHelper.MapPath("~/ApiJson/currency-backup-json.json");
            if (!System.IO.File.Exists(filePath))
                System.IO.File.Create(filePath).Dispose();
            System.IO.File.WriteAllText(filePath, curJson);
        }

        protected IList<MenufactureOverViewModelApi> PrepareManufacturersModel()
        {

            var allModel = new List<MenufactureOverViewModelApi>();
            var allManufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in allManufacturers)
            {
                var modelMan = MappingExtensions.MapTo<Manufacturer, MenufactureOverViewModelApi>(manufacturer);

                //prepare picture model
                int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                modelMan.DefaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, pictureSize)
                };
                allModel.Add(modelMan);
            }

            


            var model = new List<MenufactureOverViewModelApi>();
            var manufacturers = _manufacturerService.GetAllManufacturersDisplayedOnHomePage();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = MappingExtensions.MapTo<Manufacturer, MenufactureOverViewModelApi>(manufacturer);

                //prepare picture model
                int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                modelMan.DefaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, pictureSize)
                };
                model.Add(modelMan);
            }
            return model;
        }

        protected IconModel PrepareIconModel()
        {
            var model = new IconModel();
            model.ShowCategoriesIcon = _apiSettings.ShowHomePageTopCategoryListIcon;
            if (_apiSettings.ShowHomePageTopCategoryListIcon)
            {
                model.CategoriesIconUrl = _pictureService.GetPictureUrl(_apiSettings.CategoryListIconId);
            }
            model.ShowManufacturersIcon = _apiSettings.ShowHomePageTopManufacturersIcon;
            if (_apiSettings.ShowHomePageTopManufacturersIcon)
            {
                model.ManufacturersIconUrl = _pictureService.GetPictureUrl(_apiSettings.ManufacturerListIconId);
            }
            var catIcons = _categoryIconService.GetAllCategoryIcons().ToList();
            foreach (var caticon in catIcons)
            {
                var category = _categoryService.GetCategoryById(caticon.CategoryId);
                if (category == null || category.Deleted || !category.Published)
                    continue;

                model.CategoryIcons.Add(new IconModel.CategoryIconModel
                {
                    CategoryId = caticon.CategoryId,
                    CategoryName = string.IsNullOrWhiteSpace(caticon.TextPrompt) ? category.Name : caticon.TextPrompt,
                    IconUrl = _pictureService.GetPictureUrl(caticon.PictureId)
                });
            }

            return model;
        }

        protected IList<HomePageCategoryWithProductsModel> PrepareCategoriesWithProductsModel()
        {
            var homePageCategories = _homePageCategoryService.GetAllHomePageCategories(publish: true);
            var model = new List<HomePageCategoryWithProductsModel>();
            foreach (var hpc in homePageCategories.Where(x => x.HomePageCategoryProducts.Count > 0))
            {
                var picture = _pictureService.GetPictureById(hpc.PictureId);

                var hpm = new HomePageCategoryWithProductsModel()
                {
                    CategoryId = hpc.CategoryId,
                    TextPrompt = hpc.TextPrompt,
                    Id = hpc.Id,
                    ApplicableFor = hpc.ApplicableFor,
                    IconUrl = _pictureService.GetPictureUrl(hpc.PictureId)
                };

                var products = _productService.GetProductsByIds(hpc.HomePageCategoryProducts.OrderBy(x => x.DisplayOrder).Select(x => x.ProductId).ToArray());

                if (products == null || products.Count == 0)
                    continue;

                hpm.Products = PrepareProductOverviewModels(products).ToList();
                model.Add(hpm);
            }

            return model;
        }

        protected virtual IEnumerable<ProductOverViewModelApi> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            return products.PrepareProductOverviewModels(_workContext,
                _storeContext, _categoryService, _productService,
                _priceCalculationService, _priceFormatter, _permissionService,
                _localizationService, _taxService, _currencyService,
                _pictureService, _webHelper, _cacheManager,
                _catalogSettings, _mediaSettings,
                preparePriceModel, preparePictureModel,
                productThumbPictureSize, prepareSpecificationAttributes);
        }

        protected IList<CategoryNavigationModelApi> PrepareCategories()
        {
            var cmodel = new List<CategoryNavigationModelApi>();
            var cats = _categoryService.GetAllCategoriesDisplayedOnHomePage(ignoreAcl: true);

            foreach (var item in cats)
            {
                var categoryNavigationModelApi = new CategoryNavigationModelApi
                {
                    Id = item.Id,
                    ParentCategoryId = item.ParentCategoryId,
                    Name = item.Name,
                    DisplayOrder = item.DisplayOrder,
                };

                int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                var picture = _pictureService.GetPictureById(item.PictureId);

                categoryNavigationModelApi.ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize);
                cmodel.Add(categoryNavigationModelApi);
            }
            return cmodel;
        }


        protected IList<HomePageBannerResponseModel.BannerModel> PrepareBannerModel()
        {
            var sliders = _sliderService.GetActiveBSSliderImages();

            var pictureList = (from sliderDomain in sliders
                select new HomePageBannerResponseModel.BannerModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sliderDomain.PictureId),
                    DomainType = sliderDomain.SliderDomainTypeId,
                    DomainId = sliderDomain.DomainId,
                    //IsProduct = sliderDomain.IsProduct,
                    //ProdOrCatId = sliderDomain.ProdOrCatId,
                    Link = "",
                    Text = ""
                }).ToList();

            return pictureList;
        }

        protected IList<HomePageBannerResponseModel.BannerModel> PrepareEventBannerModel()
        {
            var sliders = _sliderService.GetActiveBSSliderImages().Where(x=>x.IsEventBanner);

            var pictureList = (from sliderDomain in sliders
                select new HomePageBannerResponseModel.BannerModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sliderDomain.PictureId),
                    DomainType = sliderDomain.SliderDomainTypeId,
                    DomainId = sliderDomain.DomainId,
                    //IsProduct = sliderDomain.IsProduct,
                    //ProdOrCatId = sliderDomain.ProdOrCatId,
                    Link = "",
                    Text = ""
                }).ToList();

            return pictureList;
        }

        protected LanguageNavSelectorModel PrepareLanguageModel()
        {
            var availableLanguages = _cacheManager.Get(string.Format(ModelCacheEventConsumer.AVAILABLE_LANGUAGES_MODEL_KEY, _storeContext.CurrentStore.Id), () =>
            {
                var languages = _languageService
                    .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                    .Select(x => new LanguageNavModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        FlagImageFileName = x.FlagImageFileName,
                    })
                    .ToList();
                return languages;
            });

            var language = new LanguageNavSelectorModel();
            language.AvailableLanguages = availableLanguages;

            return language;
        }

        protected SliderImageModel PrepareSliderImageModel(BS_Slider slider = null, SliderImageModel model = null, 
            bool includeAvailableDomainTypes = false, bool preparePictureUrl = false)
        {
            model = model ?? new SliderImageModel();

            if (slider != null)
            {
                model.DomainId = slider.DomainId;
                model.Id = slider.Id;
                model.PictureId = slider.PictureId;
                model.SliderActiveEndDate = slider.SliderActiveEndDate;
                model.SliderActiveStartDate = slider.SliderActiveStartDate;
                model.SliderDomainTypeId = slider.SliderDomainTypeId;
                model.DisplayOrder = slider.DisplayOrder;
                model.IsEventBanner = slider.IsEventBanner;
                model.SliderDomainTypeStr = (int)slider.SliderDomainType == 0 ? "" : slider.SliderDomainType.ToString();
            }
            if (preparePictureUrl)
            {
                model.PictureUrl = _pictureService.GetPictureUrl(slider.PictureId, 150);
            }

            if (includeAvailableDomainTypes)
            {
                model.AvailableSliderDomainTypes = SliderDomainType.Product.ToSelectList().ToList();
                model.AvailableSliderDomainTypes.Insert(0, new SelectListItem { Value = "0", Text = "Select type" });
            }

            return model;
        }

        protected CategoryIconsModel PrepareCategoryIconModel(BS_CategoryIcon categoryIcon)
        {
            var model = new CategoryIconsModel();
            var category = _categoryService.GetCategoryById(categoryIcon.CategoryId);
            model.CategoryId = categoryIcon.CategoryId;
            model.CategoryName = category == null ? "" : category.Name;
            model.DisplayOrder = categoryIcon.DisplayOrder;
            model.Id = categoryIcon.Id;
            model.PictureId = categoryIcon.PictureId;
            model.TextPrompt = categoryIcon.TextPrompt;
            model.PictureUrl = _pictureService.GetPictureUrl(categoryIcon.PictureId);
            return model;
        }

        protected object LoadAvailableCategoryProducts(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };

            var products = _productService.SearchProducts(categoryIds: categoryIds);
            var availableProducts = products.OrderBy(x => x.Name).Select(x => new
            {
                id = x.Id.ToString(),
                value = x.Name
            }).ToList();

            return availableProducts;
        }

        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId, true);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        [NonAction]
        protected List<SelectListItem> GetCategoryList()
        {
            string cacheKey = string.Format("Nop.pres.admin.categories.list-{0}", true);
            var categoryListItems = _cacheManager.Get(cacheKey, () =>
            {
                var categories = _categoryService.GetAllCategories(showHidden: true);
                return categories.Select(c => new SelectListItem
                {
                    Text = c.GetFormattedBreadCrumb(categories) + " (" + c.Id + ")",
                    Value = c.Id.ToString()
                });
            }).ToList();

            return categoryListItems;
        }


        [NonAction]
        protected virtual void PrepareTemplatesModel(TopicModel model)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            var templates = _contentManagementTemplateService.GetAllTopicTemplates();
            foreach (var template in templates)
            {
                model.AvailableTopicTemplates.Add(new SelectListItem
                {
                    Text = template.Name,
                    Value = template.Id.ToString()
                });
            }
        }

        [NonAction]
        protected virtual void UpdateLocales(BS_ContentManagement topic, TopicModel model)
        {
            foreach (var localized in model.Locales)
            {
                _localizedEntityService.SaveLocalizedValue(topic,
                                                               x => x.Title,
                                                               localized.Title,
                                                               localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.Body,
                                                           localized.Body,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaKeywords,
                                                           localized.MetaKeywords,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaDescription,
                                                           localized.MetaDescription,
                                                           localized.LanguageId);

                _localizedEntityService.SaveLocalizedValue(topic,
                                                           x => x.MetaTitle,
                                                           localized.MetaTitle,
                                                           localized.LanguageId);

                //search engine name
                var seName = topic.ValidateSeName(localized.SeName, localized.Title, false);
                _urlRecordService.SaveSlug(topic, seName, localized.LanguageId);
            }
        }

        [NonAction]
        protected virtual void PrepareStoresMappingModel(TopicModel model, BS_ContentManagement topic, bool excludeProperties)
        {
            if (model == null)
                throw new ArgumentNullException("model");

            //model.AvailableStores = _storeService.GetAllStores().Select(s => s.ToModel())
            //    .ToList();
            if (!excludeProperties)
            {
                if (topic != null)
                {
                    model.SelectedStoreIds = _storeMappingService.GetStoresIdsWithAccess(topic);
                }
            }
        }

        [NonAction]
        protected virtual void SaveStoreMappings(BS_ContentManagement topic, TopicModel model)
        {
            var existingStoreMappings = _storeMappingService.GetStoreMappings(topic);
            var allStores = _storeService.GetAllStores();
            foreach (var store in allStores)
            {
                if (model.SelectedStoreIds != null && model.SelectedStoreIds.Contains(store.Id))
                {
                    //new store
                    if (existingStoreMappings.Count(sm => sm.StoreId == store.Id) == 0)
                        _storeMappingService.InsertStoreMapping(topic, store.Id);
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

        [NonAction]
        private List<SelectListItem> GetSubCategories()
        {
            var categories = _categoryService.GetAllCategories();

            var parentCategories = categories.Where(w => w.ParentCategoryId == 0).ToList();

            var subCategories = categories.Join(parentCategories, p => p.ParentCategoryId, q => q.Id, (p, q) => new { p }).ToList();

            var model = subCategories.Select(s => new SelectListItem()
            {
                Text = s.p.Name,
                Value = s.p.Id.ToString()

            }).ToList();

            return model;
        }

        #endregion

        #region Methods

        #region Config

        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ConfigureModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.AndroidAppStatus = BsNopMobileSettings.AndroidAppStatus;
            model.AppKey = BsNopMobileSettings.AppKey;
            model.AppName = BsNopMobileSettings.AppName;
            model.CreatedDate = BsNopMobileSettings.CreatedDate;
            model.DownloadUrl = BsNopMobileSettings.DownloadUrl;
            model.iOsAPPUDID = BsNopMobileSettings.iOsAPPUDID;
            model.MobilWebsiteURL = BsNopMobileSettings.MobilWebsiteURL;


            if (storeScope > 0)
            {
                model.AndroidAppStatus_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);
                model.AndroidAppStatus_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);
                model.AppKey_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppKey, storeScope);
                model.AppName_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppName, storeScope);
                model.CreatedDate_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.CreatedDate, storeScope);
                model.DownloadUrl_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.DownloadUrl, storeScope);
                model.iOsAPPUDID_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope);
                model.MobilWebsiteURL_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope);
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Configure.cshtml", model);
            // return RedirectToAction("GeneralSetting");
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult Configure(ConfigureModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.AndroidAppStatus = model.AndroidAppStatus;
            BsNopMobileSettings.AppKey = model.AppKey;
            BsNopMobileSettings.AppName = model.AppName;
            BsNopMobileSettings.CreatedDate = model.CreatedDate;
            BsNopMobileSettings.DownloadUrl = model.DownloadUrl;
            BsNopMobileSettings.iOsAPPUDID = model.iOsAPPUDID;
            BsNopMobileSettings.MobilWebsiteURL = model.MobilWebsiteURL;
            BsNopMobileSettings.BSMobSetVers++;


            if (model.AndroidAppStatus_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AndroidAppStatus, storeScope);

            if (model.AppKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppKey, storeScope);

            if (model.AppName_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppName, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppName, storeScope);

            if (model.CreatedDate_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.CreatedDate, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.CreatedDate, storeScope);

            if (model.DownloadUrl_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.DownloadUrl, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.DownloadUrl, storeScope);

            if (model.iOsAPPUDID_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.iOsAPPUDID, storeScope);

            if (model.MobilWebsiteURL_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.MobilWebsiteURL, storeScope);


            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Configure.cshtml", model);
        }

        public ActionResult MobileWebSiteSetting()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new MobileSettingsModel();

            model.ActiveStoreScopeConfiguration = storeScope;
            model.ActivatePushNotification = BsNopMobileSettings.ActivatePushNotification;
            model.SandboxMode = BsNopMobileSettings.SandboxMode;
            model.GcmApiKey = BsNopMobileSettings.GcmApiKey;
            model.GoogleApiProjectNumber = BsNopMobileSettings.GoogleApiProjectNumber;
            model.UploudeIOSPEMFile = BsNopMobileSettings.UploudeIOSPEMFile;
            model.PEMPassword = BsNopMobileSettings.PEMPassword;
            model.AppNameOnGooglePlayStore = BsNopMobileSettings.AppNameOnGooglePlayStore;
            model.AppUrlOnGooglePlayStore = BsNopMobileSettings.AppUrlOnGooglePlayStore;
            model.AppNameOnAppleStore = BsNopMobileSettings.AppNameOnAppleStore;
            model.AppUrlonAppleStore = BsNopMobileSettings.AppUrlonAppleStore;
            model.AppDescription = BsNopMobileSettings.AppDescription;
            model.AppImage = BsNopMobileSettings.AppImage;
            model.AppLogo = BsNopMobileSettings.AppLogo;
            model.AppLogoAltText = BsNopMobileSettings.AppLogoAltText;


            if (storeScope > 0)
            {
                model.ActivatePushNotification_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope);
                model.SandboxMode_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.SandboxMode, storeScope);
                model.GoogleApiProjectNumber_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope);
                model.UploudeIOSPEMFile_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope);
                model.PEMPassword_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PEMPassword, storeScope);
                model.AppNameOnGooglePlayStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope);
                model.GcmApiKey_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.GcmApiKey, storeScope);
                model.AppUrlOnGooglePlayStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope);
                model.AppNameOnAppleStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope);
                model.AppUrlonAppleStore_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope);
                model.AppDescription_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppDescription, storeScope);
                model.AppImage_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppImage, storeScope);
                model.AppLogo_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppLogo, storeScope);
                model.AppLogoAltText_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.AppLogoAltText, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/MobileWebSiteSetting.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult MobileWebSiteSetting(MobileSettingsModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.ActivatePushNotification = model.ActivatePushNotification;
            BsNopMobileSettings.SandboxMode = model.SandboxMode;
            BsNopMobileSettings.GcmApiKey = model.GcmApiKey;
            BsNopMobileSettings.GoogleApiProjectNumber = model.GoogleApiProjectNumber;
            BsNopMobileSettings.UploudeIOSPEMFile = model.UploudeIOSPEMFile;
            BsNopMobileSettings.PEMPassword = model.PEMPassword;
            BsNopMobileSettings.AppNameOnGooglePlayStore = model.AppNameOnGooglePlayStore;
            BsNopMobileSettings.AppUrlOnGooglePlayStore = model.AppUrlOnGooglePlayStore;
            BsNopMobileSettings.AppNameOnAppleStore = model.AppNameOnAppleStore;
            BsNopMobileSettings.AppUrlonAppleStore = model.AppUrlonAppleStore;
            BsNopMobileSettings.AppDescription = model.AppDescription;
            BsNopMobileSettings.AppImage = model.AppImage;
            BsNopMobileSettings.AppLogo = model.AppLogo;
            BsNopMobileSettings.AppLogoAltText = model.AppLogoAltText;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.ActivatePushNotification_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.ActivatePushNotification, storeScope);

            if (model.SandboxMode_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.SandboxMode, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.SandboxMode, storeScope);

            if (model.GcmApiKey_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.GcmApiKey, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.GcmApiKey, storeScope);

            if (model.GoogleApiProjectNumber_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.GoogleApiProjectNumber, storeScope);

            if (model.UploudeIOSPEMFile_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.UploudeIOSPEMFile, storeScope);

            if (model.PEMPassword_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PEMPassword, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PEMPassword, storeScope);

            if (model.AppNameOnGooglePlayStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppNameOnGooglePlayStore, storeScope);

            if (model.AppUrlOnGooglePlayStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppUrlOnGooglePlayStore, storeScope);

            if (model.AppNameOnAppleStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppNameOnAppleStore, storeScope);

            if (model.AppUrlonAppleStore_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppUrlonAppleStore, storeScope);

            if (model.AppDescription_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppDescription, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppDescription, storeScope);

            if (model.AppImage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppImage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppImage, storeScope);

            if (model.AppLogo_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppLogo, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppLogo, storeScope);

            if (model.AppLogoAltText_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.AppLogoAltText, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.AppLogoAltText, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/MobileWebSiteSetting.cshtml", model);
        }

        public ActionResult GeneralSetting()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new GeneralSettingModel();

            model.EnableBestseller = BsNopMobileSettings.EnableBestseller;
            model.EnableFeaturedProducts = BsNopMobileSettings.EnableFeaturedProducts;
            model.EnableNewProducts = BsNopMobileSettings.EnableNewProducts;

            if (storeScope > 0)
            {
                model.EnableBestseller_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableBestseller, storeScope);
                model.EnableFeaturedProducts_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope);
                model.EnableNewProducts_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.EnableNewProducts, storeScope);
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/GeneralSetting.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult GeneralSetting(GeneralSettingModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.EnableBestseller = model.EnableBestseller;
            BsNopMobileSettings.EnableFeaturedProducts = model.EnableFeaturedProducts;
            BsNopMobileSettings.EnableNewProducts = model.EnableNewProducts;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.EnableBestseller_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableBestseller, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableBestseller, storeScope);

            if (model.EnableFeaturedProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableFeaturedProducts, storeScope);

            if (model.EnableNewProducts_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.EnableNewProducts, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.EnableNewProducts, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            //return GeneralSetting();
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/GeneralSetting.cshtml", model);
        }

        #endregion

        #region Category icon

        public ActionResult CategoryIconList()
        {
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIconList.cshtml");
        }

        [HttpPost]
        public ActionResult CategoryIconList(DataSourceRequest command)
        {
            var categoryIcons = _categoryIconService.GetAllCategoryIcons(command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult();
            gridModel.Data = categoryIcons.Select(x => PrepareCategoryIconModel(x));
            gridModel.Total = categoryIcons.TotalCount;

            return Json(gridModel);
        }

        public ActionResult CategoryIconCreate()
        {
            var model = new CategoryIconsModel();

            model.AvailableCategories = _categoryService.GetAllCategories(showHidden: true)
                .Select(x => new SelectListItem()
                {
                    Text = x.GetFormattedBreadCrumb(_categoryService),
                    Value = x.Id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

            model.AvailableCategories.Insert(0, new SelectListItem { Value = "0", Text = "Select category"});

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIconCreate.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [AdminAuthorize]
        public ActionResult CategoryIconCreate(CategoryIconsModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(model.CategoryId);
                if (categoryIcon == null)
                {
                    categoryIcon = new BS_CategoryIcon()
                    {
                        CategoryId = model.CategoryId,
                        DisplayOrder = model.DisplayOrder,
                        PictureId = model.PictureId,
                        TextPrompt = model.TextPrompt
                    };
                    _categoryIconService.InsertCategoryIcon(categoryIcon);
                }
                else
                {
                    categoryIcon.DisplayOrder = model.DisplayOrder;
                    categoryIcon.PictureId = model.PictureId;
                    categoryIcon.TextPrompt = model.TextPrompt;
                    _categoryIconService.UpdateCategoryIcon(categoryIcon);
                }
                UpdateHomepageItems();
                if (continueEditing)
                    return RedirectToAction("CategoryIconEdit", new { id = categoryIcon.Id });

                return RedirectToAction("CategoryIconList");
            }

            model.AvailableCategories = _categoryService.GetAllCategories(showHidden: true)
                .Select(x => new SelectListItem()
                {
                    Text = x.GetFormattedBreadCrumb(_categoryService),
                    Value = x.Id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

            model.AvailableCategories.Insert(0, new SelectListItem { Value = "0", Text = "Select category" });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIconCreate.cshtml", model);
        }


        public ActionResult CategoryIconEdit(int id)
        {
            var categoryIcon = _categoryIconService.GetCategoryIconById(id);
            if (categoryIcon == null)
                throw new NopException("categoryIcon");

            var model = PrepareCategoryIconModel(categoryIcon);

            model.AvailableCategories = _categoryService.GetAllCategories(showHidden: true)
                .Select(x => new SelectListItem()
                {
                    Text = x.GetFormattedBreadCrumb(_categoryService),
                    Value = x.Id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

            model.AvailableCategories.Insert(0, new SelectListItem { Value = "0", Text = "Select category" });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIconEdit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [AdminAuthorize]
        public ActionResult CategoryIconEdit(CategoryIconsModel model, bool continueEditing)
        {
            var categoryIcon = _categoryIconService.GetCategoryIconByCategoryId(model.CategoryId);
            if (categoryIcon == null)
                throw new NopException("categoryIcon");

            if (ModelState.IsValid)
            {
                categoryIcon.DisplayOrder = model.DisplayOrder;
                categoryIcon.PictureId = model.PictureId;
                categoryIcon.TextPrompt = model.TextPrompt;
                _categoryIconService.UpdateCategoryIcon(categoryIcon);
                UpdateHomepageItems();
                if (continueEditing)
                    return RedirectToAction("CategoryIconEdit", new { id = categoryIcon.Id });

                return RedirectToAction("CategoryIconList");
            }

            model.AvailableCategories = _categoryService.GetAllCategories(showHidden: true)
                .Select(x => new SelectListItem()
                {
                    Text = x.GetFormattedBreadCrumb(_categoryService),
                    Value = x.Id.ToString()
                })
                .OrderBy(x => x.Text)
                .ToList();

            model.AvailableCategories.Insert(0, new SelectListItem { Value = "0", Text = "Select category" });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/CategoryIconEdit.cshtml", model);
        }

        [HttpPost]
        public ActionResult CategoryIconDelete(int id)
        {
            var categoryIcon = _categoryIconService.GetCategoryIconById(id);
            _categoryIconService.DeleteCategoryIcon(categoryIcon);
            UpdateHomepageItems();

            return RedirectToAction("CategoryIconList");
        }


        #endregion

        #region Content management

        public ActionResult ContentManagement()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ContentManagementModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.DefaultNopFlowSameAs = BsNopMobileSettings.DefaultNopFlowSameAs;
            
            if (storeScope > 0)
            {
                model.DefaultNopFlowSameAs_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/ContentManagement.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [FormValueRequired("save")]
        public ActionResult ContentManagement(ContentManagementModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.DefaultNopFlowSameAs = model.DefaultNopFlowSameAs;

            if (model.DefaultNopFlowSameAs_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return RedirectToAction("ContentManagement");
        }

        [HttpPost]
        public ActionResult ContentManagementList(DataSourceRequest command, ContentManagementModel model)
        {
            var topicModels = _contentManagementService.GetAllTopics(model.SearchStoreId)
               .Select(x => new TopicModel
               {
                   AccessibleWhenStoreClosed = x.AccessibleWhenStoreClosed,
                   Body = x.Body,
                   DisplayOrder = x.DisplayOrder,
                   IncludeInFooterColumn1 = x.IncludeInFooterColumn1,
                   IncludeInFooterColumn2 = x.IncludeInFooterColumn2,
                   IncludeInFooterColumn3 = x.IncludeInFooterColumn3,
                   IncludeInSitemap = x.IncludeInSitemap,
                   IncludeInTopMenu = x.IncludeInTopMenu,
                   IsPasswordProtected = x.IsPasswordProtected,
                   LimitedToStores = x.LimitedToStores,
                   MetaDescription = x.MetaDescription,
                   MetaKeywords = x.MetaKeywords,
                   MetaTitle = x.MetaTitle,
                   Password = x.Password,
                   SystemName = x.SystemName,
                   Title = x.Title,
                   TopicTemplateId = x.TopicTemplateId,
                   Id = x.Id

               }).ToList();
            //little hack here:
            //we don't have paging supported for topic list page
            //now ensure that topic bodies are not returned. otherwise, we can get the following error:
            //"Error during serialization or deserialization using the JSON JavaScriptSerializer. The length of the string exceeds the value set on the maxJsonLength property. "
            foreach (var topic in topicModels)
            {
                topic.Body = "";
            }
            var gridModel = new DataSourceResult
            {
                Data = topicModels,
                Total = topicModels.Count
            };

            return Json(gridModel);
        }

        #endregion

        #region Create / Edit / Delete

        public ActionResult Create()
        {

            var model = new TopicModel();
            //templates
            PrepareTemplatesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, false);
            //locales
            AddLocales(_languageService, model.Locales);

            //default values
            model.DisplayOrder = 1;

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Create.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(TopicModel model, bool continueEditing)
        {

            if (ModelState.IsValid)
            {
                if (!model.IsPasswordProtected)
                {
                    model.Password = null;
                }
                BS_ContentManagement topic = new BS_ContentManagement()
                {
                    AccessibleWhenStoreClosed = model.AccessibleWhenStoreClosed,
                    Body = model.Body,
                    DisplayOrder = model.DisplayOrder,
                    IncludeInFooterColumn1 = model.IncludeInFooterColumn1,
                    IncludeInFooterColumn2 = model.IncludeInFooterColumn2,
                    IncludeInFooterColumn3 = model.IncludeInFooterColumn3,
                    IncludeInSitemap = model.IncludeInSitemap,
                    IncludeInTopMenu = model.IncludeInTopMenu,
                    IsPasswordProtected = model.IsPasswordProtected,
                    LimitedToStores = model.LimitedToStores,
                    MetaDescription = model.MetaDescription,
                    MetaKeywords = model.MetaKeywords,
                    MetaTitle = model.MetaTitle,
                    Password = model.Password,
                    SystemName = model.SystemName,
                    Title = model.Title,
                    TopicTemplateId = model.TopicTemplateId
                };

                _contentManagementService.InsertTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = topic.Id }) : RedirectToAction("ContentManagement");
            }

            //If we got this far, something failed, redisplay form

            //templates
            PrepareTemplatesModel(model);
            //Stores
            PrepareStoresMappingModel(model, null, true);
            return View(model);
        }

        public ActionResult Edit(int id)
        {
            var topic = _contentManagementService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            //var model = topic.ToModel();

            TopicModel model = new TopicModel()
            {
                AccessibleWhenStoreClosed = topic.AccessibleWhenStoreClosed,
                Body = topic.Body,
                DisplayOrder = topic.DisplayOrder,
                IncludeInFooterColumn1 = topic.IncludeInFooterColumn1,
                IncludeInFooterColumn2 = topic.IncludeInFooterColumn2,
                IncludeInFooterColumn3 = topic.IncludeInFooterColumn3,
                IncludeInSitemap = topic.IncludeInSitemap,
                IncludeInTopMenu = topic.IncludeInTopMenu,
                IsPasswordProtected = topic.IsPasswordProtected,
                LimitedToStores = topic.LimitedToStores,
                MetaDescription = topic.MetaDescription,
                MetaKeywords = topic.MetaKeywords,
                MetaTitle = topic.MetaTitle,
                Password = topic.Password,
                SystemName = topic.SystemName,
                Title = topic.Title,
                TopicTemplateId = topic.TopicTemplateId
            };
            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //Store
            PrepareStoresMappingModel(model, topic, false);
            //locales
            AddLocales(_languageService, model.Locales, (locale, languageId) =>
            {
                locale.Title = topic.GetLocalized(x => x.Title, languageId, false, false);
                locale.Body = topic.GetLocalized(x => x.Body, languageId, false, false);
                locale.MetaKeywords = topic.GetLocalized(x => x.MetaKeywords, languageId, false, false);
                locale.MetaDescription = topic.GetLocalized(x => x.MetaDescription, languageId, false, false);
                locale.MetaTitle = topic.GetLocalized(x => x.MetaTitle, languageId, false, false);
                locale.SeName = topic.GetSeName(languageId, false, false);
            });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Edit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Edit(TopicModel model, bool continueEditing)
        {

            var topic = _contentManagementService.GetTopicById(model.Id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            if (!model.IsPasswordProtected)
            {
                model.Password = null;
            }

            if (ModelState.IsValid)
            {
                //topic = model.ToEntity(topic);

                topic.AccessibleWhenStoreClosed = model.AccessibleWhenStoreClosed;
                topic.Body = model.Body;
                topic.DisplayOrder = model.DisplayOrder;
                topic.IncludeInFooterColumn1 = model.IncludeInFooterColumn1;
                topic.IncludeInFooterColumn2 = model.IncludeInFooterColumn2;
                topic.IncludeInFooterColumn3 = model.IncludeInFooterColumn3;
                topic.IncludeInSitemap = model.IncludeInSitemap;
                topic.IncludeInTopMenu = model.IncludeInTopMenu;
                topic.IsPasswordProtected = model.IsPasswordProtected;
                topic.LimitedToStores = model.LimitedToStores;
                topic.MetaDescription = model.MetaDescription;
                topic.MetaKeywords = model.MetaKeywords;
                topic.MetaTitle = model.MetaTitle;
                topic.Password = model.Password;
                topic.SystemName = model.SystemName;
                topic.Title = model.Title;
                topic.TopicTemplateId = model.TopicTemplateId;

                _contentManagementService.UpdateTopic(topic);
                //search engine name
                model.SeName = topic.ValidateSeName(model.SeName, topic.Title ?? topic.SystemName, true);
                _urlRecordService.SaveSlug(topic, model.SeName, 0);
                //Stores
                SaveStoreMappings(topic, model);
                //locales
                UpdateLocales(topic, model);

                SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Updated"));

                if (continueEditing)
                {
                    return RedirectToAction("Edit", new { id = topic.Id });
                }
                return RedirectToAction("ContentManagement");
            }


            //If we got this far, something failed, redisplay form

            model.Url = Url.RouteUrl("Topic", new { SeName = topic.GetSeName() }, "http");
            //templates
            PrepareTemplatesModel(model);
            //Store
            PrepareStoresMappingModel(model, topic, true);
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {

            var topic = _contentManagementService.GetTopicById(id);
            if (topic == null)
                //No topic found with the specified id
                return RedirectToAction("ContentManagement");

            _contentManagementService.DeleteTopic(topic);

            SuccessNotification(_localizationService.GetResource("Admin.ContentManagement.Topics.Deleted"));
            return RedirectToAction("ContentManagement");
        }

        #endregion

        #region Push notification

        public ActionResult PushNotification()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new PushNotificationModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.PushNotificationHeading = BsNopMobileSettings.PushNotificationHeading;
            model.PushNotificationMessage = BsNopMobileSettings.PushNotificationMessage;

            if (storeScope > 0)
            {
                model.PushNotificationHeading_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope);
                model.PushNotificationMessage_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/PushNotification.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult PushNotification(PushNotificationModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.PushNotificationHeading = model.PushNotificationHeading;
            BsNopMobileSettings.PushNotificationMessage = model.PushNotificationMessage;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.PushNotificationHeading_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PushNotificationHeading, storeScope);

            if (model.PushNotificationMessage_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PushNotificationMessage, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/PushNotification.cshtml", model);
        }

        #endregion

        #region Theme

        public ActionResult Theme()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ThemeSettingModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.HeaderBackgroundColor = BsNopMobileSettings.HeaderBackgroundColor;
            model.HeaderFontandIconColor = BsNopMobileSettings.HeaderFontandIconColor;
            model.HighlightedTextColor = BsNopMobileSettings.HighlightedTextColor;
            model.PrimaryTextColor = BsNopMobileSettings.PrimaryTextColor;
            model.SecondaryTextColor = BsNopMobileSettings.SecondaryTextColor;
            model.BackgroundColorofPrimaryButton = BsNopMobileSettings.BackgroundColorofPrimaryButton;
            model.TextColorofPrimaryButton = BsNopMobileSettings.TextColorofPrimaryButton;
            model.BackgroundColorofSecondaryButton = BsNopMobileSettings.BackgroundColorofSecondaryButton;

            if (storeScope > 0)
            {
                model.HeaderBackgroundColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope);
                model.HeaderFontandIconColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope);
                model.HighlightedTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope);
                model.PrimaryTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope);
                model.SecondaryTextColor_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope);
                model.BackgroundColorofPrimaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope);
                model.TextColorofPrimaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope);
                model.BackgroundColorofSecondaryButton_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Theme.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult Theme(ThemeSettingModel model)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);


            BsNopMobileSettings.HeaderBackgroundColor = model.HeaderBackgroundColor;
            BsNopMobileSettings.HeaderFontandIconColor = model.HeaderFontandIconColor;
            BsNopMobileSettings.HighlightedTextColor = model.HighlightedTextColor;
            BsNopMobileSettings.PrimaryTextColor = model.PrimaryTextColor;
            BsNopMobileSettings.SecondaryTextColor = model.SecondaryTextColor;
            BsNopMobileSettings.BackgroundColorofPrimaryButton = model.BackgroundColorofPrimaryButton;
            BsNopMobileSettings.TextColorofPrimaryButton = model.TextColorofPrimaryButton;
            BsNopMobileSettings.BackgroundColorofSecondaryButton = model.BackgroundColorofSecondaryButton;

            BsNopMobileSettings.BSMobSetVers++;
            _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

            if (model.HeaderBackgroundColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HeaderBackgroundColor, storeScope);

            if (model.HeaderFontandIconColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HeaderFontandIconColor, storeScope);

            if (model.HighlightedTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.HighlightedTextColor, storeScope);

            if (model.PrimaryTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.PrimaryTextColor, storeScope);

            if (model.SecondaryTextColor_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.SecondaryTextColor, storeScope);

            if (model.BackgroundColorofPrimaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.BackgroundColorofPrimaryButton, storeScope);

            if (model.TextColorofPrimaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.TextColorofPrimaryButton, storeScope);

            if (model.BackgroundColorofSecondaryButton_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.BackgroundColorofSecondaryButton, storeScope);

            //now clear settings cache
            _settingService.ClearCache();

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/Theme.cshtml", model);
        }

        #endregion

        #region Banner slider

        public ActionResult BannerSlider()
        {

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new BannerSliderModel();
            model.Picture1Id = BsNopMobileSettings.Picture1Id;
            model.Text1 = BsNopMobileSettings.Text1;
            model.Link1 = BsNopMobileSettings.Link1;
            model.IsProduct1 = BsNopMobileSettings.IsProduct1;
            model.ProductOrCategory1 = BsNopMobileSettings.ProductOrCategoryId1;

            model.Picture2Id = BsNopMobileSettings.Picture2Id;
            model.Text2 = BsNopMobileSettings.Text2;
            model.Link2 = BsNopMobileSettings.Link2;
            model.IsProduct2 = BsNopMobileSettings.IsProduct2;
            model.ProductOrCategory2 = BsNopMobileSettings.ProductOrCategoryId2;

            model.Picture3Id = BsNopMobileSettings.Picture3Id;
            model.Text3 = BsNopMobileSettings.Text3;
            model.Link3 = BsNopMobileSettings.Link3;
            model.IsProduct3 = BsNopMobileSettings.IsProduct3;
            model.ProductOrCategory3 = BsNopMobileSettings.ProductOrCategoryId3;

            model.Picture4Id = BsNopMobileSettings.Picture4Id;
            model.Text4 = BsNopMobileSettings.Text4;
            model.Link4 = BsNopMobileSettings.Link4;
            model.IsProduct4 = BsNopMobileSettings.IsProduct4;
            model.ProductOrCategory4 = BsNopMobileSettings.ProductOrCategoryId4;

            model.Picture5Id = BsNopMobileSettings.Picture5Id;
            model.Text5 = BsNopMobileSettings.Text5;
            model.Link5 = BsNopMobileSettings.Link5;
            model.IsProduct5 = BsNopMobileSettings.IsProduct5;
            model.ProductOrCategory5 = BsNopMobileSettings.ProductOrCategoryId5;

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.Picture1Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture1Id,
                    storeScope);
                model.Text1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text1,
                    storeScope);
                model.Link1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link1,
                    storeScope);
                model.IsProduct1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct1,
                    storeScope);
                model.ProductOrCategory1_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId1, storeScope);

                model.Picture2Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture2Id,
                    storeScope);
                model.Text2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text2,
                    storeScope);
                model.Link2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link2,
                    storeScope);
                model.IsProduct2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct2,
                    storeScope);
                model.ProductOrCategory2_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId2, storeScope);

                model.Picture3Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture3Id,
                    storeScope);
                model.Text3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text3,
                    storeScope);
                model.Link3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link3,
                    storeScope);
                model.IsProduct3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct3,
                    storeScope);
                model.ProductOrCategory3_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId3, storeScope);

                model.Picture4Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture4Id,
                    storeScope);
                model.Text4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text4,
                    storeScope);
                model.Link4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link4,
                    storeScope);
                model.IsProduct4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct4,
                    storeScope);
                model.ProductOrCategory4_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId4, storeScope);

                model.Picture5Id_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Picture5Id,
                    storeScope);
                model.Text5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Text5,
                    storeScope);
                model.Link5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Link5,
                    storeScope);
                model.IsProduct5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.IsProduct5,
                    storeScope);
                model.ProductOrCategory5_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings,
                    x => x.ProductOrCategoryId5, storeScope);
            }
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/BannerSlider.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        public ActionResult BannerSlider(BannerSliderModel model)
        {

            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            if (ModelState.IsValid)
            {
                BsNopMobileSettings.Picture1Id = model.Picture1Id;
                BsNopMobileSettings.Text1 = model.Text1;
                BsNopMobileSettings.Link1 = model.Link1;
                BsNopMobileSettings.IsProduct1 = model.IsProduct1;
                BsNopMobileSettings.ProductOrCategoryId1 = model.ProductOrCategory1;

                BsNopMobileSettings.Picture2Id = model.Picture2Id;
                BsNopMobileSettings.Text2 = model.Text2;
                BsNopMobileSettings.Link2 = model.Link2;
                BsNopMobileSettings.IsProduct2 = model.IsProduct2;
                BsNopMobileSettings.ProductOrCategoryId2 = model.ProductOrCategory2;

                BsNopMobileSettings.Picture3Id = model.Picture3Id;
                BsNopMobileSettings.Text3 = model.Text3;
                BsNopMobileSettings.Link3 = model.Link3;
                BsNopMobileSettings.IsProduct3 = model.IsProduct3;
                BsNopMobileSettings.ProductOrCategoryId3 = model.ProductOrCategory3;

                BsNopMobileSettings.Picture4Id = model.Picture4Id;
                BsNopMobileSettings.Text4 = model.Text4;
                BsNopMobileSettings.Link4 = model.Link4;
                BsNopMobileSettings.IsProduct4 = model.IsProduct4;
                BsNopMobileSettings.ProductOrCategoryId4 = model.ProductOrCategory4;

                BsNopMobileSettings.Picture5Id = model.Picture5Id;
                BsNopMobileSettings.Text5 = model.Text5;
                BsNopMobileSettings.Link5 = model.Link5;
                BsNopMobileSettings.IsProduct5 = model.IsProduct5;
                BsNopMobileSettings.ProductOrCategoryId5 = model.ProductOrCategory5;

                BsNopMobileSettings.BSMobSetVers++;
                _settingService.SaveSetting(BsNopMobileSettings, x => x.BSMobSetVers, storeScope, false);

                if (model.Picture1Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture1Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture1Id, storeScope);

                if (model.Text1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text1, storeScope);

                if (model.Link1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link1, storeScope);

                if (model.IsProduct1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct1, storeScope);

                if (model.ProductOrCategory1_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId1, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId1, storeScope);

                if (model.Picture2Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture2Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture2Id, storeScope);

                if (model.Text2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text2, storeScope);

                if (model.Link2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link2, storeScope);

                if (model.IsProduct2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct2, storeScope);

                if (model.ProductOrCategory2_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId2, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId2, storeScope);

                if (model.Picture3Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture3Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture3Id, storeScope);

                if (model.Text3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text3, storeScope);

                if (model.Link3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link3, storeScope);

                if (model.IsProduct3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct3, storeScope);

                if (model.ProductOrCategory3_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId3, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId3, storeScope);

                if (model.Picture4Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture4Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture4Id, storeScope);

                if (model.Text4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text4, storeScope);

                if (model.Link4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link4, storeScope);

                if (model.IsProduct4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct4, storeScope);

                if (model.ProductOrCategory4_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId4, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId4, storeScope);

                if (model.Picture5Id_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Picture5Id, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Picture5Id, storeScope);

                if (model.Text5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Text5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Text5, storeScope);

                if (model.Link5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.Link5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.Link5, storeScope);

                if (model.IsProduct5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.IsProduct5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.IsProduct5, storeScope);

                if (model.ProductOrCategory5_OverrideForStore || storeScope == 0)
                    _settingService.SaveSetting(BsNopMobileSettings, x => x.ProductOrCategoryId5, storeScope, false);
                else if (storeScope > 0)
                    _settingService.DeleteSetting(BsNopMobileSettings, x => x.ProductOrCategoryId5, storeScope);

                //now clear settings cache
                _settingService.ClearCache();
            }

            //redisplay the form

            //redisplay the form
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/BannerSlider.cshtml", model);
        }

        #endregion

        #region Slider image

        public ActionResult SliderImage()
        {
            var model = PrepareSliderImageModel(includeAvailableDomainTypes: true);
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImage.cshtml", model);
        }

        [HttpPost]
        public ActionResult SliderImageList(DataSourceRequest command, SliderImageModel model)
        {
            var sliderImages = _bsSliderService.GetBSSliderImages(
                model.SliderDomainTypeId > 0 ? model.SliderDomainTypeId : (int?)null,
                model.SliderActiveStartDate,
                model.SliderActiveEndDate,
                command.Page - 1,
                command.PageSize);

            var sliderImagesModel = sliderImages
                .Select(x => PrepareSliderImageModel(x, null, false, true))
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = sliderImagesModel,
                Total = sliderImages.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult SliderImageCreate()
        {
            var model = PrepareSliderImageModel(null, null, true);
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImageCreate.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult SliderImageCreate(SliderImageModel model, bool continueEditing)
        {
            if (ModelState.IsValid)
            {
                var slider = new BS_Slider();

                slider.DomainId = model.DomainId;
                slider.PictureId = model.PictureId;
                slider.SliderActiveEndDate = model.SliderActiveEndDate;
                slider.SliderActiveStartDate = model.SliderActiveStartDate;
                slider.SliderDomainTypeId = model.SliderDomainTypeId;
                slider.DisplayOrder = model.DisplayOrder;
                slider.IsEventBanner = model.IsEventBanner;

                _bsSliderService.InsertSlider(slider);
                UpdateHomepageItems();

                if (continueEditing)
                    return RedirectToAction("SliderImageEdit", new { id = slider.Id });

                return RedirectToAction("SliderImage");
            }

            model = PrepareSliderImageModel(null, model, true);

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImageCreate.cshtml", model);
        }

        public ActionResult SliderImageEdit(int id)
        {
            var slider = _bsSliderService.GetBsSliderImageById(id);
            if (slider == null)
                throw new ArgumentException("No slider picture found with the specified id");

            var model = PrepareSliderImageModel(slider, null, true);
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImageEdit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult SliderImageEdit(SliderImageModel model, bool continueEditing)
        {
            var slider = _bsSliderService.GetBsSliderImageById(model.Id);
            if (slider == null)
                throw new ArgumentException("No slider picture found with the specified id");

            if (ModelState.IsValid)
            {
                slider.DomainId = model.DomainId;
                slider.PictureId = model.PictureId;
                slider.SliderActiveEndDate = model.SliderActiveEndDate;
                slider.SliderActiveStartDate = model.SliderActiveStartDate;
                slider.SliderDomainTypeId = model.SliderDomainTypeId;
                slider.DisplayOrder = model.DisplayOrder;
                slider.IsEventBanner = model.IsEventBanner;

                _bsSliderService.UpdateSlider(slider);
                UpdateHomepageItems();

                if (continueEditing)
                    return RedirectToAction("SliderImageEdit", new { id = slider.Id });

                return RedirectToAction("SliderImage");
            }

            model = PrepareSliderImageModel(slider, null, true);
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImageEdit.cshtml", model);
        }

        [HttpPost]
        public ActionResult SliderImageDelete(int id)
        {
            var slider = _bsSliderService.GetBsSliderImageById(id);
            if (slider == null)
                throw new ArgumentException("No category picture found with the specified id");

            var pictureId = slider.PictureId;

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture != null)
                _pictureService.DeletePicture(picture);

            _bsSliderService.DeleteSlider(id);
            UpdateHomepageItems();

            return RedirectToAction("SliderImage");
        }

        #endregion

        #region Event Slider image
        public ActionResult EventSliderImage()
        {
            var model = PrepareSliderImageModel(includeAvailableDomainTypes: true);
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/SliderImage.cshtml", model);
        }


        #endregion

        #region Featured product

        [HttpPost]
        public ActionResult FeaturedProductList(DataSourceRequest command)
        {
            var FeatureProducts = _nopMobileService.GetAllPluginFeatureProducts(command.Page - 1, command.PageSize);

            var productlist = _productService.GetProductsByIds(FeatureProducts.Select(x => x.ProductId).ToArray());

            var featuredProductsModel = productlist
                .Select(x => new GeneralSettingModel.AssociatedProductModel
                {
                    Id = x.Id,
                    ProductName = x.Name
                })
                .ToList();

            var gridModel = new DataSourceResult
            {
                Data = featuredProductsModel,
                Total = featuredProductsModel.Count
            };
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult FeaturedProductDelete(int id)
        {
            var featuredProduct = _nopMobileService.GetPluginFeatureProductsById(id);
            if (featuredProduct == null)
                throw new ArgumentException("No associated product found with the specified id");

            _nopMobileService.DeleteFeatureProducts(featuredProduct);

            return new NullJsonResult();
        }

        public ActionResult FeaturedProductsAddPopup(string btnId)
        {
            var model = new GeneralSettingModel.AddAssociatedProductModel();
            //a vendor should have access only to his products
            model.IsLoggedInAsVendor = _workContext.CurrentVendor != null;

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
            //model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var s in _storeService.GetAllStores())
            //    model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //product types
            model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            ViewBag.btnId = btnId;
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/FeaturedProductsAddPopup.cshtml", model);
        }

        [HttpPost]
        public ActionResult FeaturedProductsAddPopupList(DataSourceRequest command, GeneralSettingModel.AddAssociatedProductModel model)
        {

            if (_workContext.CurrentVendor != null)
            {
                model.SearchVendorId = _workContext.CurrentVendor.Id;
            }

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

            var featuredProductsModel = products
               .Select(x => new GeneralSettingModel.AssociatedProductModel
               {
                   Id = x.Id,
                   ProductName = x.Name,
                   DisplayOrder = x.DisplayOrder,
                   Published = x.Published

               })
               .ToList();

            var gridModel = new DataSourceResult
            {
                Data = featuredProductsModel,
                Total = featuredProductsModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        [FormValueRequired("save")]
        public ActionResult FeaturedProductsAddPopup(string btnId, string formId, GeneralSettingModel.AddAssociatedProductModel model)
        {
            if (model.SelectedProductIds != null)
            {
                var productidlist = _nopMobileService.GetAllPluginFeatureProducts().Where(x => model.SelectedProductIds.Contains(x.ProductId)).Select(x => x.ProductId);
                foreach (int id in model.SelectedProductIds)
                {
                    if (productidlist.Count() > 0)
                    {
                        if (!productidlist.Contains(id))
                        {
                            BS_FeaturedProducts FetureProduct = new BS_FeaturedProducts()
                            {
                                ProductId = id
                            };
                            _nopMobileService.InsertFeatureProducts(FetureProduct);
                        }
                    }
                    else
                    {
                        BS_FeaturedProducts FetureProduct = new BS_FeaturedProducts()
                        {
                            ProductId = id
                        };
                        _nopMobileService.InsertFeatureProducts(FetureProduct);
                    }

                }
            }
            ViewBag.RefreshPage = true;
            ViewBag.btnId = btnId;
            ViewBag.formId = formId;
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/FeaturedProductsAddPopup.cshtml", model);
        }

        #endregion

        #region NSt Settings
        public ActionResult NopStationSecrateToken()
        {
            var model = new Nst_ConfigurationSettings();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
            model.ActiveStoreScopeConfiguration = storeScope;
            model.NST_KEY = nstSettings.NST_KEY;
            model.NST_SECRET = nstSettings.NST_SECRET;
            model.EditMode = false;
            if (storeScope > 0)
            {
                model.NST_KEY_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_KEY, storeScope);
                model.NST_SECRET_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_SECRET, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/NstSettings.cshtml", model);
        }

        public ActionResult NopStationSecrateTokenEdit()
        {
            var model = new Nst_ConfigurationSettings();
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
            model.ActiveStoreScopeConfiguration = storeScope;
            model.NST_KEY = nstSettings.NST_KEY;
            model.NST_SECRET = nstSettings.NST_SECRET;
            model.EditMode = true;
            if (storeScope > 0)
            {
                model.NST_KEY_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_KEY, storeScope);
                model.NST_SECRET_OverrideForStore = _settingService.SettingExists(nstSettings, x => x.NST_SECRET, storeScope);

            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/NstSettingsEdit.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        //[FormValueRequired("save")]
        public ActionResult NopStationSecrateTokenEdit(Nst_ConfigurationSettings model)
        {
            if (model.EditMode)
            {

                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var nstSettings = _settingService.LoadSetting<NstSettingsModel>(storeScope);
                nstSettings.NST_KEY = model.NST_KEY;
                nstSettings.NST_SECRET = model.NST_SECRET;
                _settingService.SaveSettingOverridablePerStore(nstSettings, x => x.NST_KEY, model.NST_KEY_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(nstSettings, x => x.NST_SECRET, model.NST_SECRET_OverrideForStore, storeScope, false);
                //now clear settings cache
                _settingService.ClearCache();

            }
            return RedirectToAction("NopStationSecrateToken", "MobileWebApiConfiguration");
        }
        #endregion

        #region Api settings

        public ActionResult ApiSetting()
        {
            var model = new ApiSettingsModel();
            model.CartThumbPictureSize = _apiSettings.CartThumbPictureSize;
            model.ManufacturerListIconId = _apiSettings.ManufacturerListIconId;
            model.ShowHomePageTopManufacturerListIcon = _apiSettings.ShowHomePageTopManufacturersIcon;
            model.ShowHomePageTopCategoryListIcon = _apiSettings.ShowHomePageTopCategoryListIcon;
            model.CategoryListIconId = _apiSettings.CategoryListIconId;
           
            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/ApiSetting.cshtml", model);
        }

        [HttpPost]
        public ActionResult ApiSetting(ApiSettingsModel model)
        {
            _apiSettings.CartThumbPictureSize = model.CartThumbPictureSize;
            _apiSettings.ManufacturerListIconId = model.ManufacturerListIconId;
            _apiSettings.ShowHomePageTopManufacturersIcon = model.ShowHomePageTopManufacturerListIcon;
            _apiSettings.ShowHomePageTopCategoryListIcon = model.ShowHomePageTopCategoryListIcon;
            _apiSettings.CategoryListIconId = model.CategoryListIconId;

            _settingService.SaveSetting(_apiSettings);

            return RedirectToAction("ApiSetting");
        }
        #endregion

        #region Home page category

        public ActionResult HomePageCategory()
        {
            var model = new HomePageCategoryListModel();

            //categories
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            var categories = GetCategoryList();
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.All"), Value = "0" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.PublishedOnly"), Value = "1" });
            model.AvailablePublishedOptions.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Catalog.Products.List.SearchPublished.UnpublishedOnly"), Value = "2" });

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/HomePageCategory.cshtml", model);
        }


        [HttpPost]
        public ActionResult HomePageCategoryList(DataSourceRequest command, HomePageCategoryListModel model)
        {
            var categoryIds = new List<int>();

            if (model.SearchCategoryId > 0)
                categoryIds.Add(model.SearchCategoryId);
            //include subcategories
            if (model.SearchIncludeSubCategories && model.SearchCategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.SearchCategoryId));

            //0 - all (according to "ShowHidden" parameter)
            //1 - published only
            //2 - unpublished only
            bool? publish = null;
            if (model.SearchPublishedId == 1)
                publish = true;
            else if (model.SearchPublishedId == 2)
                publish = false;

            var homepageCategories = _homePageCategoryService.GetAllHomePageCategories(model.TextPrompt, categoryIds, publish,
                command.Page - 1, command.PageSize);

            var gridModel = new DataSourceResult();
            gridModel.Data = homepageCategories.Select(x =>
            {
                var category = _categoryService.GetCategoryById(x.CategoryId);
                var homepageModel = new HomePageCategoryModel()
                {
                    Id = x.Id,
                    CategoryId = category != null ? x.CategoryId : 0,
                    DisplayOrder = x.DisplayOrder,
                    Published = x.Published,
                    TextPrompt = x.TextPrompt,
                    CategoryName = category != null ? category.GetFormattedBreadCrumb(_categoryService) : "",
                    PercentValue = x.PercentValue ?? 0
                };
                return homepageModel;
            });
            gridModel.Total = homepageCategories.TotalCount;

            return Json(gridModel);
        }

        public ActionResult HomePageCategoryCreate()
        {
            var model = new HomePageCategoryModel();

            model.AvailableCategories.Add(new SelectListItem { Text = "Select category", Value = "0" });
            var categories = GetCategoryList();
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/HomePageCategoryCreate.cshtml", model);
        }


        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult HomePageCategoryCreate(HomePageCategoryModel model, bool continueEditing) 
        {
            if (ModelState.IsValid)
            {
                var homePageCategory = new BS_HomePageCategory()
                {
                    CategoryId = model.CategoryId,
                    DisplayOrder = model.DisplayOrder,
                    Published = model.Published,
                    TextPrompt = model.TextPrompt,
                    PictureId = model.PictureId,
                    PercentValue = model.PercentValue
                };

                _homePageCategoryService.InsertHomePageCategory(homePageCategory);
                UpdateHomepageItems();
                SuccessNotification("Home page category added successfully.");
                if (continueEditing)
                {
                    return RedirectToAction("HomePageCategoryEdit", new { id = homePageCategory.Id });
                }

                return RedirectToAction("HomePageCategory");
            }

            model.AvailableCategories.Add(new SelectListItem { Text = "Select category", Value = "0" });
            var categories = GetCategoryList();
            foreach (var c in categories)
                model.AvailableCategories.Add(c);

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/HomePageCategoryCreate.cshtml", model);
        }

        public ActionResult HomePageCategoryEdit(int id)
        {
            var homePageCategory = _homePageCategoryService.GetHomePageCategoryById(id);

            if (homePageCategory == null)
                return RedirectToAction("HomePageCategory");

            var category = _categoryService.GetCategoryById(homePageCategory.CategoryId);

            //if (category == null)
            //    throw new NullReferenceException("category");

            var model = new HomePageCategoryModel()
            {
                Id = homePageCategory.Id,
                DisplayOrder = homePageCategory.DisplayOrder,
                Published = homePageCategory.Published,
                TextPrompt = homePageCategory.TextPrompt,
                CategoryName = category != null ? category.Name : "",
                PictureId = homePageCategory.PictureId,
                CategoryId = homePageCategory.CategoryId,
                PercentValue = homePageCategory.PercentValue ?? 0
            };

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/HomePageCategoryEdit.cshtml", model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult HomePageCategoryEdit(HomePageCategoryModel model, bool continueEditing)
        {
            var homePageCategory = _homePageCategoryService.GetHomePageCategoryById(model.Id);

            if (homePageCategory == null)
                throw new NopException("BS_HomePageCategory");

            if (ModelState.IsValid)
            {
                homePageCategory.DisplayOrder = model.DisplayOrder;
                homePageCategory.Published = model.Published;
                homePageCategory.TextPrompt = model.TextPrompt;
                homePageCategory.PictureId = model.PictureId;
                homePageCategory.PercentValue = model.PercentValue;

                _homePageCategoryService.UpdateHomePageCategory(homePageCategory);
                UpdateHomepageItems();

                SuccessNotification("Home page category updated successfully.");
                if (continueEditing)
                {
                    return RedirectToAction("HomePageCategoryEdit", new { id = homePageCategory.Id });
                }

                return RedirectToAction("HomePageCategory");
            }

            return View("~/Plugins/NopStation.MobileWebApi/Views/WebApi/HomePageCategoryEdit.cshtml", model);
        }

        [HttpPost]
        public ActionResult HomePageCategoryDelete(int id)
        {
            if (_workContext.CurrentVendor != null)
                return RedirectToAction("HomePageCategory");

            var homePageCategory = _homePageCategoryService.GetHomePageCategoryById(id);

            if (homePageCategory == null)
                return RedirectToAction("HomePageCategory");

            _homePageCategoryService.DeleteHomePageCategory(homePageCategory);
            UpdateHomepageItems();

            SuccessNotification("Home page category deleted successfully.");
            return RedirectToAction("HomePageCategory");
        }
        
        public ActionResult LoadCategoryProducts(int categoryId)
        {
            return Json(LoadAvailableCategoryProducts(categoryId), JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult HomePageCategoryProductList(DataSourceRequest command, int homePageCategoryId)
        {
            var homePageCategory = _homePageCategoryService.GetHomePageCategoryById(homePageCategoryId);

            if (homePageCategory == null)
                return new NullJsonResult();

            var products = homePageCategory.HomePageCategoryProducts.OrderBy(x => x.DisplayOrder).Skip((command.Page - 1) * command.PageSize).Take(command.PageSize);

            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var product = _productService.GetProductById(x.ProductId);
                var homepageModel = new HomePageCategoryModel.HomePagecategoryProductModel()
                {
                    Id = x.Id,
                    DisplayOrder = x.DisplayOrder,
                    HomePageCategoryId = homePageCategory.Id,
                    ProductId = product == null ? 0 : x.ProductId,
                    ProductName = product == null ? "" : product.Name
                };
                return homepageModel;
            });
            gridModel.Total = homePageCategory.HomePageCategoryProducts.Count();

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult HomePageCategoryProductUpdate(HomePageCategoryModel.HomePagecategoryProductModel model)
        {
            var homePageCategoryProduct = _homePageCategoryService.GetHomePageCategoryProductById(model.Id);

            if (homePageCategoryProduct == null)
                return new NullJsonResult();

            homePageCategoryProduct.DisplayOrder = model.DisplayOrder;
            _homePageCategoryService.UpdateHomePageCategoryProduct(homePageCategoryProduct);
            UpdateHomepageItems();

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult HomePageCategoryProductDelete(HomePageCategoryModel.HomePagecategoryProductModel model)
        {
            var homePageCategoryProduct = _homePageCategoryService.GetHomePageCategoryProductById(model.Id);
            if (homePageCategoryProduct == null)
                return new NullJsonResult();

            _homePageCategoryService.DeleteHomePageCategoryProduct(homePageCategoryProduct);
            UpdateHomepageItems();

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult HomePageCategoryProductCreate(HomePageCategoryModel.HomePagecategoryProductModel model)
        {
            var homePageCategory = _homePageCategoryService.GetHomePageCategoryById(model.HomePageCategoryId);

            if (homePageCategory == null)
                throw new NopException("homePageCategory");

            var homePageCategoryProduct = new BS_HomePageCategoryProduct()
            {
                DisplayOrder = model.DisplayOrder,
                HomePageCategoryId = model.HomePageCategoryId,
                ProductId = model.ProductId
            };

            _homePageCategoryService.InsertHomePageCategoryProduct(homePageCategoryProduct);
            UpdateHomepageItems();

            return new NullJsonResult();
        }

        #endregion

        #endregion
    }
}

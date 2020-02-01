using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models;
using BS.Plugin.NopStation.MobileWebApi.Models._Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Banner;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.PluginSettings;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Tax;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tasks;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using Nop.Web.Models.Media;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class HomePageTask : ITask
    {
        #region Fields

        private const string IconFilePath = "Plugins/NopStation.MobileWebApi/Content/IconPackage";

        private readonly ICategoryService _categoryService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;
        private readonly IPictureService _pictureService;
        private readonly MediaSettings _mediaSettings;
        private readonly IManufacturerService _manufacturerService;
        private readonly IPermissionService _permissionService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ICurrencyService _currencyService;
        private readonly CatalogSettings _catalogSettings;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly IProductServiceApi _productServiceApi;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ISearchTermService _searchTermService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IProductTagService _productTagService;
        private readonly IVendorService _vendorService;
        private readonly VendorSettings _vendorSettings;
        private readonly ICategoryIconService _categoryIconService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly ILanguageService _languageService;
        private readonly ICustomerServiceApi _customerServiceApi;
        private readonly IOrderReportService _orderReportService;
        private readonly TaxSettings _taxSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ApiSettings _apiSettings;
        private readonly IBS_HomePageCategoryService _homePageCategoryService;
        private readonly IBS_SliderService _sliderService;

        #endregion

        #region Ctor

        public HomePageTask(ICategoryService categoryService,
            IWorkContext workContext,
            IStoreContext storeContext,
            IWebHelper webHelper,
            ICacheManager cacheManager,
            IPictureService pictureService,
            MediaSettings mediaSettings,
            IManufacturerService manufacturerService,
            IPermissionService permissionService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            ICurrencyService currencyService,
            CatalogSettings catalogSettings,
            IPriceFormatter priceFormatter,
            IProductService productService,
            ISpecificationAttributeService specificationAttributeService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            IProductServiceApi productServiceApi,
            IGenericAttributeService genericAttributeService,
            ISearchTermService searchTermService,
            IEventPublisher eventPublisher,
            IProductTagService productTagService,
            IVendorService vendorService,
            VendorSettings vendorSettings,
            ICategoryIconService categoryIconService,
            LocalizationSettings localizationSettings,
            ILanguageService languageService,
            ICustomerServiceApi customerServiceApi,
            IOrderReportService orderReportService,
            TaxSettings taxSettings,
            ShoppingCartSettings shoppingCartSettings,
            ApiSettings apiSettings,
            IBS_HomePageCategoryService homePageCategoryService,
            IBS_SliderService sliderService)
        {
            this._categoryService = categoryService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
            this._pictureService = pictureService;
            this._mediaSettings = mediaSettings;
            this._manufacturerService = manufacturerService;
            this._permissionService = permissionService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._currencyService = currencyService;
            this._catalogSettings = catalogSettings;
            this._priceFormatter = priceFormatter;
            this._productService = productService;
            this._specificationAttributeService = specificationAttributeService;
            this._customerActivityService = customerActivityService;
            this._localizationService = localizationService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._productServiceApi = productServiceApi;
            this._genericAttributeService = genericAttributeService;
            this._searchTermService = searchTermService;
            this._eventPublisher = eventPublisher;
            this._productTagService = productTagService;
            this._vendorService = vendorService;
            this._vendorSettings = vendorSettings;
            this._categoryIconService = categoryIconService;
            this._localizationSettings = localizationSettings;
            this._languageService = languageService;
            this._customerServiceApi = customerServiceApi;
            this._orderReportService = orderReportService;
            this._taxSettings = taxSettings;
            this._shoppingCartSettings = shoppingCartSettings;
            this._apiSettings = apiSettings;
            this._homePageCategoryService = homePageCategoryService;
            this._sliderService = sliderService;
        }

        #endregion

        #region Utilities

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
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, curJson);

            filePath = CommonHelper.MapPath("~/ApiJson/currency-backup-json.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, curJson);
        }

        protected IList<MenufactureOverViewModelApi> PrepareManufacturersModel()
        {

            var allModel = new List<MenufactureOverViewModelApi>();
            var allManufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in allManufacturers)
            {
                var modelMan = manufacturer.MapTo<Manufacturer, MenufactureOverViewModelApi>();

                //prepare picture model
                int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                modelMan.DefaultPictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId, pictureSize)
                };
                allModel.Add(modelMan);
            }

            var manJson = JsonConvert.SerializeObject(allModel);
            WriteManufacturerJson(manJson);


            var model = new List<MenufactureOverViewModelApi>();
            var manufacturers = _manufacturerService.GetAllManufacturersDisplayedOnHomePage();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = manufacturer.MapTo<Manufacturer, MenufactureOverViewModelApi>();

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

        public List<CategoryNavigationModelApi> FlatToHierarchy(IEnumerable<CategoryNavigationModelApi> list, int parentId = 0)
        {
            return (from i in list
                    where i.ParentCategoryId == parentId
                    select new CategoryNavigationModelApi
                    {
                        Id = i.Id,
                        ParentCategoryId = i.ParentCategoryId,
                        Name = i.Name,
                        Extension = i.Extension,
                        ProductCount = i.ProductCount,
                        DisplayOrder = i.DisplayOrder,
                        IconPath = i.IconPath,
                        Children = FlatToHierarchy(list, i.Id),
                        ImageUrl = i.ImageUrl
                    }).ToList();
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

//        protected IList<HomePageCategoryWithProductsModel> PrepareCategoriesWithProductsModel()
//        {
//            var homePageCategories = _homePageCategoryService.GetAllHomePageCategories(publish: true);
//            var model = new List<HomePageCategoryWithProductsModel>();
//            foreach (var hpc in homePageCategories.Where(x => x.HomePageCategoryProducts.Count > 0))
//            {
//                var picture = _pictureService.GetPictureById(hpc.PictureId);
//
//                var hpm = new HomePageCategoryWithProductsModel()
//                {
//                    CategoryId = hpc.CategoryId,
//                    TextPrompt = hpc.TextPrompt,
//                    Id = hpc.Id,
//                    ApplicableFor = hpc.ApplicableFor,
//                    IconUrl = _pictureService.GetPictureUrl(hpc.PictureId)
//                };
//
//                var products = _productService.GetProductsByIds(hpc.HomePageCategoryProducts.OrderBy(x => x.DisplayOrder).Select(x => x.ProductId).ToArray());
//
//                if (products == null || products.Count == 0)
//                    continue;
//
//                hpm.Products = PrepareProductOverviewModels(products).ToList();
//                model.Add(hpm);
//            }
//
//            return model;
//        }


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
                    IconUrl = _pictureService.GetPictureUrl(hpc.PictureId),
                    ActiveEndDate = hpc.ActiveEndDate,
                    ActiveStartDate = hpc.ActiveStartDate,
                    ForBlApp = hpc.ForBlApp
                };

                if (hpc.ActiveStartDate != null)
                {
                    hpm.ActiveEndDateString = hpc.ActiveStartDate.Value.ToString("yyyy/MM/dd hh:mm:ss");
                }

                if (hpc.ActiveEndDate != null)
                {
                    hpm.ActiveEndDateString = hpc.ActiveEndDate.Value.ToString("yyyy/MM/dd hh:mm:ss");
                }

                var products = _productService.GetProductsByIds(hpc.HomePageCategoryProducts.OrderBy(x => x.DisplayOrder).Select(x => x.ProductId).ToArray());

                if (products == null || products.Count == 0)
                    continue;

                hpm.Products = PrepareProductOverviewModels(products).ToList();
                model.Add(hpm);
            }

            return model;
        }


        protected IList<CategoryNavigationModelApi> PrepareCategories()
        {
            var allCmodel = new List<CategoryNavigationModelApi>();
            var allCats = _categoryService.GetAllCategories();
            allCats.Add(_categoryService.GetAllCategories(categoryName: "App Only", showHidden: true).FirstOrDefault());

            foreach (var item in allCats.OrderBy(x => x.DisplayOrder))
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
                allCmodel.Add(categoryNavigationModelApi);
            }

            var catJson = JsonConvert.SerializeObject(allCmodel);
            var catJsonV1 = JsonConvert.SerializeObject(FlatToHierarchy(allCmodel));
            WriteCategoryJson(catJson, catJsonV1);


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

        //protected IList<HomePageBannerResponseModel.BannerModel> PrepareBannerModel()
        //{
        //    var sliders = _sliderService.GetActiveBSSliderImages();

        //    var pictureList = (from sliderDomain in sliders
        //                       select new HomePageBannerResponseModel.BannerModel
        //                       {
        //                           ImageUrl = _pictureService.GetPictureUrl(sliderDomain.PictureId),
        //                           DomainType = sliderDomain.SliderDomainTypeId,
        //                           DomainId = sliderDomain.DomainId,
        //                           //IsProduct = sliderDomain.IsProduct,
        //                           //ProdOrCatId = sliderDomain.ProdOrCatId,
        //                           Link = "",
        //                           Text = ""
        //                       }).ToList();

        //    return pictureList;
        //}


        protected IList<HomePageBannerResponseModel.BannerModel> PrepareBannerModel()
        {
            var sliders = _sliderService.GetActiveBSSliderImages();

            var pictureList = (from sliderDomain in sliders
                select new HomePageBannerResponseModel.BannerModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sliderDomain.PictureId),
                    DomainType = sliderDomain.SliderDomainTypeId,
                    DomainId = sliderDomain.DomainId,
                    IsProduct = sliderDomain.IsProduct,
                    ProdOrCatId = sliderDomain.ProdOrCatId,
                    Link = "",
                    Text = ""
                }).ToList();

            return pictureList;
        }

        protected IList<HomePageBannerResponseModel.BannerModel> PrepareEventBannerModel()
        {
            var sliders = _sliderService.GetActiveBSSliderImages().Where(x => x.IsEventBanner);

            var pictureList = (from sliderDomain in sliders
                select new HomePageBannerResponseModel.BannerModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(sliderDomain.PictureId),
                    DomainType = sliderDomain.SliderDomainTypeId,
                    DomainId = sliderDomain.DomainId,
                    IsProduct = sliderDomain.IsProduct,
                    ProdOrCatId = sliderDomain.ProdOrCatId,
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

        protected void WriteHomePageJson(HomePageResponseModel homePageModel)
        {
            var json = JsonConvert.SerializeObject(homePageModel);

            var nfilePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-json.json");
            if (!File.Exists(nfilePath))
                File.Create(nfilePath).Dispose();
            File.WriteAllText(nfilePath, json);

            nfilePath = CommonHelper.MapPath("~/ApiJson/homepage-response-model-backup-json.json");
            if (!File.Exists(nfilePath))
                File.Create(nfilePath).Dispose();
            File.WriteAllText(nfilePath, json);

        }

        protected void WriteCategoryJson(string json, string jsonV1)
        {
            var filePath = CommonHelper.MapPath("~/ApiJson/menu-category-json.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, json);

            filePath = CommonHelper.MapPath("~/ApiJson/menu-category-backup-json.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, json);

            filePath = CommonHelper.MapPath("~/ApiJson/menu-category-json-v1.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, jsonV1);

            filePath = CommonHelper.MapPath("~/ApiJson/menu-category-backup-json-v1.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, jsonV1);
        }

        protected void WriteManufacturerJson(string json)
        {
            var filePath = CommonHelper.MapPath("~/ApiJson/manufacturer-json.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, json);

            filePath = CommonHelper.MapPath("~/ApiJson/manufacturer-backup-json.json");
            if (!File.Exists(filePath))
                File.Create(filePath).Dispose();
            File.WriteAllText(filePath, json);
        }

        #endregion

        #region Methods

        #region old Execute

        //        public void Execute()
        //        {
        //            var homePageModel = new HomePageResponseModel();
        //            homePageModel.Language = PrepareLanguageModel();
        //            homePageModel.Banners = PrepareBannerModel();
        //            homePageModel.BannerIsEnabled = homePageModel.Banners.Any();
        //            homePageModel.Categories = PrepareCategories();
        //            homePageModel.CategoriesWithProducts = PrepareCategoriesWithProductsModel();
        //            homePageModel.CategoryListIcon = _pictureService.GetPictureUrl(_apiSettings.CategoryListIconId);
        //            homePageModel.Icons = PrepareIconModel();
        //            homePageModel.ManufacturerListIcon = _pictureService.GetPictureUrl(_apiSettings.ManufacturerListIconId);
        //            homePageModel.Manufacturers = PrepareManufacturersModel();
        //
        //            WriteHomePageJson(homePageModel);
        //            WriteCurrencyJson();
        //        }

        #endregion



        public void Execute()
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

        #endregion
    }
}

using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Plugin.Widgets.BsMegaMenu.Infrastructure.Cache;
using Nop.Plugin.Widgets.BsMegaMenu.Models;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Core.Domain.Catalog;
using System.Collections.Generic;
//using Nop.Admin.Extensions;
using Nop.Plugin.Widgets.BsMegaMenu.Domain;
using Nop.Services.Localization;
using Nop.Core.Data;
using Nop.Web.Extensions;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Widgets.BsMegaMenu.Services;
using Nop.Web.Models.Catalog;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Directory;
using Nop.Core.Domain.Media;

namespace Nop.Plugin.Widgets.BsMegaMenu.Controllers
{
    public class BsMegaMenuController : BasePluginController
    {
        #region Fields
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly ISettingService _settingService;
        private readonly ICacheManager _cacheManager;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IRepository<BsMegaMenuDomain> _bsMegaMenuRepo;
        private readonly IBsMegaMenuService _bsMegaMenuService;
        private readonly IProductService _productService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IWebHelper _webHelper;
        private readonly CatalogSettings _catalogSettings;
        private readonly MediaSettings _mediaSettings;
        private readonly IManufacturerService _manufacturerService;
        private readonly IMeasureService _measureService;
        #endregion

        #region Constructor
        public BsMegaMenuController(IRepository<BsMegaMenuDomain> bsMegaMenuRepo,
            IBsMegaMenuService bsMegaMenuService, IWorkContext workContext,
            IStoreContext storeContext, IStoreService storeService, MediaSettings mediaSettings,
            IPriceCalculationService priceCalculationService, CatalogSettings catalogSettings,
            IPriceFormatter priceFormatter, ICurrencyService currencyService,
            IPictureService pictureService, IWebHelper webHelper, ISettingService settingService,
            ICacheManager cacheManager, ICategoryService categoryService,
            ISpecificationAttributeService specificationAttributeService,
            IPermissionService permissionService, ITaxService taxService, IManufacturerService manufacturerService,
            IProductService productService, ILocalizationService localizationService, IMeasureService measureService)
        {
            this._bsMegaMenuRepo = bsMegaMenuRepo;
            this._bsMegaMenuService = bsMegaMenuService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._mediaSettings = mediaSettings;
            this._priceCalculationService = priceCalculationService;
            this._catalogSettings = catalogSettings;
            this._priceFormatter = priceFormatter;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._webHelper = webHelper;
            this._settingService = settingService;
            this._cacheManager = cacheManager;
            this._categoryService = categoryService;
            this._specificationAttributeService = specificationAttributeService;
            this._permissionService = permissionService;
            this._taxService = taxService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._localizationService = localizationService;
            this._measureService = measureService;
        }
        #endregion

        #region Utilities
        [NonAction]
        protected virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products, int IsLazyLoad, int NoOfItems,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (IsLazyLoad == 1)
            {
                ViewBag.IsLazyLoad = 1;
            }
            else
            {
                ViewBag.IsLazyLoad = 0;
            }
            if (NoOfItems != 3)
            {
                ViewBag.items = 1;
            }
            else
            {
                ViewBag.items = 0;
            }

            return this.PrepareProductOverviewModels(_workContext,
              _storeContext, _categoryService, _productService, _specificationAttributeService,
              _priceCalculationService, _priceFormatter, _permissionService,
              _localizationService, _taxService, _currencyService,
              _pictureService, _measureService, _webHelper, _cacheManager,
              _catalogSettings, _mediaSettings, products,
              preparePriceModel, preparePictureModel,
              productThumbPictureSize, prepareSpecificationAttributes,
              forceRedirectionAfterAddingToCart);
            //var models = _productModelFactory.PrepareProductOverviewModels(products, preparePriceModel, preparePictureModel, productThumbPictureSize, prepareSpecificationAttributes, forceRedirectionAfterAddingToCart).ToList();
            //return models;
        }
        #endregion

        #region Methods

        #region Admin

        [AdminAuthorize]
        public ActionResult ManageBsMegaMenu()
        {
            return View("~/Plugins/Widgets.BsMegaMenu/Views/ManageBsMegaMenu.cshtml");
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult ListBsMegaMenus()
        {
            var menus = _bsMegaMenuService.Menus();

            var gridModel = new DataSourceResult
            {
                Data = menus.Select(x => new
                {
                    Id = x.Id,
                    SelectedCategory = _categoryService.GetCategoryById(x.CategoryId).Name,
                    NumberOfColums = x.NumberOfColums,
                    ColumnPosition = x.ColumnPosition
                }),
                Total = menus.Count()
            };
            return Json(gridModel);
        }

        [AdminAuthorize]
        public ActionResult CreateBsMegaMenu(int Id = 0)
        {
            BsMegaMenuDomain megaMenu = new BsMegaMenuDomain();

            List<Category> categories = new List<Category>();
            var firstLevelCategories = _categoryService.GetAllCategoriesByParentCategoryId(0, false, false).ToList();
            categories.AddRange(firstLevelCategories);

            foreach (var firstLevelCategory in firstLevelCategories)
            {
                var secondLavelCategories = _categoryService.GetAllCategoriesByParentCategoryId(firstLevelCategory.Id, false, false).ToList();
                categories.AddRange(secondLavelCategories);
            }
            foreach (var c in categories)
                megaMenu.CategoryList.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

            return View("~/Plugins/Widgets.BsMegaMenu/Views/CreateBsMegaMenu.cshtml", megaMenu);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult CreateBsMegaMenu(BsMegaMenuDomain model)
        {
            if (ModelState.IsValid)
            {
                BsMegaMenuDomain entity = new BsMegaMenuDomain();

                entity.CategoryId = model.CategoryId;

                var category = _categoryService.GetCategoryById(model.CategoryId);
                var parentCategory = _bsMegaMenuService.ParentCategory(model);
                var parentCategoryNumberOfColums = _bsMegaMenuService.ParentCategoryNumberOfColums(parentCategory);

                if (category.ParentCategoryId == 0)
                {
                    entity.NumberOfColums = model.NumberOfColums;
                    entity.ColumnPosition = 0;

                }
                else
                {
                    entity.NumberOfColums = 0;
                    entity.ColumnPosition = model.ColumnPosition;
                }

                _bsMegaMenuRepo.Insert(model);
                SuccessNotification("Mega Menu for your Category Created Successfullly!");

                return RedirectToRoute(new
                {
                    Action = "CreateBsMegaMenu",
                    Controller = "BsMegaMenu",
                    Id = model.Id
                });
            }
            else
            {
                List<Category> categories = new List<Category>();
                var firstLevelCategories = _categoryService.GetAllCategoriesByParentCategoryId(0, false, false).ToList();
                categories.AddRange(firstLevelCategories);

                foreach (var firstLevelCategory in firstLevelCategories)
                {
                    var secondLavelCategories = _categoryService.GetAllCategoriesByParentCategoryId(firstLevelCategory.Id, false, false).ToList();
                    categories.AddRange(secondLavelCategories);
                }
                foreach (var c in categories)
                    model.CategoryList.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });

                return View("~/Plugins/Widgets.BsMegaMenu/Views/CreateBsMegaMenu.cshtml", model);
            }

        }

        [AdminAuthorize]
        public ActionResult UpdateBsMegaMenu(int Id)
        {
            BsMegaMenuDomain entity = new BsMegaMenuDomain();
            entity = _bsMegaMenuRepo.GetById(Id);
            var category = _categoryService.GetCategoryById(entity.CategoryId);
            entity.SelectedCategory = category.Name;
            entity.ParentCategoryId = category.ParentCategoryId;

            return View("~/Plugins/Widgets.BsMegaMenu/Views/UpdateBsMegaMenu.cshtml", entity);
        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult UpdateBsMegaMenu(BsMegaMenuDomain model)
        {
            if (ModelState.IsValid)
            {
                BsMegaMenuDomain entity = new BsMegaMenuDomain();
                entity = _bsMegaMenuRepo.GetById(model.Id);

                var category = _categoryService.GetCategoryById(model.CategoryId);
                var parentCategory = _bsMegaMenuService.ParentCategory(model);
                var parentCategoryNumberOfColums = _bsMegaMenuService.ParentCategoryNumberOfColums(parentCategory);

                if (category.ParentCategoryId == 0)
                {
                    entity.NumberOfColums = model.NumberOfColums;
                    entity.ColumnPosition = 0;

                }
                else
                {
                    entity.NumberOfColums = 0;
                    entity.ColumnPosition = model.ColumnPosition;
                }

                //if (category.ParentCategoryId == 0 && model.NumberOfColums > 4)
                //{
                //    ErrorNotification("Number of colums of your category must be less then 4");
                //}
                //else if ((category.ParentCategoryId > 0) && (model.ColumnPosition > (_bsMegaMenuRepo.Table.
                //    Where(x => x.CategoryId == parentCategory.Id).Select(y => y.NumberOfColums).FirstOrDefault())))
                //{
                //    ErrorNotification("Colum position of your category must be less than number of colums of it's parent category");
                //}

                //else
                //{
                _bsMegaMenuRepo.Update(model);
                _cacheManager.Clear();
                SuccessNotification("Mega Menu for your category updated successfullly!");
                //}

                return RedirectToRoute(new
                {
                    Action = "UpdateBsMegaMenu",
                    Controller = "BsMegaMenu",
                    Id = model.Id
                });
            }

            else
            {
                var category = _categoryService.GetCategoryById(model.CategoryId);
                model.SelectedCategory = category.Name;
                model.ParentCategoryId = category.ParentCategoryId;

                return View("~/Plugins/Widgets.BsMegaMenu/Views/UpdateBsMegaMenu.cshtml", model);
            }

        }

        [AdminAuthorize]
        [HttpPost]
        public ActionResult DeleteBsMegaMenu(int Id)
        {
            BsMegaMenuDomain entity = _bsMegaMenuRepo.GetById(Id);
            _bsMegaMenuRepo.Delete(entity);

            return new NullJsonResult();
        }

        #endregion

        #region Store

        #region cache meghamenu
        public ActionResult BsMegaMenuCache()
        {

            var model = new BsMegaMenuModel();

            var bsMegaMenuSetting = _settingService.LoadSetting<BsMegaMenuSettings>();
            bool link = bsMegaMenuSetting.ShowImage;

            var key = string.Format("BsMegamenu-{0}", _storeContext.CurrentStore.Id);
            var categories = _cacheManager.Get(key, () =>
                _categoryService.GetAllCategoriesByParentCategoryId(0, true)
                .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                .Select(x => new CategoryMenuModel
                {
                    Id = x.Id,
                    MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(x.Id),
                    Name = x.Name,
                    SeName = x.GetSeName(),
                    PictureLink = (link == true) ? _pictureService.GetPictureUrl(x.PictureId) : "",
                    SubCategories = _categoryService.GetAllCategoriesByParentCategoryId(x.Id, true)
                    .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                    .Select(y => new CategoryMenuModel
                    {
                        Id = y.Id,
                        MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(y.Id),
                        Name = y.Name,
                        SeName = y.GetSeName(),
                        PictureLink = (link == true) ? _pictureService.GetPictureUrl(y.PictureId) : "",
                        SubSubCategories = _categoryService.GetAllCategoriesByParentCategoryId(y.Id, true)
                        .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                        .Select(z => new CategoryMenuModel
                        {
                            Id = z.Id,
                            MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(z.Id),
                            Name = z.Name,
                            SeName = z.GetSeName(),
                            PictureLink = (link == true) ? _pictureService.GetPictureUrl(z.PictureId) : "",
                            SubSubSubCategories = _categoryService.GetAllCategoriesByParentCategoryId(z.Id, true)
                            .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                            .Select(w => new CategoryMenuModel
                            {
                                Id = w.Id,
                                MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(w.Id),
                                Name = w.Name,
                                SeName = w.GetSeName(),
                                PictureLink = (link == true) ? _pictureService.GetDefaultPictureUrl(y.PictureId) : ""
                            }).ToList()
                        }).ToList()

                    }).ToList()
                }).Take(7).ToList());
            foreach (var category in categories)
            {
                List<int> categoryIds = new List<int>();
                categoryIds.Add(category.Id);
                var categoryFeatureProducts = _productService.SearchProducts(categoryIds: categoryIds, featuredProducts: true);
                var categoryFeatureProductsModel = PrepareProductOverviewModels(products: categoryFeatureProducts, IsLazyLoad: 1, NoOfItems: 3, productThumbPictureSize: 120);
                foreach (var x in categoryFeatureProductsModel)
                {
                    category.CategoryFeatureProductModel.Add(x);
                }
            }

            model.BsMegaMenuSettingsModel.SetWidgetZone = bsMegaMenuSetting.SetWidgetZone;
            model.BsMegaMenuSettingsModel.ShowImage = bsMegaMenuSetting.ShowImage;
            model.BsMegaMenuSettingsModel.MenuType = bsMegaMenuSetting.MenuType;
            model.BsMegaMenuSettingsModel.noOfMenufactureItems = bsMegaMenuSetting.noOfMenufactureItems;

            model.CategoryList = categories;

            if (model.BsMegaMenuSettingsModel.noOfMenufactureItems < 1)
            {
                model.BsMegaMenuSettingsModel.noOfMenufactureItems = 24;
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(pageSize: model.BsMegaMenuSettingsModel.noOfMenufactureItems);

            CatalogPagingFilteringModel command = new CatalogPagingFilteringModel();
            foreach (var manufacturer in manufacturers)
            {
                var manModel = manufacturer.ToModel();
               // var manModel = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
                var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                if (picture != null)
                {
                    manModel.PictureModel.Title = picture.TitleAttribute;
                    manModel.PictureModel.AlternateText = picture.AltAttribute;
                    manModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId);
                }
                else
                {
                    manModel.PictureModel.Title = manufacturer.Name;
                    manModel.PictureModel.AlternateText = manufacturer.Name;
                    manModel.PictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl();
                }
                model.Manufactures.Add(manModel);

            }

            string megamenuHtml = this.RenderPartialViewToString("~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenu/PublicInfo.cshtml", model);

            var filePath = CommonHelper.MapPath("~/Plugins/Widgets.BsMegaMenu/bs-megamenuHtml.txt");
            System.IO.File.WriteAllText(filePath, megamenuHtml);
            filePath = CommonHelper.MapPath("~/Plugins/Widgets.BsMegaMenu/bs-megamenuHtml-backup.txt");
            System.IO.File.WriteAllText(filePath, megamenuHtml);

            return Content("");

        }

        #endregion
        protected string GetPictureUrl(int pictureId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.PICTURE_URL_MODEL_KEY, pictureId);
            return _cacheManager.Get(cacheKey, () =>
            {
                var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false) ?? "";
                //little hack here. nulls aren't cacheable so set it to ""

                return url;
            });
        }

        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            #region without cache
            var model = new BsMegaMenuModel();

            var bsMegaMenuSetting = _settingService.LoadSetting<BsMegaMenuSettings>();
            bool link = bsMegaMenuSetting.ShowImage;

            var key = string.Format("BsMegamenu-{0}", _storeContext.CurrentStore.Id);
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(0, true)
                .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                .Select(x => new CategoryMenuModel
                {
                    Id = x.Id,
                    MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(x.Id),
                    Name = x.Name,
                    SeName = x.GetSeName(),
                    PictureLink = (link == true) ? _pictureService.GetPictureUrl(x.PictureId) : "",
                    SubCategories = _categoryService.GetAllCategoriesByParentCategoryId(x.Id, true)
                    .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                    .Select(y => new CategoryMenuModel
                    {
                        Id = y.Id,
                        MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(y.Id),
                        Name = y.Name,
                        SeName = y.GetSeName(),
                        PictureLink = (link == true) ? _pictureService.GetPictureUrl(y.PictureId) : "",
                        SubSubCategories = _categoryService.GetAllCategoriesByParentCategoryId(y.Id, true)
                        .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                        .Select(z => new CategoryMenuModel
                        {
                            Id = z.Id,
                            MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(z.Id),
                            Name = z.Name,
                            SeName = z.GetSeName(),
                            PictureLink = (link == true) ? _pictureService.GetPictureUrl(z.PictureId) : "",
                            SubSubSubCategories = _categoryService.GetAllCategoriesByParentCategoryId(z.Id, true)
                            .Where(c => c.Published.Equals(true)).OrderBy(c => c.DisplayOrder)
                            .Select(w => new CategoryMenuModel
                            {
                                Id = w.Id,
                                MenuColumnNumber = _bsMegaMenuService.MenuColumnNumber(w.Id),
                                Name = w.Name,
                                SeName = w.GetSeName(),
                                PictureLink = (link == true) ? _pictureService.GetDefaultPictureUrl(y.PictureId) : ""
                            }).ToList()
                        }).ToList()

                    }).ToList()
                }).Take(7).ToList();

            #region feature product
            //foreach (var category in categories)
            //{
            //    List<int> categoryIds = new List<int>();
            //    categoryIds.Add(category.Id);
            //    var categoryFeatureProducts = _productService.SearchProducts(categoryIds: categoryIds, featuredProducts: true);
            //    var categoryFeatureProductsModel = PrepareProductOverviewModels(products: categoryFeatureProducts, IsLazyLoad: 1, NoOfItems: 3, productThumbPictureSize: 120);
            //    category.CategoryFeatureProductModel.AddRange(categoryFeatureProductsModel);
            //}
            #endregion          

            model.BsMegaMenuSettingsModel.SetWidgetZone = bsMegaMenuSetting.SetWidgetZone;
            model.BsMegaMenuSettingsModel.ShowImage = bsMegaMenuSetting.ShowImage;
            model.BsMegaMenuSettingsModel.MenuType = bsMegaMenuSetting.MenuType;
            model.BsMegaMenuSettingsModel.noOfMenufactureItems = bsMegaMenuSetting.noOfMenufactureItems;

            model.CategoryList = categories;

            if (model.BsMegaMenuSettingsModel.noOfMenufactureItems < 1)
            {
                model.BsMegaMenuSettingsModel.noOfMenufactureItems = 24;
            }

            var manufacturers = _manufacturerService.GetAllManufacturers(pageSize: model.BsMegaMenuSettingsModel.noOfMenufactureItems);

            CatalogPagingFilteringModel command = new CatalogPagingFilteringModel();
            foreach (var manufacturer in manufacturers)
            {
                var manModel = manufacturer.ToModel();
                //var manModel = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
                var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                if (picture != null)
                {
                    manModel.PictureModel.Title = picture.TitleAttribute;
                    manModel.PictureModel.AlternateText = picture.AltAttribute;
                    manModel.PictureModel.ImageUrl = _pictureService.GetPictureUrl(manufacturer.PictureId);
                }
                else
                {
                    manModel.PictureModel.Title = manufacturer.Name;
                    manModel.PictureModel.AlternateText = manufacturer.Name;
                    manModel.PictureModel.ImageUrl = _pictureService.GetDefaultPictureUrl();
                }
                model.Manufactures.Add(manModel);
            }

            return View(model.BsMegaMenuSettingsModel.MenuType == "Mega Menu" ? "~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenu/PublicInfo.cshtml" : "~/Plugins/Widgets.BsMegaMenu/Views/BsMegaMenu/_PublicInfo.cshtml", model);
            #endregion
        }
        #endregion

        #endregion
    }
}
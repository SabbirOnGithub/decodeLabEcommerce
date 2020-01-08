using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Kendoui;
using Nop.Plugin.Misc.HomePageProduct.Models;
using System.Collections.Generic;
using Nop.Services.Media;
using Nop.Plugin.Misc.HomePageProduct.Services;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Core.Caching;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Directory;
using System.IO;
using Nop.Services.Logging;
using Newtonsoft.Json;

namespace Nop.Plugin.Misc.HomePageProduct.Controllers
{
    public class HomePageProductController : Controller
    {
        #region Field

        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IWorkContext _workContext;
        private readonly CatalogSettings _catalogSettings;
        private readonly IProductService _productService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        private readonly IHomePageProductCategoryImageService _homePageProductCategoryImageService;
        private readonly IHomePageProductService _homePageProductService;
        private readonly IWebHelper _webHelper;
        private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;
        private readonly IPermissionService _permissionService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IHomePageCategoryService _homePageCategoryService;
        private readonly IHomePageSubCategoryService _homePageSubCategoryService;
        private readonly ILogger _logger;

        #endregion

        #region Ctr

        public HomePageProductController(IDateTimeHelper dateTimeHelper,
            IWorkContext workContext,
            CatalogSettings catalogSettings,
            IProductService productService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICategoryService categoryService,
            IHomePageProductCategoryImageService homePageProductCategoryImageService,
            IHomePageProductService homePageProductService,
            IWebHelper webHelper,
            IStoreContext storeContext,
            ICacheManager cacheManager,
            IPermissionService permissionService,
            IPriceCalculationService priceCalculationService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPriceFormatter priceFormatter,
            IHomePageCategoryService homePageCategoryService,
            IHomePageSubCategoryService homePageSubCategoryService,
            ILogger logger)
        {
            this._dateTimeHelper = dateTimeHelper;
            this._workContext = workContext;
            this._catalogSettings = catalogSettings;
            this._productService = productService;
            this._localizationService = localizationService;
            this._categoryService = categoryService;
            this._productService = productService;
            this._pictureService = pictureService;
            this._homePageProductCategoryImageService = homePageProductCategoryImageService;
            this._homePageProductService = homePageProductService;
            this._webHelper = webHelper;
            this._storeContext = storeContext;
            this._cacheManager = cacheManager;
            this._permissionService = permissionService;
            this._priceCalculationService = priceCalculationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._priceFormatter = priceFormatter;
            this._homePageCategoryService = homePageCategoryService;
            this._homePageSubCategoryService = homePageSubCategoryService;
            this._logger = logger;
        }

        #endregion

        #region Methods admin

        #region Category

        public ActionResult CategoryList()
        {
            var model = new HomeCategoryModel();
            model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            model.CategoryPriority = 0;
            var categories = _categoryService.GetAllCategories(showHidden: true);
            foreach (var c in categories)
            {
                model.AvailableCategories.Add(new SelectListItem { Text = c.GetFormattedBreadCrumb(categories), Value = c.Id.ToString() });
            }
            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/CategoryList.cshtml", model);
        }

        [HttpPost]
        public ActionResult CategoryList(DataSourceRequest command, CategoryListModel model)
        {
            var homeapgecategories = _homePageCategoryService.GetHomePageCategory(pageIndex: command.Page - 1, pageSize: command.PageSize);
            var gridModel = new DataSourceResult();

            var listModel = new List<CategoryListModel>();
            foreach (var x in homeapgecategories)
            {
                var category = _categoryService.GetCategoryById(x.CategoryId);
                if (category != null)
                {
                    var m = new CategoryListModel()
                    {
                        Id = x.Id,
                        CategoryId = x.CategoryId,
                        CategoryDisplayName = x.CategoryDisplayName,
                        CategoryName = category.Name,
                        Publish = x.Publish,
                        CategoryPriority = x.CategoryPriority
                    };
                    listModel.Add(m);
                }
            }
            gridModel.Data = listModel;
            gridModel.Total = homeapgecategories.TotalCount;
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CategoryAdd(int categoryId, int categoryPriority, string categorydispalyName)
        {
            if (categoryId > 0)
            {
                if (!_homePageCategoryService.IsCategoryExist(categoryId))
                {
                    _homePageCategoryService.Insert(new HomePageCategory
                    {
                        Publish = true,
                        CategoryId = categoryId,
                        CategoryPriority = categoryPriority,
                        CategoryDisplayName = categorydispalyName,
                    });

                    WriteInText(out _);

                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { Result = "Category already added" }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Result = "Please select a category first" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CategoryUpdate(int categoryId, int categoryPriority, string categorydispalyName)
        {
            if (categoryId > 0)
            {
                _homePageCategoryService.Delete(categoryId);
                _homePageCategoryService.Insert(new HomePageCategory
                {
                    Publish = true,
                    CategoryId = categoryId,
                    CategoryPriority = categoryPriority,
                    CategoryDisplayName = categorydispalyName,
                });

                WriteInText(out _);

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Result = "Please select a category first" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int categoryId)
        {
            _homePageCategoryService.Delete(categoryId);

            WriteInText(out _);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Category image

        public ActionResult CategoryImage(int CategoryId)
        {
            var homePageCategory = _homePageCategoryService.GetHomePageCategoryByCategoryId(CategoryId);
            var category = _categoryService.GetCategoryById(CategoryId);

            if (homePageCategory == null || category == null)
                return RedirectToRoute("Plugin.Misc.HomePageProduct.CategoryList");

            var model = new HomePageCategoryImageModel();
            model.CategoryName = !string.IsNullOrWhiteSpace(homePageCategory.CategoryDisplayName)? homePageCategory.CategoryDisplayName : category.Name;
            model.CategoryId = CategoryId;
            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/CategoryImage.cshtml", model);
        }

        [HttpPost]
        public ActionResult CategoryImageList(DataSourceRequest command, int categoryId)
        {
            var categoryPictures = _homePageProductCategoryImageService.GetCategoryPicturesByCategoryId(categoryId);
            var categoryPicturesModel = new List<HomePageCategoryImageModel>();

            foreach (var x in categoryPictures)
            {
                var picture = _pictureService.GetPictureById(x.ImageId);
                if (picture != null)
                {
                    var m = new HomePageCategoryImageModel
                    {
                        Id = x.Id,
                        PictureId = x.ImageId,
                        PictureUrl = _pictureService.GetPictureUrl(picture),
                        OverrideAltAttribute = picture.AltAttribute,
                        OverrideTitleAttribute = picture.TitleAttribute,
                        Url = x.Url,
                        DisplayOrder = x.DisplayOrder,
                        Caption = x.Caption,
                        IsMainPicture = x.IsMainPicture,
                    };
                    categoryPicturesModel.Add(m);
                }
            }

            var gridModel = new DataSourceResult
            {
                Data = categoryPicturesModel,
                Total = categoryPicturesModel.Count
            };

            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult CategoryImageAdd(int pictureId, string overrideAltAttribute, string overrideTitleAttribute,
            int categoryId, string url, int displayOrder, string categoryColor, string caption, bool isMainPicture)
        {
            if (pictureId == 0)
                throw new ArgumentException();

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            try
            {
                if(!categoryColor.StartsWith("#"))
                    categoryColor = "#" + categoryColor;

                _homePageProductCategoryImageService.Insert(new HomePageProductCategoryImage
                {
                    ImageId = pictureId,
                    CategoryId = categoryId,
                    Url = url,
                    DisplayOrder = displayOrder,
                    Caption = caption,
                    CategoryColor = categoryColor == "" ? "#ae2125" : categoryColor,
                    IsMainPicture = isMainPicture
                });

                var pictureBinary = _pictureService.LoadPictureBinary(picture);
                _pictureService.UpdatePicture(picture.Id, pictureBinary, picture.MimeType,
                    picture.SeoFilename, overrideAltAttribute, overrideTitleAttribute);

                WriteInText(out _);

                return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return Json(new { Result = false, Message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult CategoryImageUpdate(int id, string url, int displayOrder, bool IsMainPicture, string Caption)
        {
            var categoryImage = _homePageProductCategoryImageService.HomePageProductCategoryImage(id);
            if (categoryImage == null)
                throw new ArgumentException("No category picture found with the specified id");

            categoryImage.Url = url;
            categoryImage.DisplayOrder = displayOrder;
            categoryImage.Caption = Caption;
            categoryImage.IsMainPicture = IsMainPicture;
            _homePageProductCategoryImageService.UpdateCategoryPictureDetails(categoryImage);

            WriteInText(out _);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult CategoryImageDelete(int id)
        {
            var categoryImage = _homePageProductCategoryImageService.HomePageProductCategoryImage(id);
            if (categoryImage == null)
                throw new ArgumentException("No category picture found with the specified id");

            var pictureId = categoryImage.ImageId;

            var picture = _pictureService.GetPictureById(pictureId);
            if (picture == null)
                throw new ArgumentException("No picture found with the specified id");

            _pictureService.DeletePicture(picture);
            _homePageProductCategoryImageService.DeleteCategoryPicture(categoryImage);

            WriteInText(out _);

            return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult UpdateCategoryColor(int categoryId, string categoryColor)
        {
            if (!categoryColor.StartsWith("#"))
                categoryColor = "#" + categoryColor;

            var homePageProductCategoryImages = _homePageProductCategoryImageService.GetHomePageProductCategoryImagesByCategoryID(categoryId);

            for (int i = 0; i < homePageProductCategoryImages.Count; i++)
            {
                var categoryImage = _homePageProductCategoryImageService.HomePageProductCategoryImage(homePageProductCategoryImages[i].Id);
                categoryImage.CategoryColor = categoryColor;
                categoryImage.CategoryId = categoryId;
                _homePageProductCategoryImageService.UpdateCategoryColor(categoryImage);
            }

            WriteInText(out _);

            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Gurbase

        public ActionResult List(int CategoryId)
        {
            var model = new HomePageProductListModel();
            model.CategoryId = CategoryId;
            model.CategoryName = _categoryService.GetCategoryById(CategoryId).Name;
            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/List.cshtml", model);
        }

        public ActionResult SubCategoryList(int CategoryId)
        {
            var model = new HomePageProductSubCategoryListModel();
            model.CategoryId = CategoryId;
            model.CategoryName = _categoryService.GetCategoryById(CategoryId).Name;
            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/SubCategoryList.cshtml", model);
        }

        public ActionResult Configure()
        {
            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/Configure.cshtml");
        }

        [HttpPost]
        public ActionResult HomePageProductUpdate(IEnumerable<HomePageProductModel> products, int CategoryId)
        {
            if (CategoryId > 0)
            {
                var homePageProductCategory = new HomePageProductCategory();
                if (products != null)
                {
                    foreach (var pModel in products)
                    {
                        //update
                        if (pModel.ShowOnHomePage)
                        {
                            _homePageProductService.Delete(pModel.Id);

                            homePageProductCategory.ProductId = pModel.Id;
                            homePageProductCategory.CategoryId = CategoryId;
                            homePageProductCategory.Priority = pModel.Priority;
                            _homePageProductService.Insert(homePageProductCategory);
                        }
                        else
                        {
                            _homePageProductService.Delete(pModel.Id);
                        }
                    }
                }
                PublicPage();
                return new NullJsonResult();
            }
            else
            {
                PublicPage();
                return this.Json(new DataSourceResult
                {
                    Errors = "Please select a category"
                });
            }
        }

        [HttpPost]
        public ActionResult HomePageSubCategoryUpdate(IEnumerable<HomePageSubCategoryModel> SubCategories, int CategoryId)
        {
            if (CategoryId > 0)
            {
                HomePageSubCategory homePageSubCategory = new HomePageSubCategory();

                if (SubCategories != null)
                {
                    foreach (var SubCategory in SubCategories)
                    {
                        //update
                        if (SubCategory.SubCategoryShowOnHomePage)
                        {
                            _homePageSubCategoryService.Delete(SubCategory.Id);

                            homePageSubCategory.SubCategoryId = SubCategory.Id;
                            homePageSubCategory.CategoryId = CategoryId;
                            homePageSubCategory.SubCategoryPriority = SubCategory.Priority;
                            homePageSubCategory.TabName = SubCategory.TabName;
                            _homePageSubCategoryService.Insert(homePageSubCategory);
                        }
                        else
                        {
                            _homePageSubCategoryService.Delete(SubCategory.Id);
                        }
                    }
                }
                PublicPage();
                return new NullJsonResult();
            }
            else
            {
                PublicPage();
                return this.Json(new DataSourceResult
                {
                    Errors = "Please select a category"
                });
            }
            //return new NullJsonResult();
        }

        [HttpPost]
        public ActionResult ListJson(DataSourceRequest command, HomePageProductListModel model)
        {
            //Session["categoryId"] = model.SearchCategoryId;
            var categoryIds = new List<int>() { model.CategoryId };
            model.CategoryId = model.CategoryId;
            model.SearchIncludeSubCategories = true;

            //include subcategories
            if (model.SearchIncludeSubCategories && model.CategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.CategoryId));

            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize,
                showHidden: true
            );

            var gridModel = new DataSourceResult();
            gridModel.Data = products.Select(x =>
            {
                var defaultProductPicture = _pictureService.GetPicturesByProductId(x.Id).FirstOrDefault();
                return new HomePageProductModel()
                {
                    Id = x.Id,
                    Add = 0,
                    DisableBuyButton = x.DisableBuyButton,
                    //ShowOnHomePage=x.ShowOnHomePage,
                    ShowOnHomePage = _homePageProductService.GetHomePageProductByProductId(x.Id).ProductId > 0 ? true : false,
                    Delete = 0,
                    ManageInventoryMethod = x.ManageInventoryMethod.GetLocalizedEnum(_localizationService, _workContext.WorkingLanguage.Id),
                    Name = x.Name,
                    OldPrice = x.OldPrice,
                    Price = x.Price,
                    Published = x.Published,
                    Sku = x.Sku,
                    StockQuantity = x.StockQuantity,
                    Priority = _homePageProductService.GetHomePageProductByProductId(x.Id).Priority,
                    PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true)
                };
            });
            gridModel.Total = products.TotalCount;
            PublicPage();
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ListJsonSubCategory(DataSourceRequest command, HomePageProductSubCategoryListModel model)
        {
            //Session["categoryId"] = model.SearchCategoryId;
            var categoryIds = new List<int>() { model.CategoryId };
            model.CategoryId = model.CategoryId;
            model.SearchIncludeSubCategories = true;

            //include subcategories
            if (model.SearchIncludeSubCategories && model.CategoryId > 0)
                categoryIds.AddRange(GetChildCategoryIds(model.CategoryId));

            var subcategories = _categoryService.GetAllCategoriesByParentCategoryId(model.CategoryId, true, true);
            var gridModel = new DataSourceResult();
            gridModel.Data = subcategories.Select(x =>
            {
                return new HomePageSubCategoryModel()
                {
                    Id = x.Id,
                    Name = x.Name,
                    SubCategoryShowOnHomePage = _homePageSubCategoryService.GetHomePageSubCategoryBySubCategoryIdList(x.Id).Count() > 0 ? true : false,
                    Priority = _homePageSubCategoryService.GetHomePageSubCategoryBySubCategoryId(x.Id).SubCategoryPriority,
                    TabName = _homePageSubCategoryService.GetHomePageSubCategoryBySubCategoryId(x.Id).TabName
                };
            });
            gridModel.Total = subcategories.Count();
            PublicPage();
            return Json(gridModel);
        }

        [HttpPost]
        public ActionResult ChangePriority(int categoryId)
        {
            var singleCategory = _homePageCategoryService.GetHomePageCategoryByCategoryId(categoryId);
            PublicPage();
            return Json(new { catId = categoryId, priority = singleCategory.CategoryPriority, categoryDispalyName = singleCategory.CategoryDisplayName });
        }

        [HttpPost]
        public ActionResult DeleteWholeCategory(int categoryId)
        {
            var categoryHomePagePictureList = _homePageProductCategoryImageService.GetCategoryPicturesByCategoryId(categoryId);
            foreach (var categoryHomePagePicture in categoryHomePagePictureList)
            {
                _homePageProductCategoryImageService.DeleteCategoryPicture(categoryHomePagePicture);
            }
            return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult HomepageProductsBySpecialCategory()
        {
            var filePath = CommonHelper.MapPath("~/Plugins/Misc.HomePageProduct/special-category.txt");
            var jsonSpecialCategory = System.IO.File.ReadAllText(filePath);

            return Content(jsonSpecialCategory);
        }

        public virtual string RenderPartialViewToString(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = this.ControllerContext.RouteData.GetRequiredString("action");

            this.ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                ViewEngineResult viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(this.ControllerContext, viewName);
                var viewContext = new ViewContext(this.ControllerContext, viewResult.View, this.ViewData, this.TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        public ActionResult Settings()
        {
            return View("~/Plugins/Misc.BiponeePreOrder/Views/MiscBiponeePreOrder/Settings.cshtml");
        }

        [HttpPost]
        public ActionResult Settingsssssss()
        {
            return View("~/Plugins/Misc.BiponeePreOrder/Views/MiscBiponeePreOrder/Settings.cshtml");
        }

        #endregion
        
        #endregion

        #region public
        public ActionResult PublicPage()
        {
            List<CategoryHomePageModel> homePageCategories = new List<CategoryHomePageModel>();
            string categoryModelText = null;
            var filePath = CommonHelper.MapPath("~/Plugins/Misc.HomePageProduct/special-category.txt");

            try
            {
                categoryModelText = System.IO.File.ReadAllText(filePath);
            }
            catch { }

            if (string.IsNullOrWhiteSpace(categoryModelText))
            {
                try
                {
                    filePath = CommonHelper.MapPath("~/Plugins/Misc.HomePageProduct/special-category-backup.txt");
                    categoryModelText = System.IO.File.ReadAllText(filePath);
                }
                catch { }
            }

            if (!string.IsNullOrWhiteSpace(categoryModelText))
            {
                homePageCategories = JsonConvert.DeserializeObject<List<CategoryHomePageModel>>(categoryModelText);
            }
            else
            {
                WriteInText(out homePageCategories);
            }

            return View("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/PublicPage.cshtml", homePageCategories);
        }

        private void WriteInText(out List<CategoryHomePageModel> homePageCategories)
        {
            //string cacheKey = "CategoryHomePagePublicPage";
            homePageCategories = new List<CategoryHomePageModel>();
            var model = _homePageCategoryService.GetHomePageCategory();
            foreach (var item in model)
            {
                //var categoryIds = _categoryService.GetAllCategoriesByParentCategoryId(item.Id).Select(c => c.Id).ToList();
                var homePageCategory = new CategoryHomePageModel();
                var category = _categoryService.GetCategoryById(item.CategoryId);
                if (category != null)
                {
                    homePageCategory.MainCategoryName = !string.IsNullOrEmpty(item.CategoryDisplayName) ? item.CategoryDisplayName : category.Name;
                    homePageCategory.MainCategorySeName = category.GetSeName();
                }

                var subCategoryIds = _homePageSubCategoryService.GetHomePageSubCategoryByCategoryIdList(item.CategoryId);

                foreach (var subCategoryId in subCategoryIds)
                {
                    var subCategory = _homePageSubCategoryService.GetHomePageSubCategoryByCategory(subCategoryId);//.TabName
                    var categoryModel = new CategorySimpleModel
                    {
                        Id = subCategoryId,
                        Name = !string.IsNullOrEmpty(subCategory.TabName) ? subCategory.TabName : _categoryService.GetCategoryById(subCategoryId).Name,
                        SeName = _categoryService.GetCategoryById(subCategoryId).GetSeName()
                    };
                    var subcategoryProducts = _homePageProductService.GetHomePageProductCategoryProductIdByCategoryId(subCategoryId);
                    List<Product> productList = new List<Product>();
                    foreach (var productId in subcategoryProducts)
                    {
                        var product = _productService.GetProductById(productId);
                        //homePageCategory.Products.Add(product);
                        productList.Add(product);
                    }
                    categoryModel.Products.AddRange(PrepareProductOverviewModels(productList));
                    homePageCategory.Categories.Add(categoryModel);
                }

                //homePageCategory.Categories = this.PrepareCategorySimpleModels(item.CategoryId);

                //var categoryImageIdList = _homePageProductCategoryImageService.GetPageProductCategoryImageIdByCategoryId(item.CategoryId);
                var categoryImageList = _homePageProductCategoryImageService.GetCategoryPicturesByCategoryId(item.CategoryId);

                //foreach (var categoryImageId in categoryImageIdList)
                foreach (var categoryImage in categoryImageList)
                {
                    //var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, item.Id, 20, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    var picture = _pictureService.GetPictureById(categoryImage.ImageId);

                    //var pictureModel = new PictureModel
                    var pictureModel = new CustomPictureModel
                    {
                        //FullSizeImageUrl = _pictureService.GetPictureUrl(picture, 300),
                        //ImageUrl = _pictureService.GetPictureUrl(picture, 300),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture),
                        //Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), item.Name),
                        //AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), item.Name)
                        Title = "",
                        AlternateText = "",
                        Url = categoryImage.Url != null ? categoryImage.Url : "#",
                        Caption = categoryImage.Caption,
                        IsMainPicture = categoryImage.IsMainPicture,
                        DisplayOrder = categoryImage.DisplayOrder
                    };

                    homePageCategory.PictureModel.Add(pictureModel);
                }

                var productIdList = _homePageProductService.GetHomePageProductCategoryProductIdByCategoryId(item.CategoryId);
                List<Product> products = new List<Product>();
                foreach (var productId in productIdList)
                {
                    var product = _productService.GetProductById(productId);
                    //homePageCategory.Products.Add(product);
                    products.Add(product);

                }
                homePageCategory.Products.AddRange(PrepareProductOverviewModels(products));
                homePageCategory.CategoryColor = _homePageProductCategoryImageService.GetPageProductCategoryColor(item.CategoryId);
                homePageCategories.Add(homePageCategory);
            }

            //string specialCategoryHtml = this.RenderPartialViewToString("~/Plugins/Misc.HomePageProduct/Views/MiscHomePageProduct/PublicPage.cshtml", homePageCategories);

            var specialCategoryText = JsonConvert.SerializeObject(homePageCategories);

            var filePath = CommonHelper.MapPath("~/Plugins/Misc.HomePageProduct/special-category.txt");
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Dispose();
            }
            System.IO.File.WriteAllText(filePath, specialCategoryText);

            filePath = CommonHelper.MapPath("~/Plugins/Misc.HomePageProduct/special-category-backup.txt");
            if (!System.IO.File.Exists(filePath))
            {
                System.IO.File.Create(filePath).Dispose();
            }
            System.IO.File.WriteAllText(filePath, specialCategoryText);
        }
        #endregion
        #region Utility

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


        protected virtual List<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
        bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();

            //little hack for performance optimization.
            //we know that this method is used to load top and left menu for categories.
            //it'll load all categories anyway.
            //so there's no need to invoke "GetAllCategoriesByParentCategoryId" multiple times (extra SQL commands) to load childs
            //so we load all categories at once
            //if you don't like this implementation if you can uncomment the line below (old behavior) and comment several next lines (before foreach)
            //var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            //if (allCategories == null)
            //{
            //    //load categories if null passed
            //    //we implemeneted it this way for performance optimization - recursive iterations (below)
            //    //this way all categories are loaded only once
            //    allCategories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            //}
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(rootCategoryId);
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                ////product number for each category
                //if (_catalogSettings.ShowCategoryProductNumber)
                //{
                //    string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_NUMBER_OF_PRODUCTS_MODEL_KEY,
                //        string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                //        _storeContext.CurrentStore.Id,
                //        category.Id);
                //    categoryModel.NumberOfProducts = _cacheManager.Get(cacheKey, () =>
                //    {
                //        var categoryIds = new List<int>();
                //        categoryIds.Add(category.Id);
                //        //include subcategories
                //        if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                //            categoryIds.AddRange(GetChildCategoryIds(category.Id));
                //        return _productService.GetCategoryProductNumber(categoryIds, _storeContext.CurrentStore.Id);
                //    });
                //}

                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        protected IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            if (products == null)
                throw new ArgumentNullException("products");

            var models = new List<ProductOverviewModel>();
            foreach (var product in products)
            {
                var model = new ProductOverviewModel
                {
                    Id = product.Id,
                    Name = product.GetLocalized(x => x.Name),
                    ShortDescription = product.GetLocalized(x => x.ShortDescription),
                    FullDescription = product.GetLocalized(x => x.FullDescription),
                    SeName = product.GetSeName(),
                };
                //price
                if (preparePriceModel)
                {
                    #region Prepare product price

                    var priceModel = new ProductOverviewModel.ProductPriceModel();

                    switch (product.ProductType)
                    {
                        case ProductType.GroupedProduct:
                            {
                                #region Grouped product

                                var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);

                                switch (associatedProducts.Count)
                                {
                                    case 0:
                                        {
                                            //no associated products
                                            priceModel.OldPrice = null;
                                            priceModel.Price = null;
                                            priceModel.DisableBuyButton = true;
                                            priceModel.DisableWishlistButton = true;
                                            priceModel.AvailableForPreOrder = false;
                                        }
                                        break;
                                    default:
                                        {
                                            //we have at least one associated product
                                            priceModel.DisableBuyButton = true;
                                            priceModel.DisableWishlistButton = true;
                                            priceModel.AvailableForPreOrder = false;

                                            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                            {
                                                //find a minimum possible price
                                                decimal? minPossiblePrice = null;
                                                Product minPriceProduct = null;
                                                foreach (var associatedProduct in associatedProducts)
                                                {
                                                    //calculate for the maximum quantity (in case if we have tier prices)
                                                    var tmpPrice = _priceCalculationService.GetFinalPrice(associatedProduct,
                                                        _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);
                                                    if (!minPossiblePrice.HasValue || tmpPrice < minPossiblePrice.Value)
                                                    {
                                                        minPriceProduct = associatedProduct;
                                                        minPossiblePrice = tmpPrice;
                                                    }
                                                }
                                                if (minPriceProduct != null && !minPriceProduct.CustomerEntersPrice)
                                                {
                                                    if (minPriceProduct.CallForPrice)
                                                    {
                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                                    }
                                                    else if (minPossiblePrice.HasValue)
                                                    {
                                                        //calculate prices
                                                        decimal taxRate;
                                                        decimal finalPriceBase = _taxService.GetProductPrice(minPriceProduct, minPossiblePrice.Value, out taxRate);
                                                        decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                                        priceModel.OldPrice = null;
                                                        priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));

                                                    }
                                                    else
                                                    {
                                                        //Actually it's not possible (we presume that minimalPrice always has a value)
                                                        //We never should get here
                                                        //Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //hide prices
                                                priceModel.OldPrice = null;
                                                priceModel.Price = null;
                                            }
                                        }
                                        break;
                                }

                                #endregion
                            }
                            break;
                        case ProductType.SimpleProduct:
                        default:
                            {
                                #region Simple product

                                //add to cart button
                                priceModel.DisableBuyButton = product.DisableBuyButton ||
                                    !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart) ||
                                    !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                //add to wishlist button
                                priceModel.DisableWishlistButton = product.DisableWishlistButton ||
                                    !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist) ||
                                    !_permissionService.Authorize(StandardPermissionProvider.DisplayPrices);

                                //rental
                                priceModel.IsRental = product.IsRental;

                                //pre-order
                                if (product.AvailableForPreOrder)
                                {
                                    priceModel.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                                        product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                                    priceModel.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
                                }

                                //prices
                                if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                                {
                                    if (!product.CustomerEntersPrice)
                                    {
                                        if (product.CallForPrice)
                                        {
                                            //call for price
                                            priceModel.OldPrice = null;
                                            priceModel.Price = _localizationService.GetResource("Products.CallForPrice");
                                        }
                                        else
                                        {
                                            //prices

                                            //calculate for the maximum quantity (in case if we have tier prices)
                                            decimal minPossiblePrice = _priceCalculationService.GetFinalPrice(product,
                                                _workContext.CurrentCustomer, decimal.Zero, true, int.MaxValue);

                                            decimal taxRate;
                                            decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                                            decimal finalPriceBase = _taxService.GetProductPrice(product, minPossiblePrice, out taxRate);

                                            decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                                            decimal finalPrice = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceBase, _workContext.WorkingCurrency);

                                            //do we have tier prices configured?
                                            var tierPrices = new List<TierPrice>();
                                            if (product.HasTierPrices)
                                            {
                                                tierPrices.AddRange(product.TierPrices
                                                    .OrderBy(tp => tp.Quantity)
                                                    .ToList()
                                                    .FilterByStore(_storeContext.CurrentStore.Id)
                                                    .FilterForCustomer(_workContext.CurrentCustomer)
                                                    .RemoveDuplicatedQuantities());
                                            }
                                            //When there is just one tier (with  qty 1), 
                                            //there are no actual savings in the list.
                                            bool displayFromMessage = tierPrices.Count > 0 &&
                                                !(tierPrices.Count == 1 && tierPrices[0].Quantity <= 1);
                                            if (displayFromMessage)
                                            {
                                                priceModel.OldPrice = null;
                                                priceModel.Price = String.Format(_localizationService.GetResource("Products.PriceRangeFrom"), _priceFormatter.FormatPrice(finalPrice));
                                            }
                                            else
                                            {
                                                if (finalPriceBase != oldPriceBase && oldPriceBase != decimal.Zero)
                                                {
                                                    priceModel.OldPrice = _priceFormatter.FormatPrice(oldPrice);
                                                    priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                }
                                                else
                                                {
                                                    priceModel.OldPrice = null;
                                                    priceModel.Price = _priceFormatter.FormatPrice(finalPrice);
                                                }
                                            }
                                            if (product.IsRental)
                                            {
                                                //rental product
                                                priceModel.OldPrice = _priceFormatter.FormatRentalProductPeriod(product, priceModel.OldPrice);
                                                priceModel.Price = _priceFormatter.FormatRentalProductPeriod(product, priceModel.Price);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    //hide prices
                                    priceModel.OldPrice = null;
                                    priceModel.Price = null;
                                }

                                #endregion
                            }
                            break;
                    }

                    model.ProductPrice = priceModel;

                    #endregion
                }

                //picture
                if (preparePictureModel)
                {
                    #region Prepare product picture

                    //If a size has been set in the view, we use it in priority
                    int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : 200;
                    //prepare picture model
                    var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                    model.DefaultPictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                    };
                    //"title" attribute
                    model.DefaultPictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                    //"alt" attribute
                    model.DefaultPictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);

                    #endregion
                }

                models.Add(model);
            }
            return models;
        }
        #endregion
    }
}
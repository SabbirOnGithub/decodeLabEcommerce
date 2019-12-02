using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Infrastructure.Cache;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using System.Linq;
using System.Web.Http;
using Nop.Services.Media;
using Nop.Web.Models.Media;
using System.Collections.Generic;
using Nop.Services.Catalog;
using Nop.Services.Customers;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class ThemeDataController : WebApiController
    {
        #region Field
        private readonly IThemeDataService _themeDataService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ICacheManager _cacheManager;
        private readonly IWebHelper _webHelper;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IPictureService _pictureService;
        #endregion

        #region Ctor
        public ThemeDataController(IThemeDataService themeDataService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            ICacheManager cacheManager,
            IWebHelper webHelper,
            IWorkContext workContext,
            IStoreContext storeContext,
            IProductService productService,
            ICategoryService categoryService,
            IPictureService pictureService)
        {
            this._themeDataService = themeDataService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._cacheManager = cacheManager;
            this._webHelper = webHelper;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._productService = productService;
            this._categoryService = categoryService;
            this._pictureService = pictureService;
        }
        #endregion

        #region Utility   
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            string cacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_CHILD_IDENTIFIERS_MODEL_KEY,
                parentCategoryId,
                string.Join(",", _workContext.CurrentCustomer.GetCustomerRoleIds()),
                _storeContext.CurrentStore.Id);
            return _cacheManager.Get(cacheKey, () =>
            {
                var categoriesIds = new List<int>();
                var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
                foreach (var category in categories)
                {
                    categoriesIds.Add(category.Id);
                    categoriesIds.AddRange(GetChildCategoryIds(category.Id));
                }
                return categoriesIds;
            });
        }

        protected IList<Product> GetAllProductsByCategoryId(int categoryId)
        {
            var categoryIds = new List<int> { categoryId };
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                //include subcategories
                categoryIds.AddRange(GetChildCategoryIds(categoryId));
            }
            //products
            var products = _productService.SearchProducts(
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true);

            return products;
        }
        #endregion

        #region Action Method

        [Route("api/popularcategories")]
        [HttpGet]
        public IHttpActionResult PopularCategories(int? thumbPictureSize = null)
        {
            var model = _themeDataService.PopularCategories()
                    .Select(x =>
                    {
                        var catModel = x.MapTo<Category, CategoryOverViewModelApi>();
                        int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                        //prepare picture model
                        if (thumbPictureSize.HasValue)
                        {
                            pictureSize = thumbPictureSize.GetValueOrDefault();
                        }

                        var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                        catModel.DefaultPictureModel = _cacheManager.Get(categoryPictureCacheKey, () =>
                        {
                            var picture = _pictureService.GetPictureById(x.PictureId);
                            var pictureModel = new PictureModel
                            {
                                ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                            };
                            return pictureModel;
                        });
                        catModel.ProductCount = GetAllProductsByCategoryId(catModel.Id).Count;
                        return catModel;
                    })
                    .ToList();

            var result = new GeneralResponseModel<List<CategoryOverViewModelApi>>()
            {
                Data = model
            };
            return Ok(result);
        }
        #endregion
    }
}

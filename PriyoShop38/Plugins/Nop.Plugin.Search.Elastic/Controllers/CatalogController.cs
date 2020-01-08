using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Directory;
using Nop.Services.Events;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Services.Vendors;
using System.Web.Mvc;
using Nop.Plugin.Search.Elastic.Services;
using Nop.Services.Topics;

namespace Nop.Plugin.Search.Elastic.Controllers
{
    public class CatalogController : Nop.Web.Controllers.CatalogController
    {
        #region Fields

       
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IProductTagService _productTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ILogger _logger;

        //elastic search
        private readonly IElasticSearchService _elasticSearchService;

        #endregion
        public CatalogController(
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            ICategoryTemplateService categoryTemplateService,
            IManufacturerTemplateService manufacturerTemplateService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            ICustomerActivityService customerActivityService,
            ITopicService topicService,
            IEventPublisher eventPublisher,
            ISearchTermService searchTermService,
            IMeasureService measureService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            BlogSettings blogSettings,
            ForumSettings forumSettings,
            ICacheManager cacheManager,

            ILogger logger,
            IElasticSearchService elasticSearchService) : base(
                categoryService,
              manufacturerService,
              productService,
              vendorService,
              categoryTemplateService,
              manufacturerTemplateService,
              workContext,
              storeContext,
              taxService,
              currencyService,
              pictureService,
              localizationService,
              priceCalculationService,
              priceFormatter,
              webHelper,
              specificationAttributeService,
              productTagService,
              genericAttributeService,
              aclService,
              storeMappingService,
              permissionService,
              customerActivityService,
              topicService,
              eventPublisher,
              searchTermService,
              measureService,
              mediaSettings,
              catalogSettings,
              vendorSettings,
              blogSettings,
               forumSettings,
              cacheManager

                )
        {
            
            this._categoryService = categoryService;
            this._manufacturerService = manufacturerService;
            this._productService = productService;
            this._vendorService = vendorService;
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._localizationService = localizationService;
            this._webHelper = webHelper;
            this._productTagService = productTagService;
            this._genericAttributeService = genericAttributeService;
            this._aclService = aclService;
            this._storeMappingService = storeMappingService;
            this._permissionService = permissionService;
            this._customerActivityService = customerActivityService;
            this._mediaSettings = mediaSettings;
            this._catalogSettings = catalogSettings;
            this._vendorSettings = vendorSettings;
            this._logger = logger;
            this._elasticSearchService = elasticSearchService;
        }

        #region methods
        public ActionResult SearchTermAutoCompleteFromElastic(string term)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");
            string originalTerm = term;
            term = term.Replace("\"", " inch ").Replace("'", " inch ");
            term = term.ToLower().Replace("toys", "toy");


            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var respnse = _elasticSearchService.Search(term,from:0,size:productNumber);

            //var products = _productService.SearchProducts(
            //    storeId: _storeContext.CurrentStore.Id,
            //    keywords: term,
            //    searchSku: true,
            //    languageId: _workContext.WorkingLanguage.Id,
            //    visibleIndividuallyOnly: true,
            //    pageSize: productNumber);


            var products = _elasticSearchService.GetProductsByIds(respnse.ResultsIds.ToArray());



            var models =PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                          where !p.Name.StartsWith("SPECIAL:")
                          select new
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl("Product", new { SeName = p.SeName }),
                              productpictureurl = p.DefaultPictureModel.ImageUrl
                          })
                          .ToList();
            result.Add(new { label = "See All", producturl = "/search?q=" + term, productpictureurl = "" });
            //shahdat - record search history
           // RecordSearchHistory(originalTerm, true, false,(int)respnse.Total, string.Join(",", products.Select(x => x.Id)));
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}

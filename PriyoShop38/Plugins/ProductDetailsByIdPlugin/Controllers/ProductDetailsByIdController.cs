using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Helpers;
using Nop.Services.Localization;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System.Collections.Generic;
using Nop.Services.Media;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using Nop.Web.Infrastructure.Cache;
using Nop.Core.Caching;
using System.Diagnostics;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Services.Directory;
using System.IO;
using Nop.Services.Orders;
using Nop.Web.Models.Common;
using Nop.Services.Configuration;
using Nop.Plugin.Misc.ProductDetailsById;
using Nop.Web.Controllers;
using Nop.Services.Stores;
using Nop.Core.Domain.Orders;
using Nop.Services.Logging;
using Nop.Services.Vendors;
using Nop.Services.Messages;
using Nop.Services.Shipping;
using Nop.Services.Events;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Web.Framework.Security.Captcha;
using Nop.Core.Domain.Seo;

namespace Nop.Plugin.Misc.ProductDetailsById.Controllers
{

    public partial class ProductDetailsByIdController : Nop.Web.Controllers.ProductController
    {
        #region Field

        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IProductTemplateService _productTemplateService;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly ILocalizationService _localizationService;
        private readonly IMeasureService _measureService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IWebHelper _webHelper;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly IProductTagService _productTagService;
        private readonly IOrderReportService _orderReportService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly IDownloadService _downloadService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IShippingService _shippingService;
        private readonly IEventPublisher _eventPublisher;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Constructors

        public ProductDetailsByIdController(ICategoryService categoryService,
            IManufacturerService manufacturerService,
            IProductService productService,
            IVendorService vendorService,
            IProductTemplateService productTemplateService,
            IProductAttributeService productAttributeService,
            IWorkContext workContext,
            IStoreContext storeContext,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            ILocalizationService localizationService,
            IMeasureService measureService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IWebHelper webHelper,
            ISpecificationAttributeService specificationAttributeService,
            IDateTimeHelper dateTimeHelper,
            IRecentlyViewedProductsService recentlyViewedProductsService,
            ICompareProductsService compareProductsService,
            IWorkflowMessageService workflowMessageService,
            IProductTagService productTagService,
            IOrderReportService orderReportService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService,
            IDownloadService downloadService,
            ICustomerActivityService customerActivityService,
            IProductAttributeParser productAttributeParser,
            IShippingService shippingService,
            IEventPublisher eventPublisher,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            ShoppingCartSettings shoppingCartSettings,
            LocalizationSettings localizationSettings,
            CustomerSettings customerSettings,
            CaptchaSettings captchaSettings,
            SeoSettings seoSettings,
            ICacheManager cacheManager) : base(
            categoryService,
        manufacturerService,
           productService,
           vendorService,
            productTemplateService,
            productAttributeService,
             workContext,
            storeContext,
            taxService,
            currencyService,
            pictureService,
            localizationService,
            measureService,
            priceCalculationService,
            priceFormatter,
             webHelper,
            specificationAttributeService,
            dateTimeHelper,
            recentlyViewedProductsService,
            compareProductsService,
            workflowMessageService,
            productTagService,
            orderReportService,
            aclService,
            storeMappingService,
            permissionService,
            downloadService,
            customerActivityService,
            productAttributeParser,
            shippingService,
            eventPublisher,
            mediaSettings,
            catalogSettings,
            vendorSettings,
            shoppingCartSettings,
            localizationSettings,
            customerSettings,
            captchaSettings,
            seoSettings,
            cacheManager)
        {
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _productService = productService;
            _vendorService = vendorService;
            _productTemplateService = productTemplateService;
            _productAttributeService = productAttributeService;
            _workContext = workContext;
            _storeContext = storeContext;
            _taxService = taxService;
            _currencyService = currencyService;
            _pictureService = pictureService;
            _localizationService = localizationService;
            _measureService = measureService;
            _priceCalculationService = priceCalculationService;
            _priceFormatter = priceFormatter;
            _webHelper = webHelper;
            _specificationAttributeService = specificationAttributeService;
            _dateTimeHelper = dateTimeHelper;
            _recentlyViewedProductsService = recentlyViewedProductsService;
            _compareProductsService = compareProductsService;
            _workflowMessageService = workflowMessageService;
            _productTagService = productTagService;
            _orderReportService = orderReportService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _downloadService = downloadService;
            _customerActivityService = customerActivityService;
            _productAttributeParser = productAttributeParser;
            _shippingService = shippingService;
            _eventPublisher = eventPublisher;
            _mediaSettings = mediaSettings;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _shoppingCartSettings = shoppingCartSettings;
            _localizationSettings = localizationSettings;
            _customerSettings = customerSettings;
            _captchaSettings = captchaSettings;
            _seoSettings = seoSettings;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        public ActionResult ProductDetail(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
                return InvokeHttp404();

            //published?
            if (!_catalogSettings.AllowViewUnpublishedProductPage)
            {
                //Check whether the current user has a "Manage catalog" permission
                //It allows him to preview a product before publishing
                if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                    return InvokeHttp404();
            }

            //ACL (access control list)
            if (!_aclService.Authorize(product))
                return InvokeHttp404();

            //Store mapping
            if (!_storeMappingService.Authorize(product))
                return InvokeHttp404();

            //availability dates
            if (!product.IsAvailable())
                return InvokeHttp404();

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                    return RedirectToRoute("HomePage");

                return RedirectToRoute("Product", new { SeName = parentGroupedProduct.GetSeName() });
            }

            //update existing shopping cart or wishlist  item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    return RedirectToRoute("Product", new { SeName = product.GetSeName() });
                }
            }

            //prepare the model
            var model = PrepareProductDetailsPageModel(product, updatecartitem, false);

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageProducts))
                DisplayEditLink(Url.Action("Edit", "Product", new { id = product.Id, area = "Admin" }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            return View(model.ProductTemplateViewPath, model);
        }
        #endregion
    }
}
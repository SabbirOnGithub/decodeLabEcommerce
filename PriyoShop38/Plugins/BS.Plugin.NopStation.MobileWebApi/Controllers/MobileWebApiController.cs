using System;
using System.Web;
using System.Web.Http;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Security;
using Nop.Services.Configuration;
using Nop.Core.Infrastructure;
using BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel;
using Nop.Web.Framework.Controllers;
using Nop.Core;
using Nop.Services.Stores;
using Nop.Services.Media;
using Nop.Services.Common;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using Nop.Services.Seo;
using Nop.Services.Localization;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Orders;
using Nop.Services.Customers;
using Nop.Services.Tax;
using Nop.Services.Helpers;
using Nop.Services.Affiliates;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Services.Directory;
using System.Globalization;
using Nop.Core.Domain.Localization;
using Nop.Services.Topics;
using Nop.Core.Domain.Blogs;
using Nop.Core.Domain.Forums;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;
using Nop.Web.Framework.Localization;
using BS.Plugin.NopStation.MobileWebApi.Model;
using Nop.Core.Caching;
using System.Diagnostics;
using Nop.Core.Domain.Common;
using Nop.Web.Framework.Events;
using Nop.Services.Events;
using Nop.Services.Messages;
using Nop.Core.Domain.Messages;
using Nop.Core.Plugins;
using Nop.Services.Blogs;
using Nop.Web.Framework.Security.Captcha;
using Nop.Core.Domain.Seo;
using Nop.Services.Shipping;
using Nop.Core.Domain.Vendors;
using Nop.Services.Vendors;
using Nop.Core.Data;
using Nop.Services.Authentication;
using System.Web.Http.Results;
using System.Net.Http;
using System.Net;
using Nop.Services.Discounts;
using Nop.Core.Domain.Directory;
using Nop.Core.Domain.Shipping;
using System.Web.Routing;
using Nop.Services.Payments;
using BS.Plugin.NopStation.MobileWebApi.Extensions;
using Nop.Core.Domain.Payments;
using Nop.Services.Authentication.External;
using Nop.Web.Models.Common;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel;
using Nop.Web.Models.Media;
using BS.Plugin.NopStation.MobileWebApi.Models.Vendor;
//using System.Web.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    //[EnableCors(origins: "*", headers: "*", methods: "*")]
    [RoutePrefix("api/BsWebApi")]
    public class MobileWebApiController : ApiController
    {
        #region Fields
        private readonly IBsNopMobilePluginService _nopMobileService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly IPictureService _pictureService;
        private readonly MobileWebApiSettings _webApiSettings;
        private readonly IProductService _productService;
        private readonly IContentManagementService _contentManagementService;
        private readonly IUrlRecordService _urlRecordService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly ICategoryService _categoryService;
        private readonly MediaSettings _mediaSettings;
        private readonly ILocalizationService _localizationService;
        private readonly ICustomerService _customerService;
        private readonly IProductAttributeFormatter _productAttributeFormatter;
        private readonly IPriceFormatter _priceFormatter;
        private readonly ITaxService _taxService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IAffiliateService _affiliateService;
        private readonly IShoppingCartService _shoppingCartService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ShoppingCartSettings _shoppingCartSettings;
        private readonly ICurrencyService _currencyService;
        private readonly IProductAttributeParser _productAttributeParser;
        private readonly IProductAttributeService _productAttributeService;
        private readonly IDownloadService _downloadService;
        private readonly ILanguageService _languageService;
        private readonly LocalizationSettings _localizationSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly ITopicService _topicService;
        private readonly BlogSettings _blogSettings;
        private readonly ForumSettings _forumSettings;
        private readonly OrderSettings _orderSettings;
        private readonly TaxSettings _taxSettings;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICheckoutAttributeService _checkoutAttributeService;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWebHelper _webHelper;
        private readonly IAclService _aclService;
        private readonly IManufacturerService _manufacturerService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly ISearchTermService _searchTermService;
        private readonly IEventPublisher _eventPublisher;
        private readonly IOrderReportService _orderReportService;
        private readonly IRecentlyViewedProductsService _recentlyViewedProductsService;
        private readonly INewsLetterSubscriptionService _newsLetterSubscriptionService;
        private readonly IWorkflowMessageService _workflowMessageService;
        private readonly ICompareProductsService _compareProductsService;
        private readonly IPluginFinder _pluginFinder;
        private readonly IBlogService _blogService;
        private readonly CustomerSettings _customerSettings;
        private readonly CaptchaSettings _captchaSettings;
        private readonly SeoSettings _seoSettings;
        private readonly IShippingService _shippingService;
        private readonly VendorSettings _vendorSettings;
        private readonly IVendorService _vendorService;
        private readonly IProductTagService _productTagService;
        private readonly IMeasureService _measureService;
        private readonly IRepository<ProductReview> _productReviewRepository;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly ICheckoutAttributeFormatter _checkoutAttributeFormatter;
        private readonly IDiscountService _discountService;
        private readonly ICountryService _countryService;
        private readonly IStateProvinceService _stateProvinceService;
        private readonly IAddressAttributeFormatter _addressAttributeFormatter;
        private readonly AddressSettings _addressSettings;
        private readonly IPaymentService _paymentService;
        private readonly ShippingSettings _shippingSettings;
        private readonly ICheckoutAttributeParser _checkoutAttributeParser;
        private readonly RewardPointsSettings _rewardPointsSettings;
        private readonly ICategoryTemplateService _categoryTemplateService;
        private readonly IAddressAttributeParser _addressAttributeParser;
        private readonly PaymentSettings _paymentSettings;
        private readonly ILogger _logger;
        private readonly IOrderService _orderService;
        private readonly IAddressAttributeService _addressAttributeService;
        private readonly DateTimeSettings _dateTimeSettings;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly IOpenAuthenticationService _openAuthenticationService;
        private readonly ExternalAuthenticationSettings _externalAuthenticationSettings;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IPdfService _pdfService;
        private readonly IShipmentService _shipmentService;
        private readonly PdfSettings _pdfSettings;
        private readonly IAddressService _addressService;
        private readonly IRewardPointService _rewardPointService;
        private readonly HttpContextBase _httpContext;

        #endregion

        #region Ctor
        public MobileWebApiController(IRewardPointService rewardPointService, IRewardPointService rewardPointService1, HttpContextBase httpContext, HttpContextBase httpContext1)
        {
            _rewardPointService = rewardPointService;
            _rewardPointService = rewardPointService1;
            _httpContext = httpContext;
            _httpContext = httpContext1;
            _nopMobileService = EngineContext.Current.Resolve<IBsNopMobilePluginService>();
            _permissionService = EngineContext.Current.Resolve<IPermissionService>();
            _settingService = EngineContext.Current.Resolve<ISettingService>();
            _workContext = EngineContext.Current.Resolve<IWorkContext>();
            _storeContext = EngineContext.Current.Resolve<IStoreContext>();
            _storeService = EngineContext.Current.Resolve<IStoreService>();
            _pictureService = EngineContext.Current.Resolve<IPictureService>();
            _webApiSettings = EngineContext.Current.Resolve<MobileWebApiSettings>();
            _productService = EngineContext.Current.Resolve<IProductService>();
            _contentManagementService = EngineContext.Current.Resolve<IContentManagementService>();
            _urlRecordService = EngineContext.Current.Resolve<IUrlRecordService>();
            _storeMappingService = EngineContext.Current.Resolve<IStoreMappingService>();
            _localizedEntityService = EngineContext.Current.Resolve<ILocalizedEntityService>();
            _categoryService = EngineContext.Current.Resolve<ICategoryService>();
            _mediaSettings = EngineContext.Current.Resolve<MediaSettings>();
            _localizationService = EngineContext.Current.Resolve<ILocalizationService>();
            _customerService = EngineContext.Current.Resolve<ICustomerService>();
            _productAttributeFormatter = EngineContext.Current.Resolve<IProductAttributeFormatter>();
            _priceFormatter = EngineContext.Current.Resolve<IPriceFormatter>();
            _taxService = EngineContext.Current.Resolve<ITaxService>();
            _priceCalculationService = EngineContext.Current.Resolve<IPriceCalculationService>();
            _dateTimeHelper = EngineContext.Current.Resolve<IDateTimeHelper>();
            _affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
            _shoppingCartService = EngineContext.Current.Resolve<IShoppingCartService>();
            _customerActivityService = EngineContext.Current.Resolve<ICustomerActivityService>();
            _shoppingCartSettings = EngineContext.Current.Resolve<ShoppingCartSettings>();
            _currencyService = EngineContext.Current.Resolve<ICurrencyService>();
            _productAttributeParser = EngineContext.Current.Resolve<IProductAttributeParser>();
            _productAttributeService = EngineContext.Current.Resolve<IProductAttributeService>();
            _downloadService = EngineContext.Current.Resolve<IDownloadService>();
            _languageService = EngineContext.Current.Resolve<ILanguageService>();
            _localizationSettings = EngineContext.Current.Resolve<LocalizationSettings>();
            _catalogSettings = EngineContext.Current.Resolve<CatalogSettings>();
            _topicService = EngineContext.Current.Resolve<ITopicService>();
            _blogSettings = EngineContext.Current.Resolve<BlogSettings>();
            _forumSettings = EngineContext.Current.Resolve<ForumSettings>();
            _orderSettings = EngineContext.Current.Resolve<OrderSettings>();
            _taxSettings = EngineContext.Current.Resolve<TaxSettings>();
            _orderTotalCalculationService = EngineContext.Current.Resolve<IOrderTotalCalculationService>();
            _checkoutAttributeService = EngineContext.Current.Resolve<ICheckoutAttributeService>();
            _orderProcessingService = EngineContext.Current.Resolve<IOrderProcessingService>();
            _genericAttributeService = EngineContext.Current.Resolve<IGenericAttributeService>();
            _webHelper = EngineContext.Current.Resolve<IWebHelper>();
            _aclService = EngineContext.Current.Resolve<IAclService>();
            _manufacturerService = EngineContext.Current.Resolve<IManufacturerService>();
            _specificationAttributeService = EngineContext.Current.Resolve<ISpecificationAttributeService>();
            _searchTermService = EngineContext.Current.Resolve<ISearchTermService>();
            _eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();
            _orderReportService = EngineContext.Current.Resolve<IOrderReportService>();
            _recentlyViewedProductsService = EngineContext.Current.Resolve<IRecentlyViewedProductsService>();
            _newsLetterSubscriptionService = EngineContext.Current.Resolve<INewsLetterSubscriptionService>();
            _workflowMessageService = EngineContext.Current.Resolve<IWorkflowMessageService>();
            _compareProductsService = EngineContext.Current.Resolve<ICompareProductsService>();
            _pluginFinder = EngineContext.Current.Resolve<IPluginFinder>();
            _blogService = EngineContext.Current.Resolve<IBlogService>();
            _customerSettings = EngineContext.Current.Resolve<CustomerSettings>();
            _captchaSettings = EngineContext.Current.Resolve<CaptchaSettings>();
            _seoSettings = EngineContext.Current.Resolve<SeoSettings>();
            _shippingService = EngineContext.Current.Resolve<IShippingService>();
            _vendorService = EngineContext.Current.Resolve<IVendorService>();
            _vendorSettings = EngineContext.Current.Resolve<VendorSettings>();
            _productTagService = EngineContext.Current.Resolve<IProductTagService>();
            _measureService = EngineContext.Current.Resolve<IMeasureService>();
            _productReviewRepository = EngineContext.Current.Resolve<IRepository<ProductReview>>();
            _customerRegistrationService = EngineContext.Current.Resolve<ICustomerRegistrationService>();
            _authenticationService = EngineContext.Current.Resolve<IAuthenticationService>();
            _checkoutAttributeFormatter = EngineContext.Current.Resolve<ICheckoutAttributeFormatter>();
            _discountService = EngineContext.Current.Resolve<IDiscountService>();
            _stateProvinceService = EngineContext.Current.Resolve<IStateProvinceService>();
            _addressAttributeFormatter = EngineContext.Current.Resolve<IAddressAttributeFormatter>();
            _addressSettings = EngineContext.Current.Resolve<AddressSettings>();
            _paymentService = EngineContext.Current.Resolve<IPaymentService>();
            _shippingSettings = EngineContext.Current.Resolve<ShippingSettings>();
            _checkoutAttributeParser = EngineContext.Current.Resolve<ICheckoutAttributeParser>();
            _rewardPointsSettings = EngineContext.Current.Resolve<RewardPointsSettings>();
            _countryService = EngineContext.Current.Resolve<ICountryService>();
            _categoryTemplateService = EngineContext.Current.Resolve<ICategoryTemplateService>();
            _addressAttributeParser = EngineContext.Current.Resolve<IAddressAttributeParser>();
            _paymentSettings = EngineContext.Current.Resolve<PaymentSettings>();
            _logger = EngineContext.Current.Resolve<ILogger>();
            _orderService = EngineContext.Current.Resolve<IOrderService>();
            _addressAttributeService = EngineContext.Current.Resolve<IAddressAttributeService>();
            _dateTimeSettings = EngineContext.Current.Resolve<DateTimeSettings>();
            _customerAttributeService = EngineContext.Current.Resolve<ICustomerAttributeService>();
            _openAuthenticationService = EngineContext.Current.Resolve<IOpenAuthenticationService>();
            _externalAuthenticationSettings = EngineContext.Current.Resolve<ExternalAuthenticationSettings>();
            _customerAttributeParser = EngineContext.Current.Resolve<ICustomerAttributeParser>();
            _pdfService = EngineContext.Current.Resolve<IPdfService>();
            _shipmentService = EngineContext.Current.Resolve<IShipmentService>();
            _pdfSettings = EngineContext.Current.Resolve<PdfSettings>();
            _addressService = EngineContext.Current.Resolve<IAddressService>();
        }
        #endregion

        #region Utilities

        [NonAction]
        public static ManufacturerModel MToModel(Manufacturer entity)
        {
            if (entity == null)
                return null;

            var model = new ManufacturerModel
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
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
        protected virtual string ParseProductAttributes(Product product, System.Web.Mvc.FormCollection form)
        {
            string attributesXml = "";

            #region Product attributes
            var productAttributes = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);
            foreach (var attribute in productAttributes)
            {
                string controlId = string.Format("product_attribute_{0}_{1}_{2}", attribute.ProductId, attribute.ProductAttributeId, attribute.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.ColorSquares:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                int selectedAttributeId = int.Parse(ctrlAttributes);
                                if (selectedAttributeId > 0)
                                    attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.Checkboxes:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                foreach (var item in ctrlAttributes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    int selectedAttributeId = int.Parse(item);
                                    if (selectedAttributeId > 0)
                                        attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                            attribute, selectedAttributeId.ToString());
                                }
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //load read-only (already server-side selected) values
                            var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                            foreach (var selectedAttributeId in attributeValues
                                .Where(v => v.IsPreSelected)
                                .Select(v => v.Id)
                                .ToList())
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedAttributeId.ToString());
                            }
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            var ctrlAttributes = form[controlId];
                            if (!String.IsNullOrEmpty(ctrlAttributes))
                            {
                                string enteredText = ctrlAttributes.Trim();
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, enteredText);
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            var day = form[controlId + "_day"];
                            var month = form[controlId + "_month"];
                            var year = form[controlId + "_year"];
                            DateTime? selectedDate = null;
                            try
                            {
                                selectedDate = new DateTime(Int32.Parse(year), Int32.Parse(month), Int32.Parse(day));
                            }
                            catch { }
                            if (selectedDate.HasValue)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                    attribute, selectedDate.Value.ToString("D"));
                            }
                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            Guid downloadGuid;
                            Guid.TryParse(form[controlId], out downloadGuid);
                            var download = _downloadService.GetDownloadByGuid(downloadGuid);
                            if (download != null)
                            {
                                attributesXml = _productAttributeParser.AddProductAttribute(attributesXml,
                                        attribute, download.DownloadGuid.ToString());
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            #endregion

            #region Gift cards

            if (product.IsGiftCard)
            {
                string recipientName = "";
                string recipientEmail = "";
                string senderName = "";
                string senderEmail = "";
                string giftCardMessage = "";
                foreach (string formKey in form.AllKeys)
                {
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.RecipientEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        recipientEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderName", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderName = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.SenderEmail", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        senderEmail = form[formKey];
                        continue;
                    }
                    if (formKey.Equals(string.Format("giftcard_{0}.Message", product.Id), StringComparison.InvariantCultureIgnoreCase))
                    {
                        giftCardMessage = form[formKey];
                        continue;
                    }
                }

                attributesXml = _productAttributeParser.AddGiftCardAttribute(attributesXml,
                    recipientName, recipientEmail, senderName, senderEmail, giftCardMessage);
            }

            #endregion

            return attributesXml;
        }

        /// <summary>
        /// Parse product rental dates on the product details page
        /// </summary>
        /// <param name="product">Product</param>
        /// <param name="form">Form</param>
        /// <param name="startDate">Start date</param>
        /// <param name="endDate">End date</param>
        [NonAction]
        protected virtual void ParseRentalDates(Product product, System.Web.Mvc.FormCollection form,
            out DateTime? startDate, out DateTime? endDate)
        {
            startDate = null;
            endDate = null;

            string startControlId = string.Format("rental_start_date_{0}", product.Id);
            string endControlId = string.Format("rental_end_date_{0}", product.Id);
            var ctrlStartDate = form[startControlId];
            var ctrlEndDate = form[endControlId];
            try
            {
                //currenly we support only this format (as in the \Views\Product\_RentalInfo.cshtml file)
                const string datePickerFormat = "MM/dd/yyyy";
                startDate = DateTime.ParseExact(ctrlStartDate, datePickerFormat, CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(ctrlEndDate, datePickerFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
            }
        }

        [NonAction]
        private IList<CategoryModel.SubCategoryModel> PrepaireSubCategory(int id)
        {
            return _categoryService.GetAllCategoriesByParentCategoryId(id)
               .Select(x =>
               {
                   var subCatModel = new CategoryModel.SubCategoryModel
                   {
                       Id = x.Id,
                       Name = x.GetLocalized(y => y.Name),
                       SeName = x.GetSeName(),
                   };

                   //prepare picture model
                   int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                   var picture = _pictureService.GetPictureById(x.PictureId);
                   subCatModel.PictureModel = new PictureModel
                   {
                       FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                       ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                       Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                       AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                   };

                   return subCatModel;
               })
               .ToList();
        }

        private int GetActiveStoreScopeConfiguration()
        {
            int storeScope = 0;
            if (_storeService.GetAllStores().Count < 2)
                storeScope = 0;

            var storeId = _workContext.CurrentCustomer.GetAttribute<int>(SystemCustomerAttributeNames.AdminAreaStoreScopeConfiguration);
            var store = _storeService.GetStoreById(storeId);
            storeScope = store != null ? store.Id : 0;
            return storeScope;
        }
        [NonAction]
        protected virtual List<int> GetChildCategoryIds(int parentCategoryId)
        {
            var categoriesIds = new List<int>();
            var categories = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId);
            foreach (var category in categories)
            {
                categoriesIds.Add(category.Id);
                categoriesIds.AddRange(GetChildCategoryIds(category.Id));
            }
            return categoriesIds;
        }

        [NonAction]
        protected virtual IList<CategorySimpleModel> PrepareCategorySimpleModels(int rootCategoryId,
            bool loadSubCategories = true, IList<Category> allCategories = null)
        {
            var result = new List<CategorySimpleModel>();
            if (allCategories == null)
            {
                allCategories = _categoryService.GetAllCategories();
            }
            var categories = allCategories.Where(c => c.ParentCategoryId == rootCategoryId).ToList();
            foreach (var category in categories)
            {
                var categoryModel = new CategorySimpleModel
                {
                    Id = category.Id,
                    Name = category.GetLocalized(x => x.Name),
                    SeName = category.GetSeName(),
                    IncludeInTopMenu = category.IncludeInTopMenu
                };

                //product number for each category
                if (_catalogSettings.ShowCategoryProductNumber)
                {
                    var categoryIds = new List<int>();
                    categoryIds.Add(category.Id);
                    //include subcategories
                    if (_catalogSettings.ShowCategoryProductNumberIncludingSubcategories)
                        categoryIds.AddRange(GetChildCategoryIds(category.Id));
                    //categoryModel.NumberOfProducts = _productService.GetCategoryProductNumber(categoryIds, _storeContext.CurrentStore.Id);
                    categoryModel.NumberOfProducts = _productService.GetNumberOfProductsInCategory(categoryIds, _storeContext.CurrentStore.Id);
                }
                if (loadSubCategories)
                {
                    var subCategories = PrepareCategorySimpleModels(category.Id, loadSubCategories, allCategories);
                    categoryModel.SubCategories.AddRange(subCategories);
                }
                result.Add(categoryModel);
            }

            return result;
        }

        [NonAction]
        protected virtual PictureModel PrepareCartItemPictureModel(ShoppingCartItem sci,
            int pictureSize, bool showDefaultPicture, string productName)
        {
            //shopping cart item picture
            var sciPicture = sci.Product.GetProductPicture(sci.AttributesXml, _pictureService, _productAttributeParser);
            return new PictureModel
            {
                ImageUrl = _pictureService.GetPictureUrl(sciPicture, pictureSize, showDefaultPicture),
                Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), productName),
                AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), productName),
            };
        }

        [NonAction]
        protected virtual MiniShoppingCartModel PrepareMiniShoppingCartModel()
        {
            var model = new MiniShoppingCartModel
            {
                ShowProductImages = _shoppingCartSettings.ShowProductImagesInMiniShoppingCart,
                //let's always display it
                DisplayShoppingCartButton = true,
                CurrentCustomerIsGuest = _workContext.CurrentCustomer.IsGuest(),
                AnonymousCheckoutAllowed = _orderSettings.AnonymousCheckoutAllowed,
            };


            //performance optimization (use "HasShoppingCartItems" property)
            if (_workContext.CurrentCustomer.HasShoppingCartItems)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                model.TotalProducts = cart.GetTotalProducts();
                if (cart.Count > 0)
                {
                    //subtotal
                    decimal orderSubTotalDiscountAmountBase;
                    //Discount orderSubTotalAppliedDiscount;
                    List<Discount> orderSubTotalAppliedDiscounts; //change 3.8
                    decimal subTotalWithoutDiscountBase;
                    decimal subTotalWithDiscountBase;
                    var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                        out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                        out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                    decimal subtotalBase = subTotalWithoutDiscountBase;
                    decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                    model.SubTotal = _priceFormatter.FormatPrice(subtotal, false, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                    var requiresShipping = cart.RequiresShipping();
                    //a customer should visit the shopping cart page (hide checkout button) before going to checkout if:
                    //1. "terms of service" are enabled
                    //2. min order sub-total is OK
                    //3. we have at least one checkout attribute

                    var checkoutAttributesExist = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !requiresShipping).Count > 0;

                    bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
                    model.DisplayCheckoutButton = !_orderSettings.TermsOfServiceOnShoppingCartPage &&
                        minOrderSubtotalAmountOk &&
                        !checkoutAttributesExist;

                    //products. sort descending (recently added products)
                    foreach (var sci in cart
                        .OrderByDescending(x => x.Id)
                        .Take(_shoppingCartSettings.MiniShoppingCartProductNumber)
                        .ToList())
                    {
                        var cartItemModel = new MiniShoppingCartModel.ShoppingCartItemModel
                        {
                            Id = sci.Id,
                            ProductId = sci.Product.Id,
                            ProductName = sci.Product.GetLocalized(x => x.Name),
                            ProductSeName = sci.Product.GetSeName(),
                            Quantity = sci.Quantity,
                            AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml)
                        };

                        //unit prices
                        if (sci.Product.CallForPrice)
                        {
                            cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                        }
                        else
                        {
                            decimal taxRate;
                            decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                            decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                        }

                        //picture
                        if (_shoppingCartSettings.ShowProductImagesInMiniShoppingCart)
                        {
                            cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                                _mediaSettings.MiniCartThumbPictureSize, true, cartItemModel.ProductName);
                        }

                        model.Items.Add(cartItemModel);
                    }
                }
            }

            return model;
        }

        [NonAction]
        protected virtual void PrepareSortingOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductSorting = _catalogSettings.AllowProductSorting;
            if (pagingFilteringModel.AllowProductSorting)
            {
                foreach (ProductSortingEnum enumValue in Enum.GetValues(typeof(ProductSortingEnum)))
                {
                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "orderby=" + ((int)enumValue).ToString(), null);

                    var sortValue = enumValue.GetLocalizedEnum(_localizationService, _workContext);
                    pagingFilteringModel.AvailableSortOptions.Add(new SelectListItem
                    {
                        Text = sortValue,
                        Value = sortUrl,
                        Selected = enumValue == (ProductSortingEnum)command.OrderBy
                    });
                }
            }
        }

        [NonAction]
        protected virtual void PrepareViewModes(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            pagingFilteringModel.AllowProductViewModeChanging = _catalogSettings.AllowProductViewModeChanging;

            var viewMode = !string.IsNullOrEmpty(command.ViewMode)
                ? command.ViewMode
                : _catalogSettings.DefaultViewMode;
            pagingFilteringModel.ViewMode = viewMode;
            if (pagingFilteringModel.AllowProductViewModeChanging)
            {
                var currentPageUrl = _webHelper.GetThisPageUrl(true);
                //grid
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.Grid"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=grid", null),
                    Selected = viewMode == "grid"
                });
                //list
                pagingFilteringModel.AvailableViewModes.Add(new SelectListItem
                {
                    Text = _localizationService.GetResource("Catalog.ViewMode.List"),
                    Value = _webHelper.ModifyQueryString(currentPageUrl, "viewmode=list", null),
                    Selected = viewMode == "list"
                });
            }

        }

        [NonAction]
        protected virtual void PreparePageSizeOptions(CatalogPagingFilteringModel pagingFilteringModel, CatalogPagingFilteringModel command,
            bool allowCustomersToSelectPageSize, string pageSizeOptions, int fixedPageSize)
        {
            if (pagingFilteringModel == null)
                throw new ArgumentNullException("pagingFilteringModel");

            if (command == null)
                throw new ArgumentNullException("command");

            if (command.PageNumber <= 0)
            {
                command.PageNumber = 1;
            }
            pagingFilteringModel.AllowCustomersToSelectPageSize = false;
            if (allowCustomersToSelectPageSize && pageSizeOptions != null)
            {
                var pageSizes = pageSizeOptions.Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                if (pageSizes.Any())
                {
                    // get the first page size entry to use as the default (category page load) or if customer enters invalid value via query string
                    if (command.PageSize <= 0 || !pageSizes.Contains(command.PageSize.ToString()))
                    {
                        int temp;
                        if (int.TryParse(pageSizes.FirstOrDefault(), out temp))
                        {
                            if (temp > 0)
                            {
                                command.PageSize = temp;
                            }
                        }
                    }

                    var currentPageUrl = _webHelper.GetThisPageUrl(true);
                    var sortUrl = _webHelper.ModifyQueryString(currentPageUrl, "pagesize={0}", null);
                    sortUrl = _webHelper.RemoveQueryString(sortUrl, "pagenumber");

                    foreach (var pageSize in pageSizes)
                    {
                        int temp;
                        if (!int.TryParse(pageSize, out temp))
                        {
                            continue;
                        }
                        if (temp <= 0)
                        {
                            continue;
                        }

                        pagingFilteringModel.PageSizeOptions.Add(new SelectListItem
                        {
                            Text = pageSize,
                            Value = String.Format(sortUrl, pageSize),
                            Selected = pageSize.Equals(command.PageSize.ToString(), StringComparison.InvariantCultureIgnoreCase)
                        });
                    }

                    if (pagingFilteringModel.PageSizeOptions.Any())
                    {
                        pagingFilteringModel.PageSizeOptions = pagingFilteringModel.PageSizeOptions.OrderBy(x => int.Parse(x.Text)).ToList();
                        pagingFilteringModel.AllowCustomersToSelectPageSize = true;

                        if (command.PageSize <= 0)
                        {
                            command.PageSize = int.Parse(pagingFilteringModel.PageSizeOptions.FirstOrDefault().Text);
                        }
                    }
                }
            }
            else
            {
                //customer is not allowed to select a page size
                command.PageSize = fixedPageSize;
            }

            //ensure pge size is specified
            if (command.PageSize <= 0)
            {
                command.PageSize = fixedPageSize;
            }
        }

        [NonAction]
        protected virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false, int CategoryId = 0)
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
                    CategoryId = CategoryId,
                    Sku = product.Sku,
                    //ManufacturerId = product.manu

                };
                //price
                if (preparePriceModel)
                {
                    #region Prepare product price

                    var priceModel = new ProductOverviewModel.ProductPriceModel
                    {
                        ForceRedirectionAfterAddingToCart = forceRedirectionAfterAddingToCart
                    };

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
                                            //priceModel.DisableBuyButton = true;
                                            //priceModel.DisableWishlistButton = true;
                                            //compare products
                                            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
                                            //priceModel.AvailableForPreOrder = false;
                                        }
                                        break;
                                    default:
                                        {
                                            //we have at least one associated product
                                            //priceModel.DisableBuyButton = true;
                                            //priceModel.DisableWishlistButton = true;
                                            //compare products
                                            priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;
                                            //priceModel.AvailableForPreOrder = false;

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
                                                        Debug.WriteLine("Cannot calculate minPrice for product #{0}", product.Id);
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
                                //compare products
                                priceModel.DisableAddToCompareListButton = !_catalogSettings.CompareProductsEnabled;

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


                                            //property for German market
                                            //we display tax/shipping info only with "shipping enabled" for this product
                                            //we also ensure this it's not free shipping
                                            priceModel.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductBoxes
                                                && product.IsShipEnabled &&
                                                !product.IsFreeShipping;
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
                    int pictureSize = productThumbPictureSize.HasValue ? productThumbPictureSize.Value : _mediaSettings.ProductThumbPictureSize;
                    var picture = _pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
                    var pictureModel = new PictureModel
                    {
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture)
                    };
                    //"title" attribute
                    pictureModel.Title = (picture != null && !string.IsNullOrEmpty(picture.TitleAttribute)) ?
                        picture.TitleAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat"), model.Name);
                    //"alt" attribute
                    pictureModel.AlternateText = (picture != null && !string.IsNullOrEmpty(picture.AltAttribute)) ?
                        picture.AltAttribute :
                        string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat"), model.Name);

                    model.DefaultPictureModel = pictureModel;
                    #endregion
                }

                //specs
                if (prepareSpecificationAttributes)
                {
                    model.SpecificationAttributeModels = PrepareProductSpecificationModel(product);
                }

                //reviews
                model.ReviewOverviewModel = new ProductReviewOverviewModel
                {
                    ProductId = product.Id,
                    RatingSum = product.ApprovedRatingSum,
                    TotalReviews = product.ApprovedTotalReviews,
                    AllowCustomerReviews = product.AllowCustomerReviews
                };

                models.Add(model);
            }
            return models;
        }
        [NonAction]
        public IList<ProductSpecificationModel> PrepareProductSpecificationModel(Product product)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            return _specificationAttributeService.GetProductSpecificationAttributes(product.Id, 0, null, true)
                .Select(psa =>
                {
                    var m = new ProductSpecificationModel
                    {
                        SpecificationAttributeId = psa.SpecificationAttributeOption.SpecificationAttributeId,
                        SpecificationAttributeName = psa.SpecificationAttributeOption.SpecificationAttribute.GetLocalized(x => x.Name),
                    };

                    switch (psa.AttributeType)
                    {
                        case SpecificationAttributeType.Option:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.SpecificationAttributeOption.GetLocalized(x => x.Name));
                            break;
                        case SpecificationAttributeType.CustomText:
                            m.ValueRaw = HttpUtility.HtmlEncode(psa.CustomValue);
                            break;
                        case SpecificationAttributeType.CustomHtmlText:
                            m.ValueRaw = psa.CustomValue;
                            break;
                        case SpecificationAttributeType.Hyperlink:
                            m.ValueRaw = string.Format("<a href='{0}' target='_blank'>{0}</a>", psa.CustomValue);
                            break;
                        default:
                            break;
                    }
                    return m;
                }).ToList();
        }

        [NonAction]
        public static ManufacturerModel ToModel(Manufacturer entity)
        {
            if (entity == null)
                return null;

            var model = new ManufacturerModel
            {
                Id = entity.Id,
                Name = entity.GetLocalized(x => x.Name),
                Description = entity.GetLocalized(x => x.Description),
                MetaKeywords = entity.GetLocalized(x => x.MetaKeywords),
                MetaDescription = entity.GetLocalized(x => x.MetaDescription),
                MetaTitle = entity.GetLocalized(x => x.MetaTitle),
                SeName = entity.GetSeName(),
            };
            return model;
        }

        [NonAction]
        protected string GetPictureUrl(int pictureId)
        {
            var url = _pictureService.GetPictureUrl(pictureId, showDefaultPicture: false);
            //little hack here. nulls aren't cacheable so set it to ""
            if (url == null)
                url = "";

            return url;
        }

        [NonAction]
        protected virtual BlogPostListModel PrepareBlogPostListModel(BlogPagingFilteringModel command)
        {
            if (command == null)
                throw new ArgumentNullException("command");

            var model = new BlogPostListModel();
            model.PagingFilteringContext.Tag = command.Tag;
            model.PagingFilteringContext.Month = command.Month;
            model.WorkingLanguageId = _workContext.WorkingLanguage.Id;

            if (command.PageSize <= 0) command.PageSize = _blogSettings.PostsPageSize;
            if (command.PageNumber <= 0) command.PageNumber = 1;

            DateTime? dateFrom = command.GetFromMonth();
            DateTime? dateTo = command.GetToMonth();

            IPagedList<BlogPost> blogPosts;
            if (String.IsNullOrEmpty(command.Tag))
            {
                blogPosts = _blogService.GetAllBlogPosts(_storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id,
                    dateFrom, dateTo, command.PageNumber - 1, command.PageSize);
            }
            else
            {
                blogPosts = _blogService.GetAllBlogPostsByTag(_storeContext.CurrentStore.Id,
                    _workContext.WorkingLanguage.Id,
                    command.Tag, command.PageNumber - 1, command.PageSize);
            }
            model.PagingFilteringContext.LoadPagedList(blogPosts);

            model.BlogPosts = blogPosts
                .Select(x =>
                {
                    var blogPostModel = new BlogPostModel();
                    PrepareBlogPostModel(blogPostModel, x, false);
                    return blogPostModel;
                })
                .ToList();

            return model;
        }

        [NonAction]
        protected virtual void PrepareBlogPostModel(BlogPostModel model, BlogPost blogPost, bool prepareComments)
        {
            if (blogPost == null)
                throw new ArgumentNullException("blogPost");

            if (model == null)
                throw new ArgumentNullException("model");

            model.Id = blogPost.Id;
            model.MetaTitle = blogPost.MetaTitle;
            model.MetaDescription = blogPost.MetaDescription;
            model.MetaKeywords = blogPost.MetaKeywords;
            model.SeName = blogPost.GetSeName(blogPost.LanguageId, ensureTwoPublishedLanguages: false);
            model.Title = blogPost.Title;
            model.Body = blogPost.Body;
            model.BodyOverview = blogPost.BodyOverview;
            model.AllowComments = blogPost.AllowComments;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(blogPost.CreatedOnUtc, DateTimeKind.Utc);
            model.Tags = blogPost.ParseTags().ToList();
            model.NumberOfComments = blogPost.CommentCount;
            model.AddNewComment.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnBlogCommentPage;
            if (prepareComments)
            {
                var blogComments = blogPost.BlogComments.OrderBy(pr => pr.CreatedOnUtc);
                foreach (var bc in blogComments)
                {
                    var commentModel = new BlogCommentModel
                    {
                        Id = bc.Id,
                        CustomerId = bc.CustomerId,
                        CustomerName = bc.Customer.FormatUserName(),
                        CommentText = bc.CommentText,
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(bc.CreatedOnUtc, DateTimeKind.Utc),
                        AllowViewingProfiles = _customerSettings.AllowViewingProfiles && bc.Customer != null && !bc.Customer.IsGuest(),
                    };
                    if (_customerSettings.AllowCustomersToUploadAvatars)
                    {
                        commentModel.CustomerAvatarUrl = _pictureService.GetPictureUrl(
                            bc.Customer.GetAttribute<int>(SystemCustomerAttributeNames.AvatarPictureId),
                            _mediaSettings.AvatarPictureSize,
                            _customerSettings.DefaultAvatarEnabled,
                            defaultPictureType: PictureType.Avatar);
                    }
                    model.Comments.Add(commentModel);
                }
            }
        }

        [NonAction]
        protected virtual ProductDetailsModel PrepareProductDetailsPageModel(Product product,
            ShoppingCartItem updatecartitem = null, bool isAssociatedProduct = false)
        {
            if (product == null)
                throw new ArgumentNullException("product");

            #region Standard properties

            var model = new ProductDetailsModel
            {
                Id = product.Id,
                Name = product.GetLocalized(x => x.Name),
                ShortDescription = product.GetLocalized(x => x.ShortDescription),
                FullDescription = product.GetLocalized(x => x.FullDescription),
                MetaKeywords = product.GetLocalized(x => x.MetaKeywords),
                MetaDescription = product.GetLocalized(x => x.MetaDescription),
                MetaTitle = product.GetLocalized(x => x.MetaTitle),
                SeName = product.GetSeName(),
                ShowSku = _catalogSettings.ShowProductSku,
                Sku = product.Sku,
                ShowManufacturerPartNumber = _catalogSettings.ShowManufacturerPartNumber,
                FreeShippingNotificationEnabled = _catalogSettings.ShowFreeShippingNotification,
                ManufacturerPartNumber = product.ManufacturerPartNumber,
                ShowGtin = _catalogSettings.ShowGtin,
                Gtin = product.Gtin,
                StockAvailability =  product.FormatStockMessage("", _localizationService, _productAttributeParser),
                HasSampleDownload = product.IsDownload && product.HasSampleDownload,
            };

            //automatically generate product description?
            if (_seoSettings.GenerateProductMetaDescription && String.IsNullOrEmpty(model.MetaDescription))
            {
                //based on short description
                model.MetaDescription = model.ShortDescription;
            }

            //shipping info
            model.IsShipEnabled = product.IsShipEnabled;
            if (product.IsShipEnabled)
            {
                model.IsFreeShipping = product.IsFreeShipping;
                //delivery date
                var deliveryDate = _shippingService.GetDeliveryDateById(product.DeliveryDateId);
                if (deliveryDate != null)
                {
                    model.DeliveryDate = deliveryDate.GetLocalized(dd => dd.Name);
                }
            }

            //email a friend
            model.EmailAFriendEnabled = _catalogSettings.EmailAFriendEnabled;
            //compare products
            model.CompareProductsEnabled = _catalogSettings.CompareProductsEnabled;

            #endregion

            #region Vendor details

            //vendor
            if (_vendorSettings.ShowVendorOnProductDetailsPage)
            {
                var vendor = _vendorService.GetVendorById(product.VendorId);
                if (vendor != null && !vendor.Deleted && vendor.Active)
                {
                    model.ShowVendor = true;

                    model.VendorModel = new VendorBriefInfoModel
                    {
                        Id = vendor.Id,
                        Name = vendor.GetLocalized(x => x.Name),
                        SeName = vendor.GetSeName(),
                    };
                }
            }

            #endregion

            #region Page sharing

            if (_catalogSettings.ShowShareButton && !String.IsNullOrEmpty(_catalogSettings.PageShareCode))
            {
                var shareCode = _catalogSettings.PageShareCode;
                if (_webHelper.IsCurrentConnectionSecured())
                {
                    //need to change the addthis link to be https linked when the page is, so that the page doesnt ask about mixed mode when viewed in https...
                    shareCode = shareCode.Replace("http://", "https://");
                }
                model.PageShareCode = shareCode;
            }

            #endregion

            #region Back in stock subscriptions

            if (product.ManageInventoryMethod == ManageInventoryMethod.ManageStock &&
                product.BackorderMode == BackorderMode.NoBackorders &&
                product.AllowBackInStockSubscriptions &&
                product.GetTotalStockQuantity() <= 0)
            {
                //out of stock
                model.DisplayBackInStockSubscription = true;
            }

            #endregion

            #region Breadcrumb

            //do not prepare this model for the associated products. anyway it's not used
            if (_catalogSettings.CategoryBreadcrumbEnabled && !isAssociatedProduct)
            {
                var breadcrumbModel = new ProductDetailsModel.ProductBreadcrumbModel
                {
                    Enabled = _catalogSettings.CategoryBreadcrumbEnabled,
                    ProductId = product.Id,
                    ProductName = product.GetLocalized(x => x.Name),
                    ProductSeName = product.GetSeName()
                };
                var productCategories = _categoryService.GetProductCategoriesByProductId(product.Id);
                if (productCategories.Count > 0)
                {
                    var category = productCategories[0].Category;
                    if (category != null)
                    {
                        foreach (var catBr in category.GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService))
                        {
                            breadcrumbModel.CategoryBreadcrumb.Add(new CategorySimpleModel
                            {
                                Id = catBr.Id,
                                Name = catBr.GetLocalized(x => x.Name),
                                SeName = catBr.GetSeName(),
                                IncludeInTopMenu = catBr.IncludeInTopMenu
                            });
                        }
                    }
                }
                model.Breadcrumb = breadcrumbModel;
            }

            #endregion

            #region Product tags

            //do not prepare this model for the associated products. anyway it's not used
            if (!isAssociatedProduct)
            {
                model.ProductTags =
                    product.ProductTags
                    //filter by store
                    .Where(x => _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id) > 0)
                    .Select(x => new ProductTagModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                        ProductCount = _productTagService.GetProductCount(x.Id, _storeContext.CurrentStore.Id)
                    })
                    .ToList();
            }

            #endregion

            #region Pictures

            model.DefaultPictureZoomEnabled = _mediaSettings.DefaultPictureZoomEnabled;
            //default picture
            var defaultPictureSize = isAssociatedProduct ?
                _mediaSettings.AssociatedProductPictureSize :
                _mediaSettings.ProductDetailsPictureSize;
            //prepare picture models
            var pictures = _pictureService.GetPicturesByProductId(product.Id);
            var defaultPicture = pictures.FirstOrDefault();
            var defaultPictureModel = new PictureModel
            {
                ImageUrl = _pictureService.GetPictureUrl(defaultPicture, defaultPictureSize, !isAssociatedProduct),
                FullSizeImageUrl = _pictureService.GetPictureUrl(defaultPicture, 0, !isAssociatedProduct),
                Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name),
                AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name),
            };
            //"title" attribute
            defaultPictureModel.Title = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.TitleAttribute)) ?
                defaultPicture.TitleAttribute :
                string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name);
            //"alt" attribute
            defaultPictureModel.AlternateText = (defaultPicture != null && !string.IsNullOrEmpty(defaultPicture.AltAttribute)) ?
                defaultPicture.AltAttribute :
                string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name);

            //all pictures
            var pictureModels = new List<PictureModel>();
            foreach (var picture in pictures)
            {
                var pictureModel = new PictureModel
                {
                    ImageUrl = _pictureService.GetPictureUrl(picture, _mediaSettings.ProductThumbPictureSizeOnProductDetailsPage),
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    Title = string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name),
                };
                //"title" attribute
                pictureModel.Title = !string.IsNullOrEmpty(picture.TitleAttribute) ?
                    picture.TitleAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageLinkTitleFormat.Details"), model.Name);
                //"alt" attribute
                pictureModel.AlternateText = !string.IsNullOrEmpty(picture.AltAttribute) ?
                    picture.AltAttribute :
                    string.Format(_localizationService.GetResource("Media.Product.ImageAlternateTextFormat.Details"), model.Name);

                pictureModels.Add(pictureModel);
            }

            model.DefaultPictureModel = defaultPictureModel;
            model.PictureModels = pictureModels;

            #endregion

            #region Product price

            model.ProductPrice.ProductId = product.Id;
            if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.ProductPrice.HidePrices = false;
                if (product.CustomerEntersPrice)
                {
                    model.ProductPrice.CustomerEntersPrice = true;
                }
                else
                {
                    if (product.CallForPrice)
                    {
                        model.ProductPrice.CallForPrice = true;
                    }
                    else
                    {
                        decimal taxRate;
                        decimal oldPriceBase = _taxService.GetProductPrice(product, product.OldPrice, out taxRate);
                        decimal finalPriceWithoutDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: false), out taxRate);
                        decimal finalPriceWithDiscountBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, includeDiscounts: true), out taxRate);

                        decimal oldPrice = _currencyService.ConvertFromPrimaryStoreCurrency(oldPriceBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithoutDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithoutDiscountBase, _workContext.WorkingCurrency);
                        decimal finalPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(finalPriceWithDiscountBase, _workContext.WorkingCurrency);

                        if (finalPriceWithoutDiscountBase != oldPriceBase && oldPriceBase > decimal.Zero)
                            model.ProductPrice.OldPrice = _priceFormatter.FormatPrice(oldPrice);

                        model.ProductPrice.Price = _priceFormatter.FormatPrice(finalPriceWithoutDiscount);

                        if (finalPriceWithoutDiscountBase != finalPriceWithDiscountBase)
                            model.ProductPrice.PriceWithDiscount = _priceFormatter.FormatPrice(finalPriceWithDiscount);

                        model.ProductPrice.PriceValue = finalPriceWithoutDiscount;
                        model.ProductPrice.PriceWithDiscountValue = finalPriceWithDiscount;

                        //property for German market
                        //we display tax/shipping info only with "shipping enabled" for this product
                        //we also ensure this it's not free shipping
                        model.ProductPrice.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoProductDetailsPage
                            && product.IsShipEnabled &&
                            !product.IsFreeShipping;

                        //PAngV baseprice (used in Germany)
                        model.ProductPrice.BasePricePAngV = product.FormatBasePrice(finalPriceWithDiscountBase,
                            _localizationService, _measureService, _currencyService, _workContext, _priceFormatter);

                        //currency code
                        model.ProductPrice.CurrencyCode = _workContext.WorkingCurrency.CurrencyCode;

                        //rental
                        if (product.IsRental)
                        {
                            model.ProductPrice.IsRental = true;
                            var priceStr = _priceFormatter.FormatPrice(finalPriceWithDiscount);
                            model.ProductPrice.RentalPrice = _priceFormatter.FormatRentalProductPeriod(product, priceStr);
                        }
                    }
                }
            }
            else
            {
                model.ProductPrice.HidePrices = true;
                model.ProductPrice.OldPrice = null;
                model.ProductPrice.Price = null;
            }
            #endregion

            #region 'Add to cart' model

            model.AddToCart.ProductId = product.Id;
            model.AddToCart.UpdatedShoppingCartItemId = updatecartitem != null ? updatecartitem.Id : 0;

            //quantity
            model.AddToCart.EnteredQuantity = updatecartitem != null ? updatecartitem.Quantity : product.OrderMinimumQuantity;

            //'add to cart', 'add to wishlist' buttons
            model.AddToCart.DisableBuyButton = product.DisableBuyButton || !_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.AddToCart.DisableWishlistButton = product.DisableWishlistButton || !_permissionService.Authorize(StandardPermissionProvider.EnableWishlist);
            if (!_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.AddToCart.DisableBuyButton = true;
                model.AddToCart.DisableWishlistButton = true;
            }
            //pre-order
            if (product.AvailableForPreOrder)
            {
                model.AddToCart.AvailableForPreOrder = !product.PreOrderAvailabilityStartDateTimeUtc.HasValue ||
                    product.PreOrderAvailabilityStartDateTimeUtc.Value >= DateTime.UtcNow;
                model.AddToCart.PreOrderAvailabilityStartDateTimeUtc = product.PreOrderAvailabilityStartDateTimeUtc;
            }
            //rental
            model.AddToCart.IsRental = product.IsRental;

            //customer entered price
            model.AddToCart.CustomerEntersPrice = product.CustomerEntersPrice;
            if (model.AddToCart.CustomerEntersPrice)
            {
                decimal minimumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MinimumCustomerEnteredPrice, _workContext.WorkingCurrency);
                decimal maximumCustomerEnteredPrice = _currencyService.ConvertFromPrimaryStoreCurrency(product.MaximumCustomerEnteredPrice, _workContext.WorkingCurrency);

                model.AddToCart.CustomerEnteredPrice = updatecartitem != null ? updatecartitem.CustomerEnteredPrice : minimumCustomerEnteredPrice;
                model.AddToCart.CustomerEnteredPriceRange = string.Format(_localizationService.GetResource("Products.EnterProductPrice.Range"),
                    _priceFormatter.FormatPrice(minimumCustomerEnteredPrice, false, false),
                    _priceFormatter.FormatPrice(maximumCustomerEnteredPrice, false, false));
            }
            //allowed quantities
            var allowedQuantities = product.ParseAllowedQuantities();
            foreach (var qty in allowedQuantities)
            {
                model.AddToCart.AllowedQuantities.Add(new SelectListItem
                {
                    Text = qty.ToString(),
                    Value = qty.ToString(),
                    Selected = updatecartitem != null && updatecartitem.Quantity == qty
                });
            }

            #endregion

            #region Gift card

            model.GiftCard.IsGiftCard = product.IsGiftCard;
            if (model.GiftCard.IsGiftCard)
            {
                model.GiftCard.GiftCardType = product.GiftCardType;

                if (updatecartitem == null)
                {
                    model.GiftCard.SenderName = _workContext.CurrentCustomer.GetFullName();
                    model.GiftCard.SenderEmail = _workContext.CurrentCustomer.Email;
                }
                else
                {
                    string giftCardRecipientName, giftCardRecipientEmail, giftCardSenderName, giftCardSenderEmail, giftCardMessage;
                    _productAttributeParser.GetGiftCardAttribute(updatecartitem.AttributesXml,
                        out giftCardRecipientName, out giftCardRecipientEmail,
                        out giftCardSenderName, out giftCardSenderEmail, out giftCardMessage);

                    model.GiftCard.RecipientName = giftCardRecipientName;
                    model.GiftCard.RecipientEmail = giftCardRecipientEmail;
                    model.GiftCard.SenderName = giftCardSenderName;
                    model.GiftCard.SenderEmail = giftCardSenderEmail;
                    model.GiftCard.Message = giftCardMessage;
                }
            }

            #endregion

            #region Product attributes

            //performance optimization
            //We cache a value indicating whether a product has attributes
            IList<ProductAttributeMapping> productAttributeMapping = null;

            productAttributeMapping = _productAttributeService.GetProductAttributeMappingsByProductId(product.Id);

            if (productAttributeMapping == null)
            {
                productAttributeMapping = new List<ProductAttributeMapping>();
            }
            foreach (var attribute in productAttributeMapping)
            {
                var attributeModel = new ProductDetailsModel.ProductAttributeModel
                {
                    Id = attribute.Id,
                    ProductId = product.Id,
                    ProductAttributeId = attribute.ProductAttributeId,
                    Name = attribute.ProductAttribute.GetLocalized(x => x.Name),
                    Description = attribute.ProductAttribute.GetLocalized(x => x.Description),
                    TextPrompt = attribute.TextPrompt,
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = updatecartitem != null ? null : attribute.DefaultValue,
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _productAttributeService.GetProductAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new ProductDetailsModel.ProductAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb, //used with "Color squares" attribute type
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal taxRate;
                            decimal attributeValuePriceAdjustment = _priceCalculationService.GetProductAttributeValuePriceAdjustment(attributeValue);
                            decimal priceAdjustmentBase = _taxService.GetProductPrice(product, attributeValuePriceAdjustment, out taxRate);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                valueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment, false, false);
                            else if (priceAdjustmentBase < decimal.Zero)
                                valueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment, false, false);

                            valueModel.PriceAdjustmentValue = priceAdjustment;
                        }

                        //picture
                        if (attributeValue.PictureId > 0)
                        {
                            var valuePicture = _pictureService.GetPictureById(attributeValue.PictureId);
                            var pictureModel = new PictureModel();
                            if (valuePicture != null)
                            {

                                pictureModel.FullSizeImageUrl = _pictureService.GetPictureUrl(valuePicture);
                                pictureModel.ImageUrl = _pictureService.GetPictureUrl(valuePicture, defaultPictureSize);
                            }
                            valueModel.PictureModel = pictureModel;
                        }
                    }
                }

                //set already selected attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    switch (attribute.AttributeControlType)
                    {
                        case AttributeControlType.DropdownList:
                        case AttributeControlType.RadioList:
                        case AttributeControlType.Checkboxes:
                        case AttributeControlType.ColorSquares:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    //clear default selection
                                    foreach (var item in attributeModel.Values)
                                        item.IsPreSelected = false;

                                    //select new values
                                    var selectedValues = _productAttributeParser.ParseProductAttributeValues(updatecartitem.AttributesXml);
                                    foreach (var attributeValue in selectedValues)
                                        foreach (var item in attributeModel.Values)
                                            if (attributeValue.Id == item.Id)
                                                item.IsPreSelected = true;
                                }
                            }
                            break;
                        case AttributeControlType.ReadonlyCheckboxes:
                            {
                                //do nothing
                                //values are already pre-set
                            }
                            break;
                        case AttributeControlType.TextBox:
                        case AttributeControlType.MultilineTextbox:
                            {
                                if (!String.IsNullOrEmpty(updatecartitem.AttributesXml))
                                {
                                    var enteredText = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                    if (enteredText.Count > 0)
                                        attributeModel.DefaultValue = enteredText[0];
                                }
                            }
                            break;
                        case AttributeControlType.Datepicker:
                            {
                                //keep in mind my that the code below works only in the current culture
                                var selectedDateStr = _productAttributeParser.ParseValues(updatecartitem.AttributesXml, attribute.Id);
                                if (selectedDateStr.Count > 0)
                                {
                                    DateTime selectedDate;
                                    if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                           DateTimeStyles.None, out selectedDate))
                                    {
                                        //successfully parsed
                                        attributeModel.SelectedDay = selectedDate.Day;
                                        attributeModel.SelectedMonth = selectedDate.Month;
                                        attributeModel.SelectedYear = selectedDate.Year;
                                    }
                                }

                            }
                            break;
                        default:
                            break;
                    }
                }

                model.ProductAttributes.Add(attributeModel);
            }

            #endregion

            #region Product specifications

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                model.ProductSpecifications = PrepareProductSpecificationModel(product);
            }

            #endregion

            #region Product review overview

            model.ProductReviewOverview = new ProductReviewOverviewModel
            {
                ProductId = product.Id,
                RatingSum = product.ApprovedRatingSum,
                TotalReviews = product.ApprovedTotalReviews,
                AllowCustomerReviews = product.AllowCustomerReviews
            };

            #endregion

            #region Tier prices

            if (product.HasTierPrices && _permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
            {
                model.TierPrices = product.TierPrices
                    .OrderBy(x => x.Quantity)
                    .ToList()
                    .FilterByStore(_storeContext.CurrentStore.Id)
                    .FilterForCustomer(_workContext.CurrentCustomer)
                    .RemoveDuplicatedQuantities()
                    .Select(tierPrice =>
                    {
                        var m = new ProductDetailsModel.TierPriceModel
                        {
                            Quantity = tierPrice.Quantity,
                        };
                        decimal taxRate;
                        decimal priceBase = _taxService.GetProductPrice(product, _priceCalculationService.GetFinalPrice(product, _workContext.CurrentCustomer, decimal.Zero, _catalogSettings.DisplayTierPricesWithDiscounts, tierPrice.Quantity), out taxRate);
                        decimal price = _currencyService.ConvertFromPrimaryStoreCurrency(priceBase, _workContext.WorkingCurrency);
                        m.Price = _priceFormatter.FormatPrice(price, false, false);
                        return m;
                    })
                    .ToList();
            }

            #endregion

            #region Manufacturers

            //do not prepare this model for the associated products. any it's not used
            if (!isAssociatedProduct)
            {
                model.ProductManufacturers = _manufacturerService.GetProductManufacturersByProductId(product.Id).Select(x => (MToModel(x.Manufacturer))).ToList();
            }
            #endregion

            #region Rental products

            if (product.IsRental)
            {
                model.IsRental = true;
                //set already entered dates attributes (if we're going to update the existing shopping cart item)
                if (updatecartitem != null)
                {
                    model.RentalStartDate = updatecartitem.RentalStartDateUtc;
                    model.RentalEndDate = updatecartitem.RentalEndDateUtc;
                }
            }

            #endregion

            #region Associated products

            if (product.ProductType == ProductType.GroupedProduct)
            {
                //ensure no circular references
                if (!isAssociatedProduct)
                {
                    var associatedProducts = _productService.GetAssociatedProducts(product.Id, _storeContext.CurrentStore.Id);
                    foreach (var associatedProduct in associatedProducts)
                        model.AssociatedProducts.Add(PrepareProductDetailsPageModel(associatedProduct, null, true));
                }
            }

            #endregion

            return model;
        }

        /// <summary>
        /// Prepare shopping cart model
        /// </summary>
        /// <param name="model">Model instance</param>
        /// <param name="cart">Shopping cart</param>
        /// <param name="isEditable">A value indicating whether cart is editable</param>
        /// <param name="validateCheckoutAttributes">A value indicating whether we should validate checkout attributes when preparing the model</param>
        /// <param name="prepareEstimateShippingIfEnabled">A value indicating whether we should prepare "Estimate shipping" model</param>
        /// <param name="setEstimateShippingDefaultAddress">A value indicating whether we should prefill "Estimate shipping" model with the default customer address</param>
        /// <param name="prepareAndDisplayOrderReviewData">A value indicating whether we should prepare review data (such as billing/shipping address, payment or shipping data entered during checkout)</param>
        /// <returns>Model</returns>
        [NonAction]
        protected virtual void PrepareShoppingCartModel(ShoppingCartModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true,
            bool validateCheckoutAttributes = false,
            bool prepareEstimateShippingIfEnabled = true, bool setEstimateShippingDefaultAddress = true,
            bool prepareAndDisplayOrderReviewData = false)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.OnePageCheckoutEnabled = _orderSettings.OnePageCheckoutEnabled;

            if (cart.Count == 0)
                return;

            #region Simple properties

            model.IsEditable = isEditable;
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;
            var checkoutAttributesXml = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
            model.CheckoutAttributeInfo = _checkoutAttributeFormatter.FormatAttributes(checkoutAttributesXml, _workContext.CurrentCustomer);
            bool minOrderSubtotalAmountOk = _orderProcessingService.ValidateMinOrderSubtotalAmount(cart);
            if (!minOrderSubtotalAmountOk)
            {
                decimal minOrderSubtotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderSubtotalAmount, _workContext.WorkingCurrency);
                model.MinOrderSubtotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderSubtotalAmount"), _priceFormatter.FormatPrice(minOrderSubtotalAmount, true, false));
            }
            model.TermsOfServiceOnShoppingCartPage = _orderSettings.TermsOfServiceOnShoppingCartPage;
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoShoppingCart;

            //gift card and gift card boxes
            model.DiscountBox.Display = _shoppingCartSettings.ShowDiscountBox;
            var discountCouponCode = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.DiscountCouponCode);
            var discount = _discountService.GetDiscountByCouponCode(discountCouponCode);
            if (discount != null &&
                discount.RequiresCouponCode &&
                _discountService.ValidateDiscount(discount, _workContext.CurrentCustomer).IsValid)
                model.DiscountBox.CurrentCode = discount.CouponCode;
            model.GiftCardBox.Display = _shoppingCartSettings.ShowGiftCardBox;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, checkoutAttributesXml, validateCheckoutAttributes);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Checkout attributes

            var checkoutAttributes = _checkoutAttributeService.GetAllCheckoutAttributes(_storeContext.CurrentStore.Id, !cart.RequiresShipping());
            foreach (var attribute in checkoutAttributes)
            {
                var attributeModel = new ShoppingCartModel.CheckoutAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    TextPrompt = attribute.GetLocalized(x => x.TextPrompt),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                    DefaultValue = attribute.DefaultValue
                };
                if (!String.IsNullOrEmpty(attribute.ValidationFileAllowedExtensions))
                {
                    attributeModel.AllowedFileExtensions = attribute.ValidationFileAllowedExtensions
                        .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                        .ToList();
                }

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _checkoutAttributeService.GetCheckoutAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var attributeValueModel = new ShoppingCartModel.CheckoutAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            ColorSquaresRgb = attributeValue.ColorSquaresRgb,
                            IsPreSelected = attributeValue.IsPreSelected,
                        };
                        attributeModel.Values.Add(attributeValueModel);

                        //display price if allowed
                        if (_permissionService.Authorize(StandardPermissionProvider.DisplayPrices))
                        {
                            decimal priceAdjustmentBase = _taxService.GetCheckoutAttributePrice(attributeValue);
                            decimal priceAdjustment = _currencyService.ConvertFromPrimaryStoreCurrency(priceAdjustmentBase, _workContext.WorkingCurrency);
                            if (priceAdjustmentBase > decimal.Zero)
                                attributeValueModel.PriceAdjustment = "+" + _priceFormatter.FormatPrice(priceAdjustment);
                            else if (priceAdjustmentBase < decimal.Zero)
                                attributeValueModel.PriceAdjustment = "-" + _priceFormatter.FormatPrice(-priceAdjustment);
                        }
                    }
                }



                //set already selected attributes
                var selectedCheckoutAttributes = _workContext.CurrentCustomer.GetAttribute<string>(SystemCustomerAttributeNames.CheckoutAttributes, _genericAttributeService, _storeContext.CurrentStore.Id);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                    case AttributeControlType.ColorSquares:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _checkoutAttributeParser.ParseCheckoutAttributeValues(selectedCheckoutAttributes);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var enteredText = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.Datepicker:
                        {
                            //keep in mind my that the code below works only in the current culture
                            var selectedDateStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id);
                            if (selectedDateStr.Count > 0)
                            {
                                DateTime selectedDate;
                                if (DateTime.TryParseExact(selectedDateStr[0], "D", CultureInfo.CurrentCulture,
                                                       DateTimeStyles.None, out selectedDate))
                                {
                                    //successfully parsed
                                    attributeModel.SelectedDay = selectedDate.Day;
                                    attributeModel.SelectedMonth = selectedDate.Month;
                                    attributeModel.SelectedYear = selectedDate.Year;
                                }
                            }

                        }
                        break;
                    case AttributeControlType.FileUpload:
                        {
                            if (!String.IsNullOrEmpty(selectedCheckoutAttributes))
                            {
                                var downloadGuidStr = _checkoutAttributeParser.ParseValues(selectedCheckoutAttributes, attribute.Id).FirstOrDefault();
                                Guid downloadGuid;
                                Guid.TryParse(downloadGuidStr, out downloadGuid);
                                var download = _downloadService.GetDownloadByGuid(downloadGuid);
                                if (download != null)
                                    attributeModel.DefaultValue = download.DownloadGuid.ToString();
                            }
                        }
                        break;
                    default:
                        break;
                }

                model.CheckoutAttributes.Add(attributeModel);
            }

            #endregion

            #region Estimate shipping

            if (prepareEstimateShippingIfEnabled)
            {
                model.EstimateShipping.Enabled = cart.Count > 0 && cart.RequiresShipping() && _shippingSettings.EstimateShippingEnabled;
                if (model.EstimateShipping.Enabled)
                {
                    //countries
                    int? defaultEstimateCountryId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.CountryId : model.EstimateShipping.CountryId;
                    model.EstimateShipping.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                    foreach (var c in _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id))
                        model.EstimateShipping.AvailableCountries.Add(new SelectListItem
                        {
                            Text = c.GetLocalized(x => x.Name),
                            Value = c.Id.ToString(),
                            Selected = c.Id == defaultEstimateCountryId
                        });
                    //states
                    int? defaultEstimateStateId = (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null) ? _workContext.CurrentCustomer.ShippingAddress.StateProvinceId : model.EstimateShipping.StateProvinceId;
                    var states = defaultEstimateCountryId.HasValue ? _stateProvinceService.GetStateProvincesByCountryId(defaultEstimateCountryId.Value, _workContext.WorkingLanguage.Id).ToList() : new List<StateProvince>();
                    if (states.Count > 0)
                        foreach (var s in states)
                            model.EstimateShipping.AvailableStates.Add(new SelectListItem
                            {
                                Text = s.GetLocalized(x => x.Name),
                                Value = s.Id.ToString(),
                                Selected = s.Id == defaultEstimateStateId
                            });
                    else
                        model.EstimateShipping.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.OtherNonUS"), Value = "0" });

                    if (setEstimateShippingDefaultAddress && _workContext.CurrentCustomer.ShippingAddress != null)
                        model.EstimateShipping.ZipPostalCode = _workContext.CurrentCustomer.ShippingAddress.ZipPostalCode;
                }
            }

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new ShoppingCartModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allow editing?
                //1. setting enabled?
                //2. simple product?
                //3. has attribute or gift card?
                //4. visible individually?
                cartItemModel.AllowItemEditing = _shoppingCartSettings.AllowCartItemEditing &&
                    sci.Product.ProductType == ProductType.SimpleProduct &&
                    (!String.IsNullOrEmpty(cartItemModel.AttributeInfo) || sci.Product.IsGiftCard) &&
                    sci.Product.VisibleIndividually;

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }

                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                   // Discount scDiscount;
                    List<Discount> scDiscounts;  // change 3.8
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscounts != null)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion

            #region Button payment methods

            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Button)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                string actionName;
                string controllerName;
                RouteValueDictionary routeValues;
                pm.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);

                model.ButtonPaymentMethodActionNames.Add(actionName);
                model.ButtonPaymentMethodControllerNames.Add(controllerName);
                model.ButtonPaymentMethodRouteValues.Add(routeValues);
            }

            #endregion

            #region Order review data

            if (prepareAndDisplayOrderReviewData)
            {
                model.OrderReviewData.Display = true;

                //billing info
                var billingAddress = _workContext.CurrentCustomer.BillingAddress;
                if (billingAddress != null)
                    model.OrderReviewData.BillingAddress.PrepareModelApi(
                        address: billingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);

                //shipping info
                if (cart.RequiresShipping())
                {
                    model.OrderReviewData.IsShippable = true;

                    if (_shippingSettings.AllowPickUpInStore)
                    {
                        model.OrderReviewData.SelectedPickUpInStore = _workContext.CurrentCustomer.GetAttribute<bool>(SystemCustomerAttributeNames.SelectedPickupPoint, _storeContext.CurrentStore.Id);
                    }

                    if (!model.OrderReviewData.SelectedPickUpInStore)
                    {
                        var shippingAddress = _workContext.CurrentCustomer.ShippingAddress;
                        if (shippingAddress != null)
                        {
                            model.OrderReviewData.ShippingAddress.PrepareModelApi(
                                address: shippingAddress,
                                excludeProperties: false,
                                addressSettings: _addressSettings,
                                addressAttributeFormatter: _addressAttributeFormatter);
                        }
                    }


                    //selected shipping method
                    var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                    if (shippingOption != null)
                        model.OrderReviewData.ShippingMethod = shippingOption.Name;
                }
                //payment info
                var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                model.OrderReviewData.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : "";

                //custom values
                var processPaymentRequest = _httpContext.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest != null)
                {
                    model.OrderReviewData.CustomValues = processPaymentRequest.CustomValues;
                }
            }
            #endregion
        }

        [NonAction]
        protected virtual OrderTotalsModel PrepareOrderTotalsModel(IList<ShoppingCartItem> cart, bool isEditable)
        {
            var model = new OrderTotalsModel();
            model.IsEditable = isEditable;

            if (cart.Count > 0)
            {
                //subtotal
                decimal orderSubTotalDiscountAmountBase;
               // Discount orderSubTotalAppliedDiscount;
                List<Discount> orderSubTotalAppliedDiscounts; //change 3.8
                decimal subTotalWithoutDiscountBase;
                decimal subTotalWithDiscountBase;
                var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal;
                _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax,
                    out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,
                    out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                decimal subtotalBase = subTotalWithoutDiscountBase;
                decimal subtotal = _currencyService.ConvertFromPrimaryStoreCurrency(subtotalBase, _workContext.WorkingCurrency);
                model.SubTotal = _priceFormatter.FormatPrice(subtotal, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);

                if (orderSubTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderSubTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderSubTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.SubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountAmount, true, _workContext.WorkingCurrency, _workContext.WorkingLanguage, subTotalIncludingTax);
                    //model.AllowRemovingSubTotalDiscount = orderSubTotalAppliedDiscount != null &&
                    //                                      orderSubTotalAppliedDiscount.RequiresCouponCode &&
                    //                                      !String.IsNullOrEmpty(orderSubTotalAppliedDiscount.CouponCode) &&
                    //                                      model.IsEditable;
                    model.AllowRemovingSubTotalDiscount = model.IsEditable &&
                       orderSubTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode));  //change 3.8

                }


                //shipping info
                model.RequiresShipping = cart.RequiresShipping();
                if (model.RequiresShipping)
                {
                    decimal? shoppingCartShippingBase = _orderTotalCalculationService.GetShoppingCartShippingTotal(cart);
                    if (shoppingCartShippingBase.HasValue)
                    {
                        decimal shoppingCartShipping = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartShippingBase.Value, _workContext.WorkingCurrency);
                        model.Shipping = _priceFormatter.FormatShippingPrice(shoppingCartShipping, true);

                        //selected shipping method
                        var shippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                        if (shippingOption != null)
                            model.SelectedShippingMethod = shippingOption.Name;
                    }
                }

                //payment method fee
                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod, _storeContext.CurrentStore.Id);
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, paymentMethodSystemName);
                decimal paymentMethodAdditionalFeeWithTaxBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                if (paymentMethodAdditionalFeeWithTaxBase > decimal.Zero)
                {
                    decimal paymentMethodAdditionalFeeWithTax = _currencyService.ConvertFromPrimaryStoreCurrency(paymentMethodAdditionalFeeWithTaxBase, _workContext.WorkingCurrency);
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeWithTax, true);
                }

                //tax
                bool displayTax = true;
                bool displayTaxRates = true;
                if (_taxSettings.HideTaxInOrderSummary && _workContext.TaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    SortedDictionary<decimal, decimal> taxRates;
                    decimal shoppingCartTaxBase = _orderTotalCalculationService.GetTaxTotal(cart, out taxRates);
                    decimal shoppingCartTax = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTaxBase, _workContext.WorkingCurrency);

                    if (shoppingCartTaxBase == 0 && _taxSettings.HideZeroTax)
                    {
                        displayTax = false;
                        displayTaxRates = false;
                    }
                    else
                    {
                        displayTaxRates = _taxSettings.DisplayTaxRates && taxRates.Count > 0;
                        displayTax = !displayTaxRates;

                        model.Tax = _priceFormatter.FormatPrice(shoppingCartTax, true, false);
                        foreach (var tr in taxRates)
                        {
                            model.TaxRates.Add(new OrderTotalsModel.TaxRate
                            {
                                Rate = _priceFormatter.FormatTaxRate(tr.Key),
                                Value = _priceFormatter.FormatPrice(_currencyService.ConvertFromPrimaryStoreCurrency(tr.Value, _workContext.WorkingCurrency), true, false),
                            });
                        }
                    }
                }
                model.DisplayTaxRates = displayTaxRates;
                model.DisplayTax = displayTax;

                //total
                decimal orderTotalDiscountAmountBase;
              //  Discount orderTotalAppliedDiscount;
                List<Discount> orderTotalAppliedDiscounts; //change 3.8
                List<AppliedGiftCard> appliedGiftCards;
                int redeemedRewardPoints;
                decimal redeemedRewardPointsAmount;
                decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart,
                    out orderTotalDiscountAmountBase, out orderTotalAppliedDiscounts,
                    out appliedGiftCards, out redeemedRewardPoints, out redeemedRewardPointsAmount);
                if (shoppingCartTotalBase.HasValue)
                {
                    decimal shoppingCartTotal = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartTotalBase.Value, _workContext.WorkingCurrency);
                    model.OrderTotal = _priceFormatter.FormatPrice(shoppingCartTotal, true, false);
                }

                //discount
                if (orderTotalDiscountAmountBase > decimal.Zero)
                {
                    decimal orderTotalDiscountAmount = _currencyService.ConvertFromPrimaryStoreCurrency(orderTotalDiscountAmountBase, _workContext.WorkingCurrency);
                    model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderTotalDiscountAmount, true, false);
                    //model.AllowRemovingOrderTotalDiscount = orderTotalAppliedDiscount != null &&
                    //    orderTotalAppliedDiscount.RequiresCouponCode &&
                    //    !String.IsNullOrEmpty(orderTotalAppliedDiscount.CouponCode) &&
                    //    model.IsEditable;
                    model.AllowRemovingOrderTotalDiscount = model.IsEditable &&
                        orderTotalAppliedDiscounts.Any(d => d.RequiresCouponCode && !String.IsNullOrEmpty(d.CouponCode)); //change 3.8
                }

                //gift cards
                if (appliedGiftCards != null && appliedGiftCards.Count > 0)
                {
                    foreach (var appliedGiftCard in appliedGiftCards)
                    {
                        var gcModel = new OrderTotalsModel.GiftCard
                        {
                            Id = appliedGiftCard.GiftCard.Id,
                            CouponCode = appliedGiftCard.GiftCard.GiftCardCouponCode,
                        };
                        decimal amountCanBeUsed = _currencyService.ConvertFromPrimaryStoreCurrency(appliedGiftCard.AmountCanBeUsed, _workContext.WorkingCurrency);
                        gcModel.Amount = _priceFormatter.FormatPrice(-amountCanBeUsed, true, false);

                        decimal remainingAmountBase = appliedGiftCard.GiftCard.GetGiftCardRemainingAmount() - appliedGiftCard.AmountCanBeUsed;
                        decimal remainingAmount = _currencyService.ConvertFromPrimaryStoreCurrency(remainingAmountBase, _workContext.WorkingCurrency);
                        gcModel.Remaining = _priceFormatter.FormatPrice(remainingAmount, true, false);

                        model.GiftCards.Add(gcModel);
                    }
                }

                //reward points to be spent (redeemed)
                if (redeemedRewardPointsAmount > decimal.Zero)
                {
                    decimal redeemedRewardPointsAmountInCustomerCurrency = _currencyService.ConvertFromPrimaryStoreCurrency(redeemedRewardPointsAmount, _workContext.WorkingCurrency);
                    model.RedeemedRewardPoints = redeemedRewardPoints;
                    model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-redeemedRewardPointsAmountInCustomerCurrency, true, false);
                }

                //reward points to be earned
                if (_rewardPointsSettings.Enabled &&
                    _rewardPointsSettings.DisplayHowMuchWillBeEarned &&
                    shoppingCartTotalBase.HasValue)
                {
                    model.WillEarnRewardPoints = _orderTotalCalculationService
                        .CalculateRewardPoints(_workContext.CurrentCustomer, shoppingCartTotalBase.Value);
                }

            }

            return model;
        }

        [NonAction]
        protected virtual bool IsPaymentWorkflowRequired(IList<ShoppingCartItem> cart, bool ignoreRewardPoints = false)
        {
            bool result = true;

            //check whether order total equals zero
            decimal? shoppingCartTotalBase = _orderTotalCalculationService.GetShoppingCartTotal(cart, ignoreRewardPoints);
            if (shoppingCartTotalBase.HasValue && shoppingCartTotalBase.Value == decimal.Zero)
                result = false;
            return result;
        }

        [NonAction]
        protected virtual CheckoutBillingAddressModel PrepareBillingAddressModel(int? selectedCountryId = null,
            bool prePopulateNewAddressWithCustomerFields = false)
        {
            var model = new CheckoutBillingAddressModel();
            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                //allow billing
                .Where(a => a.Country == null || a.Country.AllowsBilling)
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModelApi(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    addressAttributeFormatter: _addressAttributeFormatter);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            model.NewAddress.PrepareModelApi(address:
                null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountriesForBilling(),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer);
            return model;
        }

        [NonAction]
        //protected virtual CheckoutShippingAddressModel PrepareShippingAddressModel(int? selectedCountryId = null,
        //    bool prePopulateNewAddressWithCustomerFields = false)
        //{
        //    var model = new CheckoutShippingAddressModel();
        //    //allow pickup in store?
        //    model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
        //    if (model.AllowPickUpInStore && _shippingSettings.PickUpInStoreFee > 0)
        //    {
        //        decimal shippingTotal = _shippingSettings.PickUpInStoreFee;
        //        decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
        //        decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
        //        model.PickUpInStoreFee = _priceFormatter.FormatShippingPrice(rate, true);
        //    }
        //    //existing addresses
        //    var addresses = _workContext.CurrentCustomer.Addresses
        //        //allow shipping
        //        .Where(a => a.Country == null || a.Country.AllowsShipping)
        //        //enabled for the current store
        //        .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
        //        .ToList();
        //    foreach (var address in addresses)
        //    {
        //        var addressModel = new AddressModel();
        //        addressModel.PrepareModelApi(
        //            address: address,
        //            excludeProperties: false,
        //            addressSettings: _addressSettings,
        //            addressAttributeFormatter: _addressAttributeFormatter);
        //        model.ExistingAddresses.Add(addressModel);
        //    }

        //    //new address
        //    model.NewAddress.CountryId = selectedCountryId;
        //    model.NewAddress.PrepareModelApi(
        //        address: null,
        //        excludeProperties: false,
        //        addressSettings: _addressSettings,
        //        localizationService: _localizationService,
        //        stateProvinceService: _stateProvinceService,
        //        addressAttributeService: _addressAttributeService,
        //        addressAttributeParser: _addressAttributeParser,
        //        loadCountries: () => _countryService.GetAllCountriesForShipping(),
        //        prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
        //        customer: _workContext.CurrentCustomer);
        //    return model;
        //}
        protected virtual CheckoutShippingAddressModel PrepareShippingAddressModel(int? selectedCountryId = null,
           bool prePopulateNewAddressWithCustomerFields = false, string overrideAttributesXml = "")
        {
            var model = new CheckoutShippingAddressModel();

            //allow pickup in store?
            model.AllowPickUpInStore = _shippingSettings.AllowPickUpInStore;
            if (model.AllowPickUpInStore)
            {
                model.DisplayPickupPointsOnMap = _shippingSettings.DisplayPickupPointsOnMap;
                model.GoogleMapsApiKey = _shippingSettings.GoogleMapsApiKey;
                var pickupPointProviders = _shippingService.LoadActivePickupPointProviders(_storeContext.CurrentStore.Id);
                if (pickupPointProviders.Any())
                {
                    var pickupPointsResponse = _shippingService.GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, null, _storeContext.CurrentStore.Id);
                    if (pickupPointsResponse.Success)
                        model.PickupPoints = pickupPointsResponse.PickupPoints.Select(x =>
                        {
                            var country = _countryService.GetCountryByTwoLetterIsoCode(x.CountryCode);
                            var pickupPointModel = new CheckoutPickupPointModel
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Description = x.Description,
                                ProviderSystemName = x.ProviderSystemName,
                                Address = x.Address,
                                City = x.City,
                                CountryName = country != null ? country.Name : string.Empty,
                                ZipPostalCode = x.ZipPostalCode,
                                Latitude = x.Latitude,
                                Longitude = x.Longitude,
                                OpeningHours = x.OpeningHours
                            };
                            if (x.PickupFee > 0)
                            {
                                var amount = _taxService.GetShippingPrice(x.PickupFee, _workContext.CurrentCustomer);
                                amount = _currencyService.ConvertFromPrimaryStoreCurrency(amount, _workContext.WorkingCurrency);
                                pickupPointModel.PickupFee = _priceFormatter.FormatShippingPrice(amount, true);
                            }

                            return pickupPointModel;
                        }).ToList();
                    else
                        foreach (var error in pickupPointsResponse.Errors)
                            model.Warnings.Add(error);
                }

                //only available pickup points
                if (!_shippingService.LoadActiveShippingRateComputationMethods(_storeContext.CurrentStore.Id).Any())
                {
                    if (!pickupPointProviders.Any())
                    {
                        model.Warnings.Add(_localizationService.GetResource("Checkout.ShippingIsNotAllowed"));
                        model.Warnings.Add(_localizationService.GetResource("Checkout.PickupPoints.NotAvailable"));
                    }
                    model.PickUpInStoreOnly = true;
                    model.PickUpInStore = true;
                    return model;
                }
            }

            //existing addresses
            var addresses = _workContext.CurrentCustomer.Addresses
                .Where(a => a.Country == null ||
                    (//published
                    a.Country.Published &&
                    //allow shipping
                    a.Country.AllowsShipping &&
                    //enabled for the current store
                    _storeMappingService.Authorize(a.Country)))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModelApi(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    addressAttributeFormatter: _addressAttributeFormatter);
                model.ExistingAddresses.Add(addressModel);
            }

            //new address
            model.NewAddress.CountryId = selectedCountryId;
            model.NewAddress.PrepareModelApi(
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountriesForShipping(_workContext.WorkingLanguage.Id),
                prePopulateWithCustomerFields: prePopulateNewAddressWithCustomerFields,
                customer: _workContext.CurrentCustomer);//,
                //overrideAttributesXml: overrideAttributesXml);

            return model;
        }

        [NonAction]
        protected virtual CheckoutShippingMethodModel PrepareShippingMethodModel(IList<ShoppingCartItem> cart, Address shippingAddress)
        {
            var model = new CheckoutShippingMethodModel();

            var getShippingOptionResponse = _shippingService
                .GetShippingOptions(cart, shippingAddress,
                "", _storeContext.CurrentStore.Id);
            if (getShippingOptionResponse.Success)
            {
                //performance optimization. cache returned shipping options.
                //we'll use them later (after a customer has selected an option).
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                                                       SystemCustomerAttributeNames.OfferedShippingOptions,
                                                       getShippingOptionResponse.ShippingOptions,
                                                       _storeContext.CurrentStore.Id);

                foreach (var shippingOption in getShippingOptionResponse.ShippingOptions)
                {
                    var soModel = new CheckoutShippingMethodModel.ShippingMethodModel
                    {
                        Name = shippingOption.Name,
                        Description = shippingOption.Description,
                        ShippingRateComputationMethodSystemName = shippingOption.ShippingRateComputationMethodSystemName,
                        ShippingOption = shippingOption,
                    };

                    //adjust rate
                   // Discount appliedDiscount;
                    List<Discount> appliedDiscounts; // Change 3.8
                    var shippingTotal = _orderTotalCalculationService.AdjustShippingRate(
                        shippingOption.Rate, cart, out appliedDiscounts);

                    decimal rateBase = _taxService.GetShippingPrice(shippingTotal, _workContext.CurrentCustomer);
                    decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase,
                                                                                    _workContext.WorkingCurrency);
                    soModel.Fee = _priceFormatter.FormatShippingPrice(rate, true);

                    model.ShippingMethods.Add(soModel);
                }

                //find a selected (previously) shipping method
                var selectedShippingOption = _workContext.CurrentCustomer.GetAttribute<ShippingOption>(
                        SystemCustomerAttributeNames.SelectedShippingOption, _storeContext.CurrentStore.Id);
                if (selectedShippingOption != null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.ToList()
                        .Find(so =>
                           !String.IsNullOrEmpty(so.Name) &&
                           so.Name.Equals(selectedShippingOption.Name, StringComparison.InvariantCultureIgnoreCase) &&
                           !String.IsNullOrEmpty(so.ShippingRateComputationMethodSystemName) &&
                           so.ShippingRateComputationMethodSystemName.Equals(selectedShippingOption.ShippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }
                //if no option has been selected, let's do it for the first one
                if (model.ShippingMethods.FirstOrDefault(so => so.Selected) == null)
                {
                    var shippingOptionToSelect = model.ShippingMethods.FirstOrDefault();
                    if (shippingOptionToSelect != null)
                    {
                        shippingOptionToSelect.Selected = true;
                    }
                }

                //notify about shipping from multiple locations
                if (_shippingSettings.NotifyCustomerAboutShippingFromMultipleLocations)
                {
                    model.NotifyCustomerAboutShippingFromMultipleLocations = getShippingOptionResponse.ShippingFromMultipleLocations;
                }
            }
            else
            {
                foreach (var error in getShippingOptionResponse.Errors)
                    model.Warnings.Add(error);
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutPaymentMethodModel PreparePaymentMethodModel(IList<ShoppingCartItem> cart, int filterByCountryId)
        {
            var model = new CheckoutPaymentMethodModel();

            //reward points
            if (_rewardPointsSettings.Enabled && !cart.IsRecurring())
            {
                int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id);
                decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
                decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
                if (rewardPointsAmount > decimal.Zero &&
                    _orderTotalCalculationService.CheckMinimumRewardPointsToUseRequirement(rewardPointsBalance))
                {
                    model.DisplayRewardPoints = true;
                    model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
                    model.RewardPointsBalance = rewardPointsBalance;
                }
            }

            //filter by country
            var paymentMethods = _paymentService
                .LoadActivePaymentMethods(_workContext.CurrentCustomer.Id, _storeContext.CurrentStore.Id, filterByCountryId)
                .Where(pm => pm.PaymentMethodType == PaymentMethodType.Standard || pm.PaymentMethodType == PaymentMethodType.Redirection)
                .Where(pm => !pm.HidePaymentMethod(cart))
                .ToList();
            foreach (var pm in paymentMethods)
            {
                if (cart.IsRecurring() && pm.RecurringPaymentType == RecurringPaymentType.NotSupported)
                    continue;

                var pmModel = new CheckoutPaymentMethodModel.PaymentMethodModel
                {
                    Name = pm.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id),
                    PaymentMethodSystemName = pm.PluginDescriptor.SystemName,
                    LogoUrl = pm.PluginDescriptor.GetLogoUrl(_webHelper)
                };
                //payment method additional fee
                decimal paymentMethodAdditionalFee = _paymentService.GetAdditionalHandlingFee(cart, pm.PluginDescriptor.SystemName);
                decimal rateBase = _taxService.GetPaymentMethodAdditionalFee(paymentMethodAdditionalFee, _workContext.CurrentCustomer);
                decimal rate = _currencyService.ConvertFromPrimaryStoreCurrency(rateBase, _workContext.WorkingCurrency);
                if (rate > decimal.Zero)
                    pmModel.Fee = _priceFormatter.FormatPaymentMethodAdditionalFee(rate, true);

                model.PaymentMethods.Add(pmModel);
            }

            //find a selected (previously) payment method
            var selectedPaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                SystemCustomerAttributeNames.SelectedPaymentMethod,
                _genericAttributeService, _storeContext.CurrentStore.Id);
            if (!String.IsNullOrEmpty(selectedPaymentMethodSystemName))
            {
                var paymentMethodToSelect = model.PaymentMethods.ToList()
                    .Find(pm => pm.PaymentMethodSystemName.Equals(selectedPaymentMethodSystemName, StringComparison.InvariantCultureIgnoreCase));
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }
            //if no option has been selected, let's do it for the first one
            if (model.PaymentMethods.FirstOrDefault(so => so.Selected) == null)
            {
                var paymentMethodToSelect = model.PaymentMethods.FirstOrDefault();
                if (paymentMethodToSelect != null)
                    paymentMethodToSelect.Selected = true;
            }

            return model;
        }

        [NonAction]
        protected virtual CheckoutPaymentInfoModel PreparePaymentInfoModel(IPaymentMethod paymentMethod)
        {
            var model = new CheckoutPaymentInfoModel();
            string actionName;
            string controllerName;
            RouteValueDictionary routeValues;
            paymentMethod.GetPaymentInfoRoute(out actionName, out controllerName, out routeValues);
            model.PaymentInfoActionName = actionName;
            model.PaymentInfoControllerName = controllerName;
            model.PaymentInfoRouteValues = routeValues;
            model.DisplayOrderTotals = _orderSettings.OnePageCheckoutDisplayOrderTotalsOnPaymentInfoTab;
            return model;
        }

        [NonAction]
        protected virtual CheckoutConfirmModel PrepareConfirmOrderModel(IList<ShoppingCartItem> cart)
        {
            var model = new CheckoutConfirmModel();
            //terms of service
            model.TermsOfServiceOnOrderConfirmPage = _orderSettings.TermsOfServiceOnOrderConfirmPage;
            //min order amount validation
            bool minOrderTotalAmountOk = _orderProcessingService.ValidateMinOrderTotalAmount(cart);
            if (!minOrderTotalAmountOk)
            {
                decimal minOrderTotalAmount = _currencyService.ConvertFromPrimaryStoreCurrency(_orderSettings.MinOrderTotalAmount, _workContext.WorkingCurrency);
                model.MinOrderTotalWarning = string.Format(_localizationService.GetResource("Checkout.MinOrderTotalAmount"), _priceFormatter.FormatPrice(minOrderTotalAmount, true, false));
            }
            return model;
        }

        [NonAction]
        protected virtual bool IsMinimumOrderPlacementIntervalValid(Customer customer)
        {
            //prevent 2 orders being placed within an X seconds time frame
            if (_orderSettings.MinimumOrderPlacementInterval == 0)
                return true;

            var lastOrder = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                .FirstOrDefault();
            if (lastOrder == null)
                return true;

            var interval = DateTime.UtcNow - lastOrder.CreatedOnUtc;
            return interval.TotalSeconds > _orderSettings.MinimumOrderPlacementInterval;
        }

        [NonAction]
        protected virtual void PrepareWishlistModel(WishlistModel model,
            IList<ShoppingCartItem> cart, bool isEditable = true)
        {
            if (cart == null)
                throw new ArgumentNullException("cart");

            if (model == null)
                throw new ArgumentNullException("model");

            model.EmailWishlistEnabled = _shoppingCartSettings.EmailWishlistEnabled;
            model.IsEditable = isEditable;
            model.DisplayAddToCart = _permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart);
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoWishlist;

            if (cart.Count == 0)
                return;

            #region Simple properties

            var customer = cart.GetCustomer();
            model.CustomerGuid = customer.CustomerGuid;
            model.CustomerFullname = customer.GetFullName();
            model.ShowProductImages = _shoppingCartSettings.ShowProductImagesOnShoppingCart;
            model.ShowSku = _catalogSettings.ShowProductSku;

            //cart warnings
            var cartWarnings = _shoppingCartService.GetShoppingCartWarnings(cart, "", false);
            foreach (var warning in cartWarnings)
                model.Warnings.Add(warning);

            #endregion

            #region Cart items

            foreach (var sci in cart)
            {
                var cartItemModel = new WishlistModel.ShoppingCartItemModel
                {
                    Id = sci.Id,
                    Sku = sci.Product.FormatSku(sci.AttributesXml, _productAttributeParser),
                    ProductId = sci.Product.Id,
                    ProductName = sci.Product.GetLocalized(x => x.Name),
                    ProductSeName = sci.Product.GetSeName(),
                    Quantity = sci.Quantity,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                };

                //allowed quantities
                var allowedQuantities = sci.Product.ParseAllowedQuantities();
                foreach (var qty in allowedQuantities)
                {
                    cartItemModel.AllowedQuantities.Add(new SelectListItem
                    {
                        Text = qty.ToString(),
                        Value = qty.ToString(),
                        Selected = sci.Quantity == qty
                    });
                }


                //recurring info
                if (sci.Product.IsRecurring)
                    cartItemModel.RecurringInfo = string.Format(_localizationService.GetResource("ShoppingCart.RecurringPeriod"), sci.Product.RecurringCycleLength, sci.Product.RecurringCyclePeriod.GetLocalizedEnum(_localizationService, _workContext));

                //rental info
                if (sci.Product.IsRental)
                {
                    var rentalStartDate = sci.RentalStartDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = sci.RentalEndDateUtc.HasValue ? sci.Product.FormatRentalDate(sci.RentalEndDateUtc.Value) : "";
                    cartItemModel.RentalInfo = string.Format(_localizationService.GetResource("ShoppingCart.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }

                //unit prices
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.UnitPrice = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    decimal taxRate;
                    decimal shoppingCartUnitPriceWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate);
                    decimal shoppingCartUnitPriceWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartUnitPriceWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.UnitPrice = _priceFormatter.FormatPrice(shoppingCartUnitPriceWithDiscount);
                }
                //subtotal, discount
                if (sci.Product.CallForPrice)
                {
                    cartItemModel.SubTotal = _localizationService.GetResource("Products.CallForPrice");
                }
                else
                {
                    //sub total
                   // Discount scDiscount;
                    List<Discount> scDiscounts;  // change 3.8
                    decimal shoppingCartItemDiscountBase;
                    decimal taxRate;
                    decimal shoppingCartItemSubTotalWithDiscountBase = _taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci, true, out shoppingCartItemDiscountBase, out scDiscounts), out taxRate);
                    decimal shoppingCartItemSubTotalWithDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemSubTotalWithDiscountBase, _workContext.WorkingCurrency);
                    cartItemModel.SubTotal = _priceFormatter.FormatPrice(shoppingCartItemSubTotalWithDiscount);

                    //display an applied discount amount
                    if (scDiscounts != null)
                    {
                        shoppingCartItemDiscountBase = _taxService.GetProductPrice(sci.Product, shoppingCartItemDiscountBase, out taxRate);
                        if (shoppingCartItemDiscountBase > decimal.Zero)
                        {
                            decimal shoppingCartItemDiscount = _currencyService.ConvertFromPrimaryStoreCurrency(shoppingCartItemDiscountBase, _workContext.WorkingCurrency);
                            cartItemModel.Discount = _priceFormatter.FormatPrice(shoppingCartItemDiscount);
                        }
                    }
                }

                //picture
                if (_shoppingCartSettings.ShowProductImagesOnShoppingCart)
                {
                    cartItemModel.Picture = PrepareCartItemPictureModel(sci,
                        _mediaSettings.CartThumbPictureSize, true, cartItemModel.ProductName);
                }

                //item warnings
                var itemWarnings = _shoppingCartService.GetShoppingCartItemWarnings(
                    _workContext.CurrentCustomer,
                    sci.ShoppingCartType,
                    sci.Product,
                    sci.StoreId,
                    sci.AttributesXml,
                    sci.CustomerEnteredPrice,
                    sci.RentalStartDateUtc,
                    sci.RentalEndDateUtc,
                    sci.Quantity,
                    false);
                foreach (var warning in itemWarnings)
                    cartItemModel.Warnings.Add(warning);

                model.Items.Add(cartItemModel);
            }

            #endregion
        }

        [NonAction]
        protected virtual void PrepareCustomerInfoModel(CustomerInfoModel model, Customer customer,
           bool excludeProperties, string overrideCustomCustomerAttributesXml = "")
        {
            if (model == null)
                throw new ArgumentNullException("model");

            if (customer == null)
                throw new ArgumentNullException("customer");

            model.AllowCustomersToSetTimeZone = _dateTimeSettings.AllowCustomersToSetTimeZone;
            foreach (var tzi in _dateTimeHelper.GetSystemTimeZones())
                model.AvailableTimeZones.Add(new SelectListItem { Text = tzi.DisplayName, Value = tzi.Id, Selected = (excludeProperties ? tzi.Id == model.TimeZoneId : tzi.Id == _dateTimeHelper.CurrentTimeZone.Id) });

            if (!excludeProperties)
            {
                model.VatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber);
                model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                model.Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender);
                var dateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth);
                if (dateOfBirth.HasValue)
                {
                    model.DateOfBirthDay = dateOfBirth.Value.Day;
                    model.DateOfBirthMonth = dateOfBirth.Value.Month;
                    model.DateOfBirthYear = dateOfBirth.Value.Year;
                }
                model.Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company);
                model.StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress);
                model.StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2);
                model.ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode);
                model.City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City);
                model.CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId);
                model.StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId);
                model.Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone);
                model.Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax);

                //newsletter
                var newsletter = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(customer.Email, _storeContext.CurrentStore.Id);
                model.Newsletter = newsletter != null && newsletter.Active;

                model.Signature = customer.GetAttribute<string>(SystemCustomerAttributeNames.Signature);

                model.Email = customer.Email;
                model.Username = customer.Username;
            }
            else
            {
                if (_customerSettings.UsernamesEnabled && !_customerSettings.AllowUsersToChangeUsernames)
                    model.Username = customer.Username;
            }

            //countries and states
            if (_customerSettings.CountryEnabled)
            {
                model.AvailableCountries.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectCountry"), Value = "0" });
                foreach (var c in _countryService.GetAllCountries())
                {
                    model.AvailableCountries.Add(new SelectListItem
                    {
                        Text = c.GetLocalized(x => x.Name),
                        Value = c.Id.ToString(),
                        Selected = c.Id == model.CountryId
                    });
                }

                if (_customerSettings.StateProvinceEnabled)
                {
                    //states
                    var states = _stateProvinceService.GetStateProvincesByCountryId(model.CountryId).ToList();
                    if (states.Count > 0)
                    {
                        model.AvailableStates.Add(new SelectListItem { Text = _localizationService.GetResource("Address.SelectState"), Value = "0" });

                        foreach (var s in states)
                        {
                            model.AvailableStates.Add(new SelectListItem { Text = s.GetLocalized(x => x.Name), Value = s.Id.ToString(), Selected = (s.Id == model.StateProvinceId) });
                        }
                    }
                    else
                    {
                        bool anyCountrySelected = model.AvailableCountries.Any(x => x.Selected);

                        model.AvailableStates.Add(new SelectListItem
                        {
                            Text = _localizationService.GetResource(anyCountrySelected ? "Address.OtherNonUS" : "Address.SelectState"),
                            Value = "0"
                        });
                    }

                }
            }
            model.DisplayVatNumber = _taxSettings.EuVatEnabled;
            model.VatNumberStatusNote = ((VatNumberStatus)customer.GetAttribute<int>(SystemCustomerAttributeNames.VatNumberStatusId))
                .GetLocalizedEnum(_localizationService, _workContext);
            model.GenderEnabled = _customerSettings.GenderEnabled;
            model.DateOfBirthEnabled = _customerSettings.DateOfBirthEnabled;
            model.DateOfBirthRequired = _customerSettings.DateOfBirthRequired;
            model.CompanyEnabled = _customerSettings.CompanyEnabled;
            model.CompanyRequired = _customerSettings.CompanyRequired;
            model.StreetAddressEnabled = _customerSettings.StreetAddressEnabled;
            model.StreetAddressRequired = _customerSettings.StreetAddressRequired;
            model.StreetAddress2Enabled = _customerSettings.StreetAddress2Enabled;
            model.StreetAddress2Required = _customerSettings.StreetAddress2Required;
            model.ZipPostalCodeEnabled = _customerSettings.ZipPostalCodeEnabled;
            model.ZipPostalCodeRequired = _customerSettings.ZipPostalCodeRequired;
            model.CityEnabled = _customerSettings.CityEnabled;
            model.CityRequired = _customerSettings.CityRequired;
            model.CountryEnabled = _customerSettings.CountryEnabled;
            model.CountryRequired = _customerSettings.CountryRequired;
            model.StateProvinceEnabled = _customerSettings.StateProvinceEnabled;
            model.StateProvinceRequired = _customerSettings.StateProvinceRequired;
            model.PhoneEnabled = _customerSettings.PhoneEnabled;
            model.PhoneRequired = _customerSettings.PhoneRequired;
            model.FaxEnabled = _customerSettings.FaxEnabled;
            model.FaxRequired = _customerSettings.FaxRequired;
            model.NewsletterEnabled = _customerSettings.NewsletterEnabled;
            model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            model.AllowUsersToChangeUsernames = _customerSettings.AllowUsersToChangeUsernames;
            model.CheckUsernameAvailabilityEnabled = _customerSettings.CheckUsernameAvailabilityEnabled;
            model.SignatureEnabled = _forumSettings.ForumsEnabled && _forumSettings.SignaturesEnabled;

            //external authentication
            model.NumberOfExternalAuthenticationProviders = _openAuthenticationService
                .LoadActiveExternalAuthenticationMethods(_storeContext.CurrentStore.Id)
                .Count;
            foreach (var ear in _openAuthenticationService.GetExternalIdentifiersFor(customer))
            {
                var authMethod = _openAuthenticationService.LoadExternalAuthenticationMethodBySystemName(ear.ProviderSystemName);
                if (authMethod == null || !authMethod.IsMethodActive(_externalAuthenticationSettings))
                    continue;

                model.AssociatedExternalAuthRecords.Add(new CustomerInfoModel.AssociatedExternalAuthModel
                {
                    Id = ear.Id,
                    Email = ear.Email,
                    ExternalIdentifier = ear.ExternalIdentifier,
                    AuthMethodName = authMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id)
                });
            }

            //custom customer attributes
            var customAttributes = PrepareCustomCustomerAttributes(customer, overrideCustomCustomerAttributesXml);
            foreach (var customAttribute in customAttributes)
                model.CustomerAttributes.Add(customAttribute);
        }

        [NonAction]
        protected virtual IList<CustomerAttributeModel> PrepareCustomCustomerAttributes(Customer customer,
            string overrideAttributesXml = "")
        {
            if (customer == null)
                throw new ArgumentNullException("customer");

            var result = new List<CustomerAttributeModel>();

            var customerAttributes = _customerAttributeService.GetAllCustomerAttributes();
            foreach (var attribute in customerAttributes)
            {
                var attributeModel = new CustomerAttributeModel
                {
                    Id = attribute.Id,
                    Name = attribute.GetLocalized(x => x.Name),
                    IsRequired = attribute.IsRequired,
                    AttributeControlType = attribute.AttributeControlType,
                };

                if (attribute.ShouldHaveValues())
                {
                    //values
                    var attributeValues = _customerAttributeService.GetCustomerAttributeValues(attribute.Id);
                    foreach (var attributeValue in attributeValues)
                    {
                        var valueModel = new CustomerAttributeValueModel
                        {
                            Id = attributeValue.Id,
                            Name = attributeValue.GetLocalized(x => x.Name),
                            IsPreSelected = attributeValue.IsPreSelected
                        };
                        attributeModel.Values.Add(valueModel);
                    }
                }

                //set already selected attributes
                var selectedAttributesXml = !String.IsNullOrEmpty(overrideAttributesXml) ?
                    overrideAttributesXml :
                    customer.GetAttribute<string>(SystemCustomerAttributeNames.CustomCustomerAttributes, _genericAttributeService);
                switch (attribute.AttributeControlType)
                {
                    case AttributeControlType.DropdownList:
                    case AttributeControlType.RadioList:
                    case AttributeControlType.Checkboxes:
                        {
                            if (!String.IsNullOrEmpty(selectedAttributesXml))
                            {
                                //clear default selection
                                foreach (var item in attributeModel.Values)
                                    item.IsPreSelected = false;

                                //select new values
                                var selectedValues = _customerAttributeParser.ParseCustomerAttributeValues(selectedAttributesXml);
                                foreach (var attributeValue in selectedValues)
                                    foreach (var item in attributeModel.Values)
                                        if (attributeValue.Id == item.Id)
                                            item.IsPreSelected = true;
                            }
                        }
                        break;
                    case AttributeControlType.ReadonlyCheckboxes:
                        {
                            //do nothing
                            //values are already pre-set
                        }
                        break;
                    case AttributeControlType.TextBox:
                    case AttributeControlType.MultilineTextbox:
                        {
                            if (!String.IsNullOrEmpty(selectedAttributesXml))
                            {
                                var enteredText = _customerAttributeParser.ParseValues(selectedAttributesXml, attribute.Id);
                                if (enteredText.Count > 0)
                                    attributeModel.DefaultValue = enteredText[0];
                            }
                        }
                        break;
                    case AttributeControlType.ColorSquares:
                    case AttributeControlType.Datepicker:
                    case AttributeControlType.FileUpload:
                    default:
                        //not supported attribute control types
                        break;
                }

                result.Add(attributeModel);
            }


            return result;
        }

        [NonAction]
        protected virtual CustomerOrderListModel PrepareCustomerOrderListModel()
        {
            var model = new CustomerOrderListModel();
            var orders = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id);
            foreach (var order in orders)
            {
                var orderModel = new CustomerOrderListModel.OrderDetailsModel
                {
                    Id = order.Id,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc),
                    OrderStatusEnum = order.OrderStatus,
                    OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext),
                    PaymentStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext),
                    ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext),
                    IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order)
                };
                var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
                orderModel.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                model.Orders.Add(orderModel);
            }

            var recurringPayments = _orderService.SearchRecurringPayments(_storeContext.CurrentStore.Id,
                _workContext.CurrentCustomer.Id);
            foreach (var recurringPayment in recurringPayments)
            {
                var recurringPaymentModel = new CustomerOrderListModel.RecurringOrderModel
                {
                    Id = recurringPayment.Id,
                    StartDate = _dateTimeHelper.ConvertToUserTime(recurringPayment.StartDateUtc, DateTimeKind.Utc).ToString(),
                    CycleInfo = string.Format("{0} {1}", recurringPayment.CycleLength, recurringPayment.CyclePeriod.GetLocalizedEnum(_localizationService, _workContext)),
                    NextPayment = recurringPayment.NextPaymentDate.HasValue ? _dateTimeHelper.ConvertToUserTime(recurringPayment.NextPaymentDate.Value, DateTimeKind.Utc).ToString() : "",
                    TotalCycles = recurringPayment.TotalCycles,
                    CyclesRemaining = recurringPayment.CyclesRemaining,
                    InitialOrderId = recurringPayment.InitialOrder.Id,
                    CanCancel = _orderProcessingService.CanCancelRecurringPayment(_workContext.CurrentCustomer, recurringPayment),
                };

                model.RecurringOrders.Add(recurringPaymentModel);
            }

            return model;
        }

        [NonAction]
        protected virtual OrderDetailsModel PrepareOrderDetailsModel(Order order)
        {
            if (order == null)
                throw new ArgumentNullException("order");
            var model = new OrderDetailsModel();

            model.Id = order.Id;
            model.CreatedOn = _dateTimeHelper.ConvertToUserTime(order.CreatedOnUtc, DateTimeKind.Utc);
            model.OrderStatus = order.OrderStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.IsReOrderAllowed = _orderSettings.IsReOrderAllowed;
            model.IsReturnRequestAllowed = _orderProcessingService.IsReturnRequestAllowed(order);
            model.PdfInvoiceDisabled = _pdfSettings.DisablePdfInvoicesForPendingOrders && order.OrderStatus == OrderStatus.Pending;

            //shipping info
            model.ShippingStatus = order.ShippingStatus.GetLocalizedEnum(_localizationService, _workContext);
            if (order.ShippingStatus != ShippingStatus.ShippingNotRequired)
            {
                model.IsShippable = true;
                model.PickUpInStore = order.PickUpInStore;
                if (!order.PickUpInStore)
                {
                    model.ShippingAddress.PrepareModelApi(
                        address: order.ShippingAddress,
                        excludeProperties: false,
                        addressSettings: _addressSettings,
                        addressAttributeFormatter: _addressAttributeFormatter);
                }
                model.ShippingMethod = order.ShippingMethod;


                //shipments (only already shipped)
                var shipments = order.Shipments.Where(x => x.ShippedDateUtc.HasValue).OrderBy(x => x.CreatedOnUtc).ToList();
                foreach (var shipment in shipments)
                {
                    var shipmentModel = new OrderDetailsModel.ShipmentBriefModel
                    {
                        Id = shipment.Id,
                        TrackingNumber = shipment.TrackingNumber,
                    };
                    if (shipment.ShippedDateUtc.HasValue)
                        shipmentModel.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
                    if (shipment.DeliveryDateUtc.HasValue)
                        shipmentModel.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);
                    model.Shipments.Add(shipmentModel);
                }
            }


            //billing info
            model.BillingAddress.PrepareModelApi(
                address: order.BillingAddress,
                excludeProperties: false,
                addressSettings: _addressSettings,
                addressAttributeFormatter: _addressAttributeFormatter);

            //VAT number
            model.VatNumber = order.VatNumber;

            //payment method
            var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
            model.PaymentMethod = paymentMethod != null ? paymentMethod.GetLocalizedFriendlyName(_localizationService, _workContext.WorkingLanguage.Id) : order.PaymentMethodSystemName;
            model.PaymentMethodStatus = order.PaymentStatus.GetLocalizedEnum(_localizationService, _workContext);
            model.CanRePostProcessPayment = _paymentService.CanRePostProcessPayment(order);
            //custom values
            model.CustomValues = order.DeserializeCustomValues();

            //order subtotal
            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax && !_taxSettings.ForceTaxExclusionFromOrderSubtotal)
            {
                //including tax

                //order subtotal
                var orderSubtotalInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalInclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountInclTax, order.CurrencyRate);
                if (orderSubTotalDiscountInclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order subtotal
                var orderSubtotalExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubtotalExclTax, order.CurrencyRate);
                model.OrderSubtotal = _priceFormatter.FormatPrice(orderSubtotalExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //discount (applied to order subtotal)
                var orderSubTotalDiscountExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderSubTotalDiscountExclTax, order.CurrencyRate);
                if (orderSubTotalDiscountExclTaxInCustomerCurrency > decimal.Zero)
                    model.OrderSubTotalDiscount = _priceFormatter.FormatPrice(-orderSubTotalDiscountExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                //including tax

                //order shipping
                var orderShippingInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingInclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                //payment method additional fee
                var paymentMethodAdditionalFeeInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeInclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeInclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
            }
            else
            {
                //excluding tax

                //order shipping
                var orderShippingExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderShippingExclTax, order.CurrencyRate);
                model.OrderShipping = _priceFormatter.FormatShippingPrice(orderShippingExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                //payment method additional fee
                var paymentMethodAdditionalFeeExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.PaymentMethodAdditionalFeeExclTax, order.CurrencyRate);
                if (paymentMethodAdditionalFeeExclTaxInCustomerCurrency > decimal.Zero)
                    model.PaymentMethodAdditionalFee = _priceFormatter.FormatPaymentMethodAdditionalFee(paymentMethodAdditionalFeeExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
            }

            //tax
            bool displayTax = true;
            bool displayTaxRates = true;
            if (_taxSettings.HideTaxInOrderSummary && order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
            {
                displayTax = false;
                displayTaxRates = false;
            }
            else
            {
                if (order.OrderTax == 0 && _taxSettings.HideZeroTax)
                {
                    displayTax = false;
                    displayTaxRates = false;
                }
                else
                {
                    displayTaxRates = _taxSettings.DisplayTaxRates && order.TaxRatesDictionary.Count > 0;
                    displayTax = !displayTaxRates;

                    var orderTaxInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTax, order.CurrencyRate);
                    //TODO pass languageId to _priceFormatter.FormatPrice
                    model.Tax = _priceFormatter.FormatPrice(orderTaxInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

                    foreach (var tr in order.TaxRatesDictionary)
                    {
                        model.TaxRates.Add(new OrderDetailsModel.TaxRate
                        {
                            Rate = _priceFormatter.FormatTaxRate(tr.Key),
                            //TODO pass languageId to _priceFormatter.FormatPrice
                            Value = _priceFormatter.FormatPrice(_currencyService.ConvertCurrency(tr.Value, order.CurrencyRate), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                        });
                    }
                }
            }
            model.DisplayTaxRates = displayTaxRates;
            model.DisplayTax = displayTax;
            model.DisplayTaxShippingInfo = _catalogSettings.DisplayTaxShippingInfoOrderDetailsPage;
            model.PricesIncludeTax = order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax;

            //discount (applied to order total)
            var orderDiscountInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderDiscount, order.CurrencyRate);
            if (orderDiscountInCustomerCurrency > decimal.Zero)
                model.OrderTotalDiscount = _priceFormatter.FormatPrice(-orderDiscountInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);


            //gift cards
            foreach (var gcuh in order.GiftCardUsageHistory)
            {
                model.GiftCards.Add(new OrderDetailsModel.GiftCard
                {
                    CouponCode = gcuh.GiftCard.GiftCardCouponCode,
                    Amount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(gcuh.UsedValue, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage),
                });
            }

            //reward points           
            if (order.RedeemedRewardPointsEntry != null)
            {
                model.RedeemedRewardPoints = -order.RedeemedRewardPointsEntry.Points;
                model.RedeemedRewardPointsAmount = _priceFormatter.FormatPrice(-(_currencyService.ConvertCurrency(order.RedeemedRewardPointsEntry.UsedAmount, order.CurrencyRate)), true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);
            }

            //total
            var orderTotalInCustomerCurrency = _currencyService.ConvertCurrency(order.OrderTotal, order.CurrencyRate);
            model.OrderTotal = _priceFormatter.FormatPrice(orderTotalInCustomerCurrency, true, order.CustomerCurrencyCode, false, _workContext.WorkingLanguage);

            //checkout attributes
            model.CheckoutAttributeInfo = order.CheckoutAttributeDescription;

            //order notes
            foreach (var orderNote in order.OrderNotes
                .Where(on => on.DisplayToCustomer)
                .OrderByDescending(on => on.CreatedOnUtc)
                .ToList())
            {
                model.OrderNotes.Add(new OrderDetailsModel.OrderNote
                {
                    Id = orderNote.Id,
                    HasDownload = orderNote.DownloadId > 0,
                    Note = orderNote.FormatOrderNoteText(),
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(orderNote.CreatedOnUtc, DateTimeKind.Utc)
                });
            }


            //purchased products
            model.ShowSku = _catalogSettings.ShowProductSku;
           // var orderItems = _orderService.GetAllOrderItems(order.Id, null, null, null, null, null, null);
            var orderItems = order.OrderItems; // change3.8
            foreach (var orderItem in orderItems)
            {
                var orderItemModel = new OrderDetailsModel.OrderItemModel
                {
                    Id = orderItem.Id,
                    OrderItemGuid = orderItem.OrderItemGuid,
                    Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                    ProductId = orderItem.Product.Id,
                    ProductName = orderItem.Product.GetLocalized(x => x.Name),
                    ProductSeName = orderItem.Product.GetSeName(),
                    Quantity = orderItem.Quantity,
                    AttributeInfo = orderItem.AttributeDescription,
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                    orderItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(orderItemModel);

                //unit price, subtotal
                if (order.CustomerTaxDisplayType == TaxDisplayType.IncludingTax)
                {
                    //including tax
                    var unitPriceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceInclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);

                    var priceInclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceInclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceInclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, true);
                }
                else
                {
                    //excluding tax
                    var unitPriceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.UnitPriceExclTax, order.CurrencyRate);
                    orderItemModel.UnitPrice = _priceFormatter.FormatPrice(unitPriceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);

                    var priceExclTaxInCustomerCurrency = _currencyService.ConvertCurrency(orderItem.PriceExclTax, order.CurrencyRate);
                    orderItemModel.SubTotal = _priceFormatter.FormatPrice(priceExclTaxInCustomerCurrency, true, order.CustomerCurrencyCode, _workContext.WorkingLanguage, false);
                }

                //downloadable products
                if (_downloadService.IsDownloadAllowed(orderItem))
                    orderItemModel.DownloadId = orderItem.Product.DownloadId;
                if (_downloadService.IsLicenseDownloadAllowed(orderItem))
                    orderItemModel.LicenseId = orderItem.LicenseDownloadId.HasValue ? orderItem.LicenseDownloadId.Value : 0;
            }

            return model;
        }


        [NonAction]
        protected virtual ShipmentDetailsModel PrepareShipmentDetailsModel(Shipment shipment)
        {
            if (shipment == null)
                throw new ArgumentNullException("shipment");

            var order = shipment.Order;
            if (order == null)
                throw new Exception("order cannot be loaded");
            var model = new ShipmentDetailsModel();

            model.Id = shipment.Id;
            if (shipment.ShippedDateUtc.HasValue)
                model.ShippedDate = _dateTimeHelper.ConvertToUserTime(shipment.ShippedDateUtc.Value, DateTimeKind.Utc);
            if (shipment.DeliveryDateUtc.HasValue)
                model.DeliveryDate = _dateTimeHelper.ConvertToUserTime(shipment.DeliveryDateUtc.Value, DateTimeKind.Utc);

            //tracking number and shipment information
            if (!String.IsNullOrEmpty(shipment.TrackingNumber))
            {
                model.TrackingNumber = shipment.TrackingNumber;
                var srcm = _shippingService.LoadShippingRateComputationMethodBySystemName(order.ShippingRateComputationMethodSystemName);
                if (srcm != null &&
                    srcm.PluginDescriptor.Installed &&
                    srcm.IsShippingRateComputationMethodActive(_shippingSettings))
                {
                    var shipmentTracker = srcm.ShipmentTracker;
                    if (shipmentTracker != null)
                    {
                        model.TrackingNumberUrl = shipmentTracker.GetUrl(shipment.TrackingNumber);
                        if (_shippingSettings.DisplayShipmentEventsToCustomers)
                        {
                            var shipmentEvents = shipmentTracker.GetShipmentEvents(shipment.TrackingNumber);
                            if (shipmentEvents != null)
                            {
                                foreach (var shipmentEvent in shipmentEvents)
                                {
                                    var shipmentStatusEventModel = new ShipmentDetailsModel.ShipmentStatusEventModel();
                                    var shipmentEventCountry = _countryService.GetCountryByTwoLetterIsoCode(shipmentEvent.CountryCode);
                                    shipmentStatusEventModel.Country = shipmentEventCountry != null
                                                                           ? shipmentEventCountry.GetLocalized(x => x.Name)
                                                                           : shipmentEvent.CountryCode;
                                    shipmentStatusEventModel.Date = shipmentEvent.Date;
                                    shipmentStatusEventModel.EventName = shipmentEvent.EventName;
                                    shipmentStatusEventModel.Location = shipmentEvent.Location;
                                    model.ShipmentStatusEvents.Add(shipmentStatusEventModel);
                                }
                            }
                        }
                    }
                }
            }

            //products in this shipment
            model.ShowSku = _catalogSettings.ShowProductSku;
            foreach (var shipmentItem in shipment.ShipmentItems)
            {
                var orderItem = _orderService.GetOrderItemById(shipmentItem.OrderItemId);
                if (orderItem == null)
                    continue;

                var shipmentItemModel = new ShipmentDetailsModel.ShipmentItemModel
                {
                    Id = shipmentItem.Id,
                    Sku = orderItem.Product.FormatSku(orderItem.AttributesXml, _productAttributeParser),
                    ProductId = orderItem.Product.Id,
                    ProductName = orderItem.Product.GetLocalized(x => x.Name),
                    ProductSeName = orderItem.Product.GetSeName(),
                    AttributeInfo = orderItem.AttributeDescription,
                    QuantityOrdered = orderItem.Quantity,
                    QuantityShipped = shipmentItem.Quantity,
                };
                //rental info
                if (orderItem.Product.IsRental)
                {
                    var rentalStartDate = orderItem.RentalStartDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalStartDateUtc.Value) : "";
                    var rentalEndDate = orderItem.RentalEndDateUtc.HasValue ? orderItem.Product.FormatRentalDate(orderItem.RentalEndDateUtc.Value) : "";
                    shipmentItemModel.RentalInfo = string.Format(_localizationService.GetResource("Order.Rental.FormattedDate"),
                        rentalStartDate, rentalEndDate);
                }
                model.Items.Add(shipmentItemModel);
            }

            //order details model
            model.Order = PrepareOrderDetailsModel(order);

            return model;
        }


        #endregion

        #region Methods

        #region Old methods
        /*--------------------------------------------------Get And Set Overview Settings---------------------------------------------------*/
        // POST api/NopMobileWebApi/GetOverviewSettings
        [Route("GetOverviewSettings")]
        public ConfigureModel GetOverviewSettings()
        {
            var storeScope = GetActiveStoreScopeConfiguration();

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

            return model;

        }

        // POST api/NopMobileWebApi/SetOverviewSettings
        [HttpPost]
        [Route("SetOverviewSettings")]
        public IHttpActionResult SetOverviewSettings(ConfigureModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();

            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.AndroidAppStatus = model.AndroidAppStatus;
            BsNopMobileSettings.AppKey = model.AppKey;
            BsNopMobileSettings.AppName = model.AppName;
            BsNopMobileSettings.CreatedDate = model.CreatedDate;
            BsNopMobileSettings.DownloadUrl = model.DownloadUrl;
            BsNopMobileSettings.iOsAPPUDID = model.iOsAPPUDID;
            BsNopMobileSettings.MobilWebsiteURL = model.MobilWebsiteURL;

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

            //now clear settings cache
            _settingService.ClearCache();

            return Ok();
        }

        /*--------------------------------------------------Get And Set CategoryIcons Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetCategoryIconsSettings
        //[Route("GetCategoryIconsSettings")]
        //public CategoryIconsModel GetCategoryIconsSettings()
        //{
        //    var storeScope = GetActiveStoreScopeConfiguration();
        //    var BsNopMobileSettings = _settingService.LoadSetting<BsNopMobileSettings>(storeScope);
        //    var model = new CategoryIconsModel();
        //    model.ActiveStoreScopeConfiguration = storeScope;
        //    model.Willusedefaultnopcategory = BsNopMobileSettings.Willusedefaultnopcategory;

        //    if (storeScope > 0)
        //    {
        //        model.Willusedefaultnopcategory_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.Willusedefaultnopcategory, storeScope);

        //    }
        //    return model;
        //}

        // POST api/NopMobileWebApi/SetCategoryIconsSettings
        //[HttpPost]
        //[Route("SetCategoryIconsSettings")]
        //public IHttpActionResult SetCategoryIconsSettings(CategoryIconsModel model)
        //{
        //    var storeScope = GetActiveStoreScopeConfiguration();
        //    var BsNopMobileSettings = _settingService.LoadSetting<BsNopMobileSettings>(storeScope);

        //    BsNopMobileSettings.Willusedefaultnopcategory = model.Willusedefaultnopcategory;

        //    if (model.Willusedefaultnopcategory_OverrideForStore || storeScope == 0)
        //        _settingService.SaveSetting(BsNopMobileSettings, x => x.Willusedefaultnopcategory, storeScope, false);
        //    else if (storeScope > 0)
        //        _settingService.DeleteSetting(BsNopMobileSettings, x => x.Willusedefaultnopcategory, storeScope);

        //    //now clear settings cache
        //    _settingService.ClearCache();

        //    return Ok();
        //}

        /*--------------------------------------------------Get And Set ContentManagement Settings---------------------------------------------------*/


        // POST api/NopMobileWebApi/GetContentManagementSettings
        [Route("GetContentManagementSettings")]
        public ContentManagementModel GetContentManagementSettings()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new ContentManagementModel();
            model.ActiveStoreScopeConfiguration = storeScope;
            model.DefaultNopFlowSameAs = BsNopMobileSettings.DefaultNopFlowSameAs;

            if (storeScope > 0)
            {
                model.DefaultNopFlowSameAs_OverrideForStore = _settingService.SettingExists(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);

            }
            return model;
        }

        // POST api/NopMobileWebApi/SetContentManagementSettings
        [HttpPost]
        [Route("SetContentManagementSettings")]
        public IHttpActionResult SetContentManagementSettings(ContentManagementModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.DefaultNopFlowSameAs = model.DefaultNopFlowSameAs;

            if (model.DefaultNopFlowSameAs_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(BsNopMobileSettings, x => x.DefaultNopFlowSameAs, storeScope);


            //now clear settings cache
            _settingService.ClearCache();

            return Ok();
        }

        /*--------------------------------------------------Get And Set MobilSe Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetMobileWebSiteSetting
        [Route("GetMobileWebSiteSetting")]
        public MobileSettingsModel GetMobileWebSiteSetting()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
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

            }
            return model;
        }

        // POST api/NopMobileWebApi/SetMobileWebSiteSetting
        [HttpPost]
        [Route("SetMobileWebSiteSetting")]
        public IHttpActionResult SetMobileWebSiteSetting(MobileSettingsModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
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

            //now clear settings cache
            _settingService.ClearCache();

            return Ok();
        }

        /*--------------------------------------------------Get And Set PushNotification Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetPushNotificationSetting
        [Route("GetPushNotificationSetting")]
        public PushNotificationModel GetPushNotificationSetting()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
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
            return model;
        }

        // POST api/NopMobileWebApi/SetPushNotificationSetting
        [HttpPost]
        [Route("SetPushNotificationSetting")]
        public IHttpActionResult SetPushNotificationSetting(PushNotificationModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.PushNotificationHeading = model.PushNotificationHeading;
            BsNopMobileSettings.PushNotificationMessage = model.PushNotificationMessage;


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

            return Ok();
        }

        /*--------------------------------------------------Get And Set Theme Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetThemeSetting
        [Route("GetThemeSetting")]
        public ThemeSettingModel GetThemeSetting()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
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
            return model;
        }

        // POST api/NopMobileWebApi/SetThemeSetting
        [HttpPost]
        [Route("SetThemeSetting")]
        public IHttpActionResult SetThemeSetting(ThemeSettingModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.HeaderBackgroundColor = model.HeaderBackgroundColor;
            BsNopMobileSettings.HeaderFontandIconColor = model.HeaderFontandIconColor;
            BsNopMobileSettings.HighlightedTextColor = model.HighlightedTextColor;
            BsNopMobileSettings.PrimaryTextColor = model.PrimaryTextColor;
            BsNopMobileSettings.SecondaryTextColor = model.SecondaryTextColor;
            BsNopMobileSettings.BackgroundColorofPrimaryButton = model.BackgroundColorofPrimaryButton;
            BsNopMobileSettings.TextColorofPrimaryButton = model.TextColorofPrimaryButton;
            BsNopMobileSettings.BackgroundColorofSecondaryButton = model.BackgroundColorofSecondaryButton;



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
            return Ok();

        }

        /*--------------------------------------------------Get And Set BannerSlider Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetBannerSliderSetting
        [Route("GetBannerSliderSetting")]
        public BannerSliderModel GetBannerSliderSetting()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            var model = new BannerSliderModel();
            return model;
        }

        // POST api/NopMobileWebApi/SetBannerSliderSetting
        [HttpPost]
        [Route("SetBannerSliderSetting")]
        public IHttpActionResult SetBannerSliderSetting(BannerSliderModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);
            return Ok();
        }

        /*--------------------------------------------------Get And Set General Settings---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetGeneralSetting
        [Route("GetGeneralSetting")]
        public GeneralSettingModel GetGeneralSetting()
        {
            var storeScope = GetActiveStoreScopeConfiguration();
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
            return model;
        }

        // POST api/NopMobileWebApi/SetGeneralSetting
        [HttpPost]
        [Route("SetGeneralSetting")]
        public IHttpActionResult SetGeneralSetting(GeneralSettingModel model)
        {
            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            BsNopMobileSettings.EnableBestseller = model.EnableBestseller;
            BsNopMobileSettings.EnableFeaturedProducts = model.EnableFeaturedProducts;
            BsNopMobileSettings.EnableNewProducts = model.EnableNewProducts;

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


            return Ok();
        }

        // POST api/NopMobileWebApi/GetFeatureProductList
        [Route("GetFeatureProductList")]
        public IList<ProductModel> GetFeatureProductList()
        {
            var FeatureProducts = _nopMobileService.GetAllPluginFeatureProducts();
            if (FeatureProducts.Count() > 0)
            {
                var productlist = _productService.GetProductsByIds(FeatureProducts.Select(x => x.ProductId).ToArray()).Select(p => new ProductModel()
                {
                    AdditionalShippingCharge = p.AdditionalShippingCharge,
                    AdminComment = p.AdminComment,
                    AllowAddingOnlyExistingAttributeCombinations = p.AllowAddingOnlyExistingAttributeCombinations,
                    AllowBackInStockSubscriptions = p.AllowBackInStockSubscriptions,
                    AllowCustomerReviews = p.AllowCustomerReviews,
                    AllowedQuantities = p.AllowedQuantities,
                    //AppliedDiscounts = p.AppliedDiscounts.ToList(),
                    ApprovedRatingSum = p.ApprovedRatingSum,
                    ApprovedTotalReviews = p.ApprovedTotalReviews,
                    AutomaticallyAddRequiredProducts = p.AutomaticallyAddRequiredProducts,
                    AvailableEndDateTimeUtc = p.AvailableEndDateTimeUtc,
                    AvailableForPreOrder = p.AvailableForPreOrder,
                    AvailableStartDateTimeUtc = p.AvailableStartDateTimeUtc,
                    BackorderMode = p.BackorderMode,
                    BackorderModeId = p.BackorderModeId,
                    BasepriceAmount = p.BasepriceAmount,
                    BasepriceBaseAmount = p.BasepriceBaseAmount,
                    BasepriceBaseUnitId = p.BasepriceBaseUnitId,
                    BasepriceEnabled = p.BasepriceEnabled,
                    BasepriceUnitId = p.BasepriceUnitId,
                    CallForPrice = p.CallForPrice,
                    CreatedOnUtc = p.CreatedOnUtc,
                    CustomerEntersPrice = p.CustomerEntersPrice,
                    Deleted = p.Deleted,
                    DeliveryDateId = p.DeliveryDateId,
                    DisableBuyButton = p.DisableBuyButton,
                    DisableWishlistButton = p.DisableWishlistButton,
                    DisplayOrder = p.DisplayOrder,
                    DisplayStockAvailability = p.DisplayStockAvailability,
                    DisplayStockQuantity = p.DisplayStockQuantity,
                    DownloadActivationType = p.DownloadActivationType,
                    DownloadActivationTypeId = p.DownloadActivationTypeId,
                    DownloadExpirationDays = p.DownloadExpirationDays,
                    DownloadId = p.DownloadId,
                    FullDescription = p.FullDescription,
                    GiftCardType = p.GiftCardType,
                    GiftCardTypeId = p.GiftCardTypeId,
                    Gtin = p.Gtin,
                    HasDiscountsApplied = p.HasDiscountsApplied,
                    HasSampleDownload = p.HasSampleDownload,
                    HasTierPrices = p.HasTierPrices,
                    HasUserAgreement = p.HasUserAgreement,
                    Height = p.Height,
                    Id = p.Id,
                    IsDownload = p.IsDownload,
                    IsFreeShipping = p.IsFreeShipping,
                    IsGiftCard = p.IsGiftCard,
                    IsRecurring = p.IsRecurring,
                    IsRental = p.IsRental,
                    ProductType = p.ProductType,
                    ProductTemplateId = p.ProductTemplateId,
                    //ProductTags = p.ProductTags.ToList(),
                    //ProductSpecificationAttributes = p.ProductSpecificationAttributes.ToList(),
                    RecurringCycleLength = p.RecurringCycleLength,
                    IsShipEnabled = p.IsShipEnabled,
                    IsTaxExempt = p.IsTaxExempt,
                    IsTelecommunicationsOrBroadcastingOrElectronicServices = p.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                    Length = p.Length,
                    LimitedToStores = p.LimitedToStores,
                    LowStockActivity = p.LowStockActivity,
                    LowStockActivityId = p.LowStockActivityId,
                    ManageInventoryMethod = p.ManageInventoryMethod,
                    ManageInventoryMethodId = p.ManageInventoryMethodId,
                    ManufacturerPartNumber = p.ManufacturerPartNumber,
                    MaximumCustomerEnteredPrice = p.MaximumCustomerEnteredPrice,
                    MaxNumberOfDownloads = p.MaxNumberOfDownloads,
                    MetaDescription = p.MetaDescription,
                    MetaKeywords = p.MetaKeywords,
                    MetaTitle = p.MetaTitle,
                    MinimumCustomerEnteredPrice = p.MinimumCustomerEnteredPrice,
                    MinStockQuantity = p.MinStockQuantity,
                    Name = p.Name,
                    NotApprovedRatingSum = p.NotApprovedRatingSum,
                    NotApprovedTotalReviews = p.NotApprovedTotalReviews,
                    NotifyAdminForQuantityBelow = p.NotifyAdminForQuantityBelow,
                    OldPrice = p.OldPrice,
                    OrderMaximumQuantity = p.OrderMaximumQuantity,
                    OrderMinimumQuantity = p.OrderMinimumQuantity,
                    ParentGroupedProductId = p.ParentGroupedProductId,
                    PreOrderAvailabilityStartDateTimeUtc = p.PreOrderAvailabilityStartDateTimeUtc,
                    Price = p.Price,
                    //ProductAttributeCombinations = p.ProductAttributeCombinations.ToList(),
                    //ProductAttributeMappings = p.ProductAttributeMappings.ToList(),
                    //ProductCategories = p.ProductCategories.ToList(),
                    ProductCost = p.ProductCost,
                    SpecialProductCost = p.SpecialProductCost,
                    SpecialProductCostStartDateTimeUtc = p.SpecialProductCostStartDateTimeUtc,
                    SpecialProductCostEndDateTimeUtc = p.SpecialProductCostEndDateTimeUtc,
                    //ProductManufacturers = p.ProductManufacturers.ToList(),
                    //ProductPictures = p.ProductPictures.ToList(),
                    //ProductReviews = p.ProductReviews.ToList(),
                    ProductTypeId = p.ProductTypeId,
                    //ProductWarehouseInventory = p.ProductWarehouseInventory.ToList(),
                    Published = p.Published,
                    RecurringCyclePeriod = p.RecurringCyclePeriod,
                    RecurringCyclePeriodId = p.RecurringCyclePeriodId,
                    RecurringTotalCycles = p.RecurringTotalCycles,
                    RentalPriceLength = p.RentalPriceLength,
                    RentalPricePeriod = p.RentalPricePeriod,
                    RentalPricePeriodId = p.RentalPricePeriodId,
                    RequiredProductIds = p.RequiredProductIds,
                    RequireOtherProducts = p.RequireOtherProducts,
                    SampleDownloadId = p.SampleDownloadId,
                    ShipSeparately = p.ShipSeparately,
                    ShortDescription = p.ShortDescription,
                    ShowOnHomePage = p.ShowOnHomePage,
                    Sku = p.Sku,
                    SpecialPrice = p.SpecialPrice,
                    SpecialPriceEndDateTimeUtc = p.SpecialPriceEndDateTimeUtc,
                    SpecialPriceStartDateTimeUtc = p.SpecialPriceStartDateTimeUtc,
                    StockQuantity = p.StockQuantity,
                    SubjectToAcl = p.SubjectToAcl,
                    TaxCategoryId = p.TaxCategoryId,
                    //TierPrices = p.TierPrices.ToList(),
                    UnlimitedDownloads = p.UnlimitedDownloads,
                    UpdatedOnUtc = p.UpdatedOnUtc,
                    UseMultipleWarehouses = p.UseMultipleWarehouses,
                    UserAgreementText = p.UserAgreementText,
                    VendorId = p.VendorId,
                    VisibleIndividually = p.VisibleIndividually,
                    WarehouseId = p.WarehouseId,
                    Weight = p.Weight,
                    Width = p.Width,

                });
                return productlist.ToList();
            }
            return null;
        }

        // POST api/NopMobileWebApi/GetFeatureProductListID
        [Route("GetFeatureProductListID")]
        public IList<int> GetFeatureProductListID()
        {
            var FeatureProducts = _nopMobileService.GetAllPluginFeatureProducts();
            if (FeatureProducts.Count() > 0)
            {
                var productlist = _productService.GetProductsByIds(FeatureProducts.Select(x => x.ProductId).ToArray());
                return productlist.Select(x => x.Id).ToList();
            }
            return null;
        }

        // POST api/NopMobileWebApi/SetFeatureProductList
        [HttpPost]
        [Route("SetFeatureProductList")]
        public IHttpActionResult SetFeatureProductList(IList<int> productids)
        {
            if (productids.Count() > 0)
            {

                var productidlist = _nopMobileService.GetAllPluginFeatureProducts().Where(x => productids.Contains(x.ProductId)).Select(x => x.ProductId);

                foreach (int id in productids)
                {
                    var Products = _productService.GetProductById(id);
                    if (Products != null)
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

                return Ok();
            }

            return BadRequest();
        }

        // POST api/NopMobileWebApi/DeleteFeatureProductListByIDS
        [HttpPost]
        [Route("DeleteFeatureProductListByIDS")]
        public IHttpActionResult DeleteFeatureProductListByIDS(IList<int> productids)
        {
            if (productids.Count() > 0)
            {
                foreach (var id in productids)
                {
                    var item = _nopMobileService.GetPluginFeatureProductsById(id);
                    if (item != null)
                    {
                        _nopMobileService.DeleteFeatureProducts(item);
                    }
                }
                return Ok();
            }
            return BadRequest();
        }

        /*--------------------------------------------------Get And Set TopicPage ---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetTopicList
        [Route("GetTopicList")]
        public IList<TopicModel> GetTopicList()
        {
            var topicList = _contentManagementService.GetAllTopics(0);
            var topicModels = topicList.Select(x => new TopicModel()
            {
                AccessibleWhenStoreClosed = x.AccessibleWhenStoreClosed,
                Body = x.Body,
                DisplayOrder = x.DisplayOrder,
                Id = x.Id,
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
                TopicTemplateId = x.TopicTemplateId
            });
            return topicModels.ToList();
        }

        // POST api/NopMobileWebApi/GetTopicById
        [Route("GetTopicById")]
        public TopicModel GetTopicById(int id)
        {
            var topic = _contentManagementService.GetTopicById(id);
            var topicModels = new TopicModel()
            {
                AccessibleWhenStoreClosed = topic.AccessibleWhenStoreClosed,
                Body = topic.Body,
                DisplayOrder = topic.DisplayOrder,
                Id = topic.Id,
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
            return topicModels;
        }

        // POST api/NopMobileWebApi/DeleteTopicById
        [HttpPost]
        [Route("DeleteTopicById")]
        public IHttpActionResult DeleteTopicById(int id)
        {
            var topic = _contentManagementService.GetTopicById(id);

            if (topic != null)
            {
                _contentManagementService.DeleteTopic(topic);
                return Ok();
            }
            else
                return BadRequest();
        }

        // POST api/NopMobileWebApi/InsertTopic
        [HttpPost]
        [Route("InsertTopic")]
        public IHttpActionResult InsertTopic(TopicModel model)
        {
            if (model != null)
            {
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
                return Ok();
            }
            return BadRequest();
        }

        // POST api/NopMobileWebApi/UpdateTopic
        [HttpPost]
        [Route("UpdateTopic")]
        public IHttpActionResult UpdateTopic(TopicModel model)
        {
            if (model != null && model.Id > 0)
            {
                var topic = _contentManagementService.GetTopicById(model.Id);

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
                return Ok();
            }
            return BadRequest();
        }

        /*--------------------------------------------------Get Product ---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetProduct
        [Route("GetProduct")]
        public IList<ProductModel> GetProduct(int pageIndex = 1,
            int pageSize = int.MaxValue,
            int manufacturerId = 0,
            int storeId = 0,
            int vendorId = 0,
            int warehouseId = 0,
            ProductType? productType = null,
            bool visibleIndividuallyOnly = false,
            bool? featuredProducts = null,
            decimal? priceMin = null,
            decimal? priceMax = null,
            int productTagId = 0,
            string keywords = null,
            bool searchDescriptions = false,
            bool searchSku = true,
            bool searchProductTags = false,
            int languageId = 0,
            ProductSortingEnum orderBy = ProductSortingEnum.Position,
            bool showHidden = false,
            bool? overridePublished = null)
        {

            var products = _productService.SearchProducts(
                        //categoryIds: categoryIds.ToList(),
                        manufacturerId: manufacturerId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: priceMin,
                        priceMax: priceMax,
                        keywords: keywords,
                        searchDescriptions: searchDescriptions,
                        searchSku: searchSku,
                        searchProductTags: searchProductTags,
                        languageId: languageId,
                        //filteredSpecs: filteredSpecs.ToList(),
                        orderBy: (ProductSortingEnum)orderBy,
                        pageIndex: pageIndex - 1,
                        pageSize: pageSize).Select(p => new ProductModel()
                        {

                            AdditionalShippingCharge = p.AdditionalShippingCharge,
                            AdminComment = p.AdminComment,
                            AllowAddingOnlyExistingAttributeCombinations = p.AllowAddingOnlyExistingAttributeCombinations,
                            AllowBackInStockSubscriptions = p.AllowBackInStockSubscriptions,
                            AllowCustomerReviews = p.AllowCustomerReviews,
                            AllowedQuantities = p.AllowedQuantities,
                            //AppliedDiscounts = p.AppliedDiscounts.ToList(),
                            ApprovedRatingSum = p.ApprovedRatingSum,
                            ApprovedTotalReviews = p.ApprovedTotalReviews,
                            AutomaticallyAddRequiredProducts = p.AutomaticallyAddRequiredProducts,
                            AvailableEndDateTimeUtc = p.AvailableEndDateTimeUtc,
                            AvailableForPreOrder = p.AvailableForPreOrder,
                            AvailableStartDateTimeUtc = p.AvailableStartDateTimeUtc,
                            BackorderMode = p.BackorderMode,
                            BackorderModeId = p.BackorderModeId,
                            BasepriceAmount = p.BasepriceAmount,
                            BasepriceBaseAmount = p.BasepriceBaseAmount,
                            BasepriceBaseUnitId = p.BasepriceBaseUnitId,
                            BasepriceEnabled = p.BasepriceEnabled,
                            BasepriceUnitId = p.BasepriceUnitId,
                            CallForPrice = p.CallForPrice,
                            CreatedOnUtc = p.CreatedOnUtc,
                            CustomerEntersPrice = p.CustomerEntersPrice,
                            Deleted = p.Deleted,
                            DeliveryDateId = p.DeliveryDateId,
                            DisableBuyButton = p.DisableBuyButton,
                            DisableWishlistButton = p.DisableWishlistButton,
                            DisplayOrder = p.DisplayOrder,
                            DisplayStockAvailability = p.DisplayStockAvailability,
                            DisplayStockQuantity = p.DisplayStockQuantity,
                            DownloadActivationType = p.DownloadActivationType,
                            DownloadActivationTypeId = p.DownloadActivationTypeId,
                            DownloadExpirationDays = p.DownloadExpirationDays,
                            DownloadId = p.DownloadId,
                            FullDescription = p.FullDescription,
                            GiftCardType = p.GiftCardType,
                            GiftCardTypeId = p.GiftCardTypeId,
                            Gtin = p.Gtin,
                            HasDiscountsApplied = p.HasDiscountsApplied,
                            HasSampleDownload = p.HasSampleDownload,
                            HasTierPrices = p.HasTierPrices,
                            HasUserAgreement = p.HasUserAgreement,
                            Height = p.Height,
                            Id = p.Id,
                            IsDownload = p.IsDownload,
                            IsFreeShipping = p.IsFreeShipping,
                            IsGiftCard = p.IsGiftCard,
                            IsRecurring = p.IsRecurring,
                            IsRental = p.IsRental,
                            ProductType = p.ProductType,
                            ProductTemplateId = p.ProductTemplateId,
                            //ProductTags = p.ProductTags.ToList(),
                            //ProductSpecificationAttributes = p.ProductSpecificationAttributes.ToList(),
                            RecurringCycleLength = p.RecurringCycleLength,
                            IsShipEnabled = p.IsShipEnabled,
                            IsTaxExempt = p.IsTaxExempt,
                            IsTelecommunicationsOrBroadcastingOrElectronicServices = p.IsTelecommunicationsOrBroadcastingOrElectronicServices,
                            Length = p.Length,
                            LimitedToStores = p.LimitedToStores,
                            LowStockActivity = p.LowStockActivity,
                            LowStockActivityId = p.LowStockActivityId,
                            ManageInventoryMethod = p.ManageInventoryMethod,
                            ManageInventoryMethodId = p.ManageInventoryMethodId,
                            ManufacturerPartNumber = p.ManufacturerPartNumber,
                            MaximumCustomerEnteredPrice = p.MaximumCustomerEnteredPrice,
                            MaxNumberOfDownloads = p.MaxNumberOfDownloads,
                            MetaDescription = p.MetaDescription,
                            MetaKeywords = p.MetaKeywords,
                            MetaTitle = p.MetaTitle,
                            MinimumCustomerEnteredPrice = p.MinimumCustomerEnteredPrice,
                            MinStockQuantity = p.MinStockQuantity,
                            Name = p.Name,
                            NotApprovedRatingSum = p.NotApprovedRatingSum,
                            NotApprovedTotalReviews = p.NotApprovedTotalReviews,
                            NotifyAdminForQuantityBelow = p.NotifyAdminForQuantityBelow,
                            OldPrice = p.OldPrice,
                            OrderMaximumQuantity = p.OrderMaximumQuantity,
                            OrderMinimumQuantity = p.OrderMinimumQuantity,
                            ParentGroupedProductId = p.ParentGroupedProductId,
                            PreOrderAvailabilityStartDateTimeUtc = p.PreOrderAvailabilityStartDateTimeUtc,
                            Price = p.Price,
                            //ProductAttributeCombinations = p.ProductAttributeCombinations.ToList(),
                            //ProductAttributeMappings = p.ProductAttributeMappings.ToList(),
                            //ProductCategories = p.ProductCategories.ToList(),
                            ProductCost = p.ProductCost,
                            SpecialProductCost = p.SpecialProductCost,
                            SpecialProductCostStartDateTimeUtc = p.SpecialProductCostStartDateTimeUtc,
                            SpecialProductCostEndDateTimeUtc = p.SpecialProductCostEndDateTimeUtc,
                            //ProductManufacturers = p.ProductManufacturers.ToList(),
                            //ProductPictures = p.ProductPictures.ToList(),
                            //ProductReviews = p.ProductReviews.ToList(),
                            ProductTypeId = p.ProductTypeId,
                            //ProductWarehouseInventory = p.ProductWarehouseInventory.ToList(),
                            Published = p.Published,
                            RecurringCyclePeriod = p.RecurringCyclePeriod,
                            RecurringCyclePeriodId = p.RecurringCyclePeriodId,
                            RecurringTotalCycles = p.RecurringTotalCycles,
                            RentalPriceLength = p.RentalPriceLength,
                            RentalPricePeriod = p.RentalPricePeriod,
                            RentalPricePeriodId = p.RentalPricePeriodId,
                            RequiredProductIds = p.RequiredProductIds,
                            RequireOtherProducts = p.RequireOtherProducts,
                            SampleDownloadId = p.SampleDownloadId,
                            ShipSeparately = p.ShipSeparately,
                            ShortDescription = p.ShortDescription,
                            ShowOnHomePage = p.ShowOnHomePage,
                            Sku = p.Sku,
                            SpecialPrice = p.SpecialPrice,
                            SpecialPriceEndDateTimeUtc = p.SpecialPriceEndDateTimeUtc,
                            SpecialPriceStartDateTimeUtc = p.SpecialPriceStartDateTimeUtc,
                            StockQuantity = p.StockQuantity,
                            SubjectToAcl = p.SubjectToAcl,
                            TaxCategoryId = p.TaxCategoryId,
                            //TierPrices = p.TierPrices.ToList(),
                            UnlimitedDownloads = p.UnlimitedDownloads,
                            UpdatedOnUtc = p.UpdatedOnUtc,
                            UseMultipleWarehouses = p.UseMultipleWarehouses,
                            UserAgreementText = p.UserAgreementText,
                            VendorId = p.VendorId,
                            VisibleIndividually = p.VisibleIndividually,
                            WarehouseId = p.WarehouseId,
                            Weight = p.Weight,
                            Width = p.Width,
                        });
            return products.ToList();
        }

        /*--------------------------------------------------Get Category ---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetAllCategory
        [Route("GetAllCategory")]
        public IList<CategoryModel> GetAllCategory(string categoryname = "")
        {
            var category = _categoryService.GetAllCategories(categoryName: categoryname).Select(x => new CategoryModel()
            {
                Description = x.Description,
                Id = x.Id,
                MetaDescription = x.MetaDescription,
                MetaKeywords = x.MetaKeywords,
                Name = x.Name,
                MetaTitle = x.MetaTitle,
                SeName = x.GetSeName(),
                SubCategories = PrepaireSubCategory(x.Id),
                PictureUrl = (x.PictureId > 0 ? _pictureService.GetPictureUrl(x.PictureId) : ""),
                PictureId = x.PictureId
            });

            return category.ToList();
        }

        // POST api/NopMobileWebApi/GetCategoryById
        [Route("GetCategoryById")]
        public CategoryModel GetCategoryById(int categoryId)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category != null)
            {
                var cat = new CategoryModel()
                {
                    Description = category.Description,
                    Id = category.Id,
                    MetaDescription = category.MetaDescription,
                    MetaKeywords = category.MetaKeywords,
                    Name = category.Name,
                    MetaTitle = category.MetaTitle,
                    SeName = category.GetSeName(),
                    SubCategories = PrepaireSubCategory(category.Id),
                    PictureUrl = (category.PictureId > 0 ? _pictureService.GetPictureUrl(category.PictureId) : ""),
                    PictureId = category.PictureId
                };
                return cat;
            }
            return null;
        }

        // POST api/NopMobileWebApi/GetAllCategoriesByParentCategoryId
        [Route("GetAllCategoriesByParentCategoryId")]
        public IList<CategoryModel> GetAllCategoriesByParentCategoryId(int parentCategoryId)
        {
            var category = _categoryService.GetAllCategoriesByParentCategoryId(parentCategoryId).Select(x => new CategoryModel()
            {
                Description = x.Description,
                Id = x.Id,
                MetaDescription = x.MetaDescription,
                MetaKeywords = x.MetaKeywords,
                Name = x.Name,
                MetaTitle = x.MetaTitle,
                SeName = x.GetSeName(),
                SubCategories = PrepaireSubCategory(x.Id),
                PictureUrl = (x.PictureId > 0 ? _pictureService.GetPictureUrl(x.PictureId) : ""),
                PictureId = x.PictureId
            }).ToList();

            return category;
        }

        // POST api/NopMobileWebApi/GetAllCategoriesDisplayedOnHomePage
        [Route("GetAllCategoriesDisplayedOnHomePage")]
        public IList<CategoryModel> GetAllCategoriesDisplayedOnHomePage(bool showHidden = false)
        {
            var category = _categoryService.GetAllCategoriesDisplayedOnHomePage(showHidden: showHidden).Select(x => new CategoryModel()
            {
                Description = x.Description,
                Id = x.Id,
                MetaDescription = x.MetaDescription,
                MetaKeywords = x.MetaKeywords,
                Name = x.Name,
                MetaTitle = x.MetaTitle,
                SeName = x.GetSeName(),
                SubCategories = PrepaireSubCategory(x.Id),
                PictureUrl = (x.PictureId > 0 ? _pictureService.GetPictureUrl(x.PictureId) : ""),
                PictureId = x.PictureId
            }).ToList();

            return category;
        }


        /*--------------------------------------------------Get CustomerCart ---------------------------------------------------*/

        // POST api/NopMobileWebApi/GetCartByCustomerId
        [Route("GetCartByCustomerId")]
        public IList<ShoppingCartItemModel> GetCartByCustomerId(int customerId = 0, int cartTypeId = 0)
        {
            if (customerId > 0)
            {
                var customer = _customerService.GetCustomerById(customerId);
                var cart = customer.ShoppingCartItems.Where(x => x.ShoppingCartTypeId == cartTypeId).ToList();
                var cartdetail = cart.Select(sci => new ShoppingCartItemModel()
                {
                    Id = sci.Id,
                    Store = _storeService.GetStoreById(sci.StoreId) != null ? _storeService.GetStoreById(sci.StoreId).Name : "Unknown",
                    ProductId = sci.ProductId,
                    Quantity = sci.Quantity,
                    ProductName = sci.Product.Name,
                    AttributeInfo = _productAttributeFormatter.FormatAttributes(sci.Product, sci.AttributesXml),
                    //UnitPrice = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetUnitPrice(sci), out taxRate)),
                    //Total = _priceFormatter.FormatPrice(_taxService.GetProductPrice(sci.Product, _priceCalculationService.GetSubTotal(sci), out taxRate)),
                    UpdatedOn = _dateTimeHelper.ConvertToUserTime(sci.UpdatedOnUtc, DateTimeKind.Utc),

                }).ToList();
                return cartdetail;
            }
            return null;
        }


        /*--------------------------------------------------Get Customer---------------------------------------------------*/
        // POST api/NopMobileWebApi/GetCustomerById
        [Route("GetCustomerById")]
        public CustomerModel GetCustomerById(int customerId = 0)
        {
            if (customerId > 0)
            {
                var customer = _customerService.GetCustomerById(customerId);
                if (customer != null)
                {
                    var affiliate = _affiliateService.GetAffiliateById(customer.AffiliateId);

                    var cust = new CustomerModel()
                    {
                        Id = customer.Id,
                        Email = customer.Email,
                        Username = customer.Username,
                        AdminComment = customer.AdminComment,
                        IsTaxExempt = customer.IsTaxExempt,
                        Active = customer.Active,
                        AffiliateId = (affiliate != null) ? affiliate.Id : 0,
                        AffiliateName = (affiliate != null) ? affiliate.GetFullName() : "",
                        TimeZoneId = customer.GetAttribute<string>(SystemCustomerAttributeNames.TimeZoneId),
                        VatNumber = customer.GetAttribute<string>(SystemCustomerAttributeNames.VatNumber),
                        CreatedOn = _dateTimeHelper.ConvertToUserTime(customer.CreatedOnUtc, DateTimeKind.Utc),
                        LastActivityDate = _dateTimeHelper.ConvertToUserTime(customer.LastActivityDateUtc, DateTimeKind.Utc),
                        LastIpAddress = customer.LastIpAddress,
                        LastVisitedPage = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastVisitedPage),
                        SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToArray(),
                        //form fields
                        FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName),
                        LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName),
                        Gender = customer.GetAttribute<string>(SystemCustomerAttributeNames.Gender),
                        DateOfBirth = customer.GetAttribute<DateTime?>(SystemCustomerAttributeNames.DateOfBirth),
                        Company = customer.GetAttribute<string>(SystemCustomerAttributeNames.Company),
                        StreetAddress = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress),
                        StreetAddress2 = customer.GetAttribute<string>(SystemCustomerAttributeNames.StreetAddress2),
                        ZipPostalCode = customer.GetAttribute<string>(SystemCustomerAttributeNames.ZipPostalCode),
                        City = customer.GetAttribute<string>(SystemCustomerAttributeNames.City),
                        CountryId = customer.GetAttribute<int>(SystemCustomerAttributeNames.CountryId),
                        StateProvinceId = customer.GetAttribute<int>(SystemCustomerAttributeNames.StateProvinceId),
                        Phone = customer.GetAttribute<string>(SystemCustomerAttributeNames.Phone),
                        Fax = customer.GetAttribute<string>(SystemCustomerAttributeNames.Fax),
                    };
                    return cust;
                }

            }
            return null;
        }

        /*--------------------------------------------------Add Product to cart---------------------------------------------------*/
        // POST api/NopMobileWebApi/AddProductToCart_Catalog
        //[HttpPost]
        //[Route("AddProductToCart_Catalog")]
        //public IHttpActionResult AddProductToCart_Catalog(int productId, int shoppingCartTypeId,
        //    int quantity, bool forceredirection = false)
        //{

        //    var cartType = (ShoppingCartType)shoppingCartTypeId;

        //    var product = _productService.GetProductById(productId);
        //    if (product == null)
        //        //no product found
        //        return BadRequest("No product found with the specified ID");


        //    //we can add only simple products
        //    if (product.ProductType != ProductType.SimpleProduct)
        //    {
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    //products with "minimum order quantity" more than a specified qty
        //    if (product.OrderMinimumQuantity > quantity)
        //    {
        //        //we cannot add to the cart such products from category pages
        //        //it can confuse customers. That's why we redirect customers to the product details page
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    if (product.CustomerEntersPrice)
        //    {
        //        //cannot be added to the cart (requires a customer to enter price)
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    if (product.IsRental)
        //    {
        //        //rental products require start/end dates to be entered
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    var allowedQuantities = product.ParseAllowedQuantities();
        //    if (allowedQuantities.Length > 0)
        //    {
        //        //cannot be added to the cart (requires a customer to select a quantity from dropdownlist)
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    if (product.ProductAttributeMappings.Count > 0)
        //    {
        //        //product has some attributes. let a customer see them
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    //get standard warnings without attribute validations
        //    //first, try to find existing shopping cart item
        //    var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //        .Where(sci => sci.ShoppingCartType == cartType)
        //        .LimitPerStore(_storeContext.CurrentStore.Id)
        //        .ToList();
        //    var shoppingCartItem = _shoppingCartService.FindShoppingCartItemInTheCart(cart, cartType, product);
        //    //if we already have the same product in the cart, then use the total quantity to validate
        //    var quantityToValidate = shoppingCartItem != null ? shoppingCartItem.Quantity + quantity : quantity;
        //    var addToCartWarnings = _shoppingCartService
        //        .GetShoppingCartItemWarnings(_workContext.CurrentCustomer, cartType,
        //        product, _storeContext.CurrentStore.Id, string.Empty,
        //        decimal.Zero, null, null, quantityToValidate, false, true, false, false, false);
        //    if (addToCartWarnings.Count > 0)
        //    {
        //        //cannot be added to the cart
        //        //let's display standard warnings
        //        return (new CustomJsonResult()
        //        {
        //            success = false,
        //            message = addToCartWarnings.ToArray().ToString()
        //        });
        //    }

        //    //now let's try adding product to the cart (now including product attribute validation, etc)
        //    addToCartWarnings = _shoppingCartService.AddToCart(customer: _workContext.CurrentCustomer,
        //        product: product,
        //        shoppingCartType: cartType,
        //        storeId: _storeContext.CurrentStore.Id,
        //        quantity: quantity);
        //    if (addToCartWarnings.Count > 0)
        //    {
        //        //cannot be added to the cart
        //        //but we do not display attribute and gift card warnings here. let's do it on the product details page
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("Product", new { SeName = product.GetSeName() }),
        //        //});
        //    }

        //    //added to the cart/wishlist
        //    switch (cartType)
        //    {
        //        case ShoppingCartType.Wishlist:
        //            {
        //                //activity log
        //                _customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

        //                if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct || forceredirection)
        //                {
        //                    //redirect to the wishlist page
        //                    //return (new CustomJsonResult()
        //                    //{
        //                    //    redirect = Url.RouteUrl("Wishlist"),
        //                    //});
        //                }

        //                //display notification message and update appropriate blocks
        //                var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
        //                _workContext.CurrentCustomer.ShoppingCartItems
        //                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //                .LimitPerStore(_storeContext.CurrentStore.Id)
        //                .ToList()
        //                .GetTotalProducts());
        //                //return (new CustomJsonResult()
        //                //{
        //                //    success = true,
        //                //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
        //                //    updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
        //                //});
        //                return Ok();
        //            }
        //        case ShoppingCartType.ShoppingCart:
        //        default:
        //            {
        //                //activity log
        //                _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

        //                if (_shoppingCartSettings.DisplayCartAfterAddingProduct || forceredirection)
        //                {
        //                    //redirect to the shopping cart page
        //                    //return (new CustomJsonResult()
        //                    //{
        //                    //    redirect = Url.RouteUrl("ShoppingCart"),
        //                    //});
        //                }

        //                //display notification message and update appropriate blocks
        //                var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
        //                _workContext.CurrentCustomer.ShoppingCartItems
        //                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //                .LimitPerStore(_storeContext.CurrentStore.Id)
        //                .ToList()
        //                .GetTotalProducts());

        //                //var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
        //                //    ? this.RenderPartialViewToString("FlyoutShoppingCart", PrepareMiniShoppingCartModel())
        //                //    : "";

        //                //return (new CustomJsonResult()
        //                //{
        //                //    success = true,
        //                //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
        //                //    updatetopcartsectionhtml = updatetopcartsectionhtml,
        //                //    updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
        //                //});
        //                return Ok();
        //            }
        //    }
        //}

        //add product to cart using AJAX
        //currently we use this method on the product details pages

        // POST api/NopMobileWebApi/AddProductToCart_Details
        //[HttpPost]
        //[Route("AddProductToCart_Details")]
        //public IHttpActionResult AddProductToCart_Details(int productId, int shoppingCartTypeId, System.Web.Mvc.FormCollection form)
        //{
        //    var product = _productService.GetProductById(productId);
        //    if (product == null)
        //    {
        //        //return (new CustomJsonResult()
        //        //{
        //        //    redirect = Url.RouteUrl("HomePage"),
        //        //});
        //        return BadRequest("No Product Foung");
        //    }

        //    //we can add only simple products
        //    if (product.ProductType != ProductType.SimpleProduct)
        //    {
        //        //return (new CustomJsonResult()
        //        //{
        //        //    success = false,
        //        //    message = "Only simple products could be added to the cart"
        //        //});
        //        return BadRequest("Only simple products could be added to the cart");
        //    }

        //    #region Update existing shopping cart item?
        //    int updatecartitemid = 0;
        //    foreach (string formKey in form.AllKeys)
        //        if (formKey.Equals(string.Format("addtocart_{0}.UpdatedShoppingCartItemId", productId), StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            int.TryParse(form[formKey], out updatecartitemid);
        //            break;
        //        }
        //    ShoppingCartItem updatecartitem = null;
        //    if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
        //    {
        //        var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //            .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //            .LimitPerStore(_storeContext.CurrentStore.Id)
        //            .ToList();
        //        updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
        //        //not found?
        //        if (updatecartitem == null)
        //        {
        //            return (new CustomJsonResult()
        //            {
        //                success = false,
        //                message = "No shopping cart item found to update"
        //            });
        //        }
        //        //is it this product?
        //        if (product.Id != updatecartitem.ProductId)
        //        {
        //            return (new CustomJsonResult()
        //            {
        //                success = false,
        //                message = "This product does not match a passed shopping cart item identifier"
        //            });
        //        }
        //    }
        //    #endregion

        //    #region Customer entered price
        //    decimal customerEnteredPriceConverted = decimal.Zero;
        //    if (product.CustomerEntersPrice)
        //    {
        //        foreach (string formKey in form.AllKeys)
        //        {
        //            if (formKey.Equals(string.Format("addtocart_{0}.CustomerEnteredPrice", productId), StringComparison.InvariantCultureIgnoreCase))
        //            {
        //                decimal customerEnteredPrice;
        //                if (decimal.TryParse(form[formKey], out customerEnteredPrice))
        //                    customerEnteredPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(customerEnteredPrice, _workContext.WorkingCurrency);
        //                break;
        //            }
        //        }
        //    }
        //    #endregion

        //    #region Quantity

        //    int quantity = 1;
        //    foreach (string formKey in form.AllKeys)
        //        if (formKey.Equals(string.Format("addtocart_{0}.EnteredQuantity", productId), StringComparison.InvariantCultureIgnoreCase))
        //        {
        //            int.TryParse(form[formKey], out quantity);
        //            break;
        //        }

        //    #endregion

        //    //product and gift card attributes
        //    string attributes = ParseProductAttributes(product, form);

        //    //rental attributes
        //    DateTime? rentalStartDate = null;
        //    DateTime? rentalEndDate = null;
        //    if (product.IsRental)
        //    {
        //        ParseRentalDates(product, form, out rentalStartDate, out rentalEndDate);
        //    }

        //    //save item
        //    var addToCartWarnings = new List<string>();
        //    var cartType = (ShoppingCartType)shoppingCartTypeId;
        //    if (updatecartitem == null)
        //    {
        //        //add to the cart
        //        addToCartWarnings.AddRange(_shoppingCartService.AddToCart(_workContext.CurrentCustomer,
        //            product, cartType, _storeContext.CurrentStore.Id,
        //            attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate, quantity, true));
        //    }
        //    else
        //    {
        //        var cart = _workContext.CurrentCustomer.ShoppingCartItems
        //            .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //            .LimitPerStore(_storeContext.CurrentStore.Id)
        //            .ToList();
        //        var otherCartItemWithSameParameters = _shoppingCartService.FindShoppingCartItemInTheCart(
        //            cart, cartType, product, attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate);
        //        if (otherCartItemWithSameParameters != null &&
        //            otherCartItemWithSameParameters.Id == updatecartitem.Id)
        //        {
        //            //ensure it's other shopping cart cart item
        //            otherCartItemWithSameParameters = null;
        //        }
        //        //update existing item
        //        addToCartWarnings.AddRange(_shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
        //            updatecartitem.Id, attributes, customerEnteredPriceConverted,
        //            rentalStartDate, rentalEndDate, quantity, true));
        //        if (otherCartItemWithSameParameters != null && addToCartWarnings.Count == 0)
        //        {
        //            //delete the same shopping cart item (the other one)
        //            _shoppingCartService.DeleteShoppingCartItem(otherCartItemWithSameParameters);
        //        }
        //    }

        //    #region Return result

        //    if (addToCartWarnings.Count > 0)
        //    {
        //        //cannot be added to the cart/wishlist
        //        //let's display warnings
        //        //return (new CustomJsonResult()
        //        //{
        //        //    success = false,
        //        //    message = addToCartWarnings.ToArray()
        //        //});
        //        return BadRequest("Error");
        //    }

        //    //added to the cart/wishlist
        //    switch (cartType)
        //    {
        //        case ShoppingCartType.Wishlist:
        //            {
        //                //activity log
        //                _customerActivityService.InsertActivity("PublicStore.AddToWishlist", _localizationService.GetResource("ActivityLog.PublicStore.AddToWishlist"), product.Name);

        //                //if (_shoppingCartSettings.DisplayWishlistAfterAddingProduct)
        //                //{
        //                //    //redirect to the wishlist page
        //                //    return (new CustomJsonResult()
        //                //    {
        //                //        redirect = Url.RouteUrl("Wishlist"),
        //                //    });
        //                //}

        //                //display notification message and update appropriate blocks
        //                var updatetopwishlistsectionhtml = string.Format(_localizationService.GetResource("Wishlist.HeaderQuantity"),
        //                _workContext.CurrentCustomer.ShoppingCartItems
        //                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
        //                .LimitPerStore(_storeContext.CurrentStore.Id)
        //                .ToList()
        //                .GetTotalProducts());

        //                return Ok();

        //                //return (new CustomJsonResult()
        //                //{
        //                //    success = true,
        //                //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheWishlist.Link"), Url.RouteUrl("Wishlist")),
        //                //    updatetopwishlistsectionhtml = updatetopwishlistsectionhtml,
        //                //});
        //            }
        //        case ShoppingCartType.ShoppingCart:
        //        default:
        //            {
        //                //activity log
        //                _customerActivityService.InsertActivity("PublicStore.AddToShoppingCart", _localizationService.GetResource("ActivityLog.PublicStore.AddToShoppingCart"), product.Name);

        //                //if (_shoppingCartSettings.DisplayCartAfterAddingProduct)
        //                //{
        //                //    //redirect to the shopping cart page
        //                //    return (new CustomJsonResult()
        //                //    {
        //                //        redirect = Url.RouteUrl("ShoppingCart"),
        //                //    });
        //                //}

        //                //display notification message and update appropriate blocks
        //                var updatetopcartsectionhtml = string.Format(_localizationService.GetResource("ShoppingCart.HeaderQuantity"),
        //                _workContext.CurrentCustomer.ShoppingCartItems
        //                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
        //                .LimitPerStore(_storeContext.CurrentStore.Id)
        //                .ToList()
        //                .GetTotalProducts());

        //                return Ok();

        //                //var updateflyoutcartsectionhtml = _shoppingCartSettings.MiniShoppingCartEnabled
        //                //    ? this.RenderPartialViewToString("FlyoutShoppingCart", PrepareMiniShoppingCartModel())
        //                //    : "";

        //                //return (new CustomJsonResult()
        //                //{
        //                //    success = true,
        //                //    message = string.Format(_localizationService.GetResource("Products.ProductHasBeenAddedToTheCart.Link"), Url.RouteUrl("ShoppingCart")),
        //                //    updatetopcartsectionhtml = updatetopcartsectionhtml,
        //                //    updateflyoutcartsectionhtml = updateflyoutcartsectionhtml
        //                //});
        //            }
        //    }


        //    #endregion
        //}
        #endregion

        #region Get Methods

        [HttpGet]
        public TopMenuModel TopMenu()
        {
            //categories
            var cachedCategoriesModel = PrepareCategorySimpleModels(0);

            //top menu topics
            var cachedTopicModel = _topicService.GetAllTopics(_storeContext.CurrentStore.Id).Where(t => t.IncludeInTopMenu)
                .Select(t => new TopMenuModel.TopMenuTopicModel
                {
                    Id = t.Id,
                    Name = t.GetLocalized(x => x.Title),
                    SeName = t.GetSeName()
                }).ToList();
            var model = new TopMenuModel
            {
                Categories = cachedCategoriesModel,
                Topics = cachedTopicModel,
                RecentlyAddedProductsEnabled = _catalogSettings.NewProductsEnabled,
                BlogEnabled = _blogSettings.Enabled,
                ForumEnabled = _forumSettings.ForumsEnabled
            };
            return model;
        }

        [HttpGet]
        public LanguageSelectorModel GetLanguageSelector()
        {
            var result = _languageService
                .GetAllLanguages(storeId: _storeContext.CurrentStore.Id)
                .Select(x => new LanguageModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    FlagImageFileName = x.FlagImageFileName
                })
                .ToList();

            var model = new LanguageSelectorModel
            {
                CurrentLanguageId = _workContext.WorkingLanguage.Id,
                AvailableLanguages = result,
                UseImages = _localizationSettings.UseImagesForLanguageSelection
            };

            var currentLanguage = new LanguageModel()
            {
                Id = _workContext.WorkingLanguage.Id,
                Name = _workContext.WorkingLanguage.Name,
                FlagImageFileName = _workContext.WorkingLanguage.FlagImageFileName,
               
            };
           // model.CurrentLanguage = currentLanguage;

            return model;
        }

        [HttpGet]
        public CurrencySelectorModel CurrencySelector()
        {
            var result = _currencyService
                .GetAllCurrencies(storeId: _storeContext.CurrentStore.Id)
                .Select(x =>
                {
                    //currency char
                    var currencySymbol = "";
                    if (!string.IsNullOrEmpty(x.DisplayLocale))
                        currencySymbol = new RegionInfo(x.DisplayLocale).CurrencySymbol;
                    else
                        currencySymbol = x.CurrencyCode;
                    //model
                    var currencyModel = new CurrencyModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        CurrencySymbol = currencySymbol
                    };
                    return currencyModel;
                })
                .ToList();


            var model = new CurrencySelectorModel
            {
                CurrentCurrencyId = _workContext.WorkingCurrency.Id,
                AvailableCurrencies = result
            };

            var currentCurrency = new CurrencyModel();

            var symbol = "";
            if (!string.IsNullOrEmpty(_workContext.WorkingCurrency.DisplayLocale))
                symbol = new RegionInfo(_workContext.WorkingCurrency.DisplayLocale).CurrencySymbol;
            else
                symbol = _workContext.WorkingCurrency.CurrencyCode;

            currentCurrency.Id = _workContext.WorkingCurrency.Id;
            currentCurrency.Name = _workContext.WorkingCurrency.Name;
            currentCurrency.CurrencySymbol = symbol;

           // model.CurrentCurrency = currentCurrency;

            return model;
        }

        [HttpGet]
        public MiniShoppingCartModel FlyoutShoppingCart()
        {
            if (!_shoppingCartSettings.MiniShoppingCartEnabled)
            {
                var Model = new MiniShoppingCartModel();
                return Model;
            }

            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
            {
                var Model = new MiniShoppingCartModel();
                return Model;
            }
            var model = PrepareMiniShoppingCartModel();
            return model;
        }

        [HttpGet]
        public IList<CategoryModel> HomepageCategories()
        {
            var model = _categoryService.GetAllCategoriesDisplayedOnHomePage()
                .Select(x =>
                {
                    var catModel = x.ToCModel();

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;


                    var picture = _pictureService.GetPictureById(x.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), catModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), catModel.Name)
                    };
                    catModel.PictureModel = pictureModel;
                    var categoryIds = new List<int>();
                    categoryIds.Add(x.Id);
                    if (_catalogSettings.ShowProductsFromSubcategories)
                    {
                        //include subcategories
                        categoryIds.AddRange(GetChildCategoryIds(x.Id));
                    }
                    IList<int> filterableSpecificationAttributeOptionIds;
                    var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: null,
                priceMax: null,
                filteredSpecs: null,
                orderBy: ProductSortingEnum.NameAsc,
                pageIndex: 0,
                pageSize: int.MaxValue);
                    catModel.Products = PrepareProductOverviewModels(products, true, true, null, false, false, x.Id).ToList();

                    return catModel;
                }).ToList();
            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> HomepageBestSellers(int? productThumbPictureSize = null)
        {
            if (!_catalogSettings.ShowBestsellersOnHomepage || _catalogSettings.NumberOfBestsellersOnHomepage == 0)
            {
                var model = new List<ProductOverviewModel>();
                return model;
            }
            //load and cache report
            var report =
                    _orderReportService.BestSellersReport(storeId: _storeContext.CurrentStore.Id,
                    pageSize: _catalogSettings.NumberOfBestsellersOnHomepage);


            //load products
            var products = _productService.GetProductsByIds(report.Select(x => x.ProductId).ToArray());
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            //prepare model
            var pModel = PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return pModel;
        }

        [HttpGet]
        public IList<ProductOverviewModel> RecentlyAddedProducts()
        {
            var model = new List<ProductOverviewModel>();
            if (!_catalogSettings.NewProductsEnabled)
                return model;

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: _catalogSettings.NewProductsNumber);


            model.AddRange(PrepareProductOverviewModels(products));

            return model;
        }

        //GetAllProductReviews
        [HttpGet]
        public IList<ProductOverviewModel> TopRatedProducts()
        {
            var model = new List<ProductOverviewModel>();

            var products = _productReviewRepository.Table.Select(p => p.Product).Where(p => p.ApprovedTotalReviews > 0).OrderByDescending(p => (p.ApprovedRatingSum / p.ApprovedTotalReviews)).Take(5).ToList();
            model.AddRange(PrepareProductOverviewModels(products));

            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> HomepageProducts(int? productThumbPictureSize)
        {
            var products = _productService.GetAllProductsDisplayedOnHomePage();
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var model = PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return model;
        }

        [HttpGet]
        public IList<ManufacturerModel> ManufacturerAll()
        {
            var model = new List<ManufacturerModel>();
            var manufacturers = _manufacturerService.GetAllManufacturers();
            foreach (var manufacturer in manufacturers)
            {
                var modelMan = ToModel(manufacturer);
                //prepare picture model
                int pictureSize = _mediaSettings.ManufacturerThumbPictureSize;
                var picture = _pictureService.GetPictureById(manufacturer.PictureId);
                var pictureModel = new PictureModel
                {
                    FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                    ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                    Title = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageLinkTitleFormat"), modelMan.Name),
                    AlternateText = string.Format(_localizationService.GetResource("Media.Manufacturer.ImageAlternateTextFormat"), modelMan.Name)
                };
                modelMan.PictureModel = pictureModel;
                model.Add(modelMan);
            }
            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> RecentlyViewedProducts()
        {
            if (!_catalogSettings.RecentlyViewedProductsEnabled)
            {
                var model = new List<ProductOverviewModel>();
                return model;
            }

            var products = _recentlyViewedProductsService.GetRecentlyViewedProducts(_catalogSettings.RecentlyViewedProductsNumber);
            var pModel = new List<ProductOverviewModel>();
            pModel.AddRange(PrepareProductOverviewModels(products));
            return pModel;
        }

        [HttpGet]
        public CompareProductsModel CompareProducts()
        {
            if (!_catalogSettings.CompareProductsEnabled)
            {
                var compModel = new CompareProductsModel();
                return compModel;
            }

            var model = new CompareProductsModel
            {
                IncludeShortDescriptionInCompareProducts = _catalogSettings.IncludeShortDescriptionInCompareProducts,
                IncludeFullDescriptionInCompareProducts = _catalogSettings.IncludeFullDescriptionInCompareProducts,
            };

            var products = _compareProductsService.GetComparedProducts();

            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            //prepare model
            PrepareProductOverviewModels(products, prepareSpecificationAttributes: true)
                .ToList()
                .ForEach(model.Products.Add);
            return model;
        }

        [HttpGet]
        public SliderModel HomepageSlider()
        {

            var storeScope = GetActiveStoreScopeConfiguration();
            var BsNopMobileSettings = _settingService.LoadSetting<MobileWebApiSettings>(storeScope);

            var model = new SliderModel();
            model.Picture1Url = GetPictureUrl(BsNopMobileSettings.Picture1Id);
            model.Text1 = BsNopMobileSettings.Text1;
            model.Link1 = BsNopMobileSettings.Link1;

            model.Picture2Url = GetPictureUrl(BsNopMobileSettings.Picture2Id);
            model.Text2 = BsNopMobileSettings.Text2;
            model.Link2 = BsNopMobileSettings.Link2;

            model.Picture3Url = GetPictureUrl(BsNopMobileSettings.Picture3Id);
            model.Text3 = BsNopMobileSettings.Text3;
            model.Link3 = BsNopMobileSettings.Link3;

            model.Picture4Url = GetPictureUrl(BsNopMobileSettings.Picture4Id);
            model.Text4 = BsNopMobileSettings.Text4;
            model.Link4 = BsNopMobileSettings.Link4;

            model.Picture5Url = GetPictureUrl(BsNopMobileSettings.Picture5Id);
            model.Text5 = BsNopMobileSettings.Text5;
            model.Link5 = BsNopMobileSettings.Link5;

            if (string.IsNullOrEmpty(model.Picture1Url) && string.IsNullOrEmpty(model.Picture2Url) &&
                string.IsNullOrEmpty(model.Picture3Url) && string.IsNullOrEmpty(model.Picture4Url) &&
                string.IsNullOrEmpty(model.Picture5Url))
            //no pictures uploaded
            {
                return model = null;
            }


            return model;
        }

        [HttpGet]
        public BlogPostListModel BlogList(BlogPagingFilteringModel command)
        {
            if (!_blogSettings.Enabled)
            {
                var blogPostListModel = new BlogPostListModel();
                return blogPostListModel;
            }

            var model = PrepareBlogPostListModel(command);
            return model;
        }

        [HttpGet]
        public BlogPostModel BlogPost(int blogPostId)
        {
            if (!_blogSettings.Enabled)
            {
                var blogPostModel = new BlogPostModel();
                return blogPostModel;
            }

            var blogPost = _blogService.GetBlogPostById(blogPostId);
            if (blogPost == null ||
                (blogPost.StartDateUtc.HasValue && blogPost.StartDateUtc.Value >= DateTime.UtcNow) ||
                (blogPost.EndDateUtc.HasValue && blogPost.EndDateUtc.Value <= DateTime.UtcNow))
            {
                var blogPostModel = new BlogPostModel();
                return blogPostModel;
            }

            //Store mapping
            if (!_storeMappingService.Authorize(blogPost))
            {
                var blogPostModel = new BlogPostModel();
                return blogPostModel;
            }

            var model = new BlogPostModel();
            PrepareBlogPostModel(model, blogPost, true);

            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> RelatedProducts(int productId, int? productThumbPictureSize)
        {
            //load and cache report
            var productIds = _productService.GetRelatedProductsByProductId1(productId).Select(x => x.ProductId2).ToArray();
            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            var model = PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();
            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> ProductsAlsoPurchased(int productId, int? productThumbPictureSize)
        {
            if (!_catalogSettings.ProductsAlsoPurchasedEnabled)
            {
                var pAPModel = new List<ProductOverviewModel>();
                return pAPModel;
            }

            //load and cache report
            var productIds = _orderReportService.GetAlsoPurchasedProductsIds(_storeContext.CurrentStore.Id, productId, _catalogSettings.ProductsAlsoPurchasedNumber);

            //load products
            var products = _productService.GetProductsByIds(productIds);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (products.Count == 0)
            {
                var pAPModel = new List<ProductOverviewModel>();
                return pAPModel;
            }

            //prepare model
            var model = PrepareProductOverviewModels(products, true, true, productThumbPictureSize).ToList();

            return model;
        }

        [HttpGet]
        public IList<ProductOverviewModel> CrossSellProducts(int? productThumbPictureSize)
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            var products = _productService.GetCrosssellProductsByShoppingCart(cart, _shoppingCartSettings.CrossSellsNumber);
            //ACL and store mapping
            products = products.Where(p => _aclService.Authorize(p) && _storeMappingService.Authorize(p)).ToList();
            //availability dates
            products = products.Where(p => p.IsAvailable()).ToList();

            if (products.Count == 0)
            {
                var cSPModel = new List<ProductOverviewModel>();
                return cSPModel;
            }

            //Cross-sell products are dispalyed on the shopping cart page.
            //We know that the entire shopping cart page is not refresh
            //even if "ShoppingCartSettings.DisplayCartAfterAddingProduct" setting  is enabled.
            //That's why we force page refresh (redirect) in this case
            var model = PrepareProductOverviewModels(products,
                productThumbPictureSize: productThumbPictureSize, forceRedirectionAfterAddingToCart: true)
                .ToList();

            return model;
        }

        [HttpGet]
        public ProductDetailsModel ProductDetails(int productId, int updatecartitemid = 0)
        {
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted)
            {
                var productModel = new ProductDetailsModel();
                var responceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "No product found",
                    Success = false
                };
                productModel.ResponceModel = responceModel;
                return productModel;
            }

            //Is published?
            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a product before publishing
            if (!product.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageProducts))
            {
                var productModel = new ProductDetailsModel();
                var responceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "No permission to view product",
                    Success = false
                };
                productModel.ResponceModel = responceModel;
                return productModel;
            }

            //ACL (access control list)
            if (!_aclService.Authorize(product))
            {
                var productModel = new ProductDetailsModel();
                var responceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "You are not authorize",
                    Success = false
                };
                productModel.ResponceModel = responceModel;
                return productModel;
            }

            //Store mapping
            if (!_storeMappingService.Authorize(product))
            {
                var productModel = new ProductDetailsModel();
                var responceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "No product found",
                    Success = false
                };
                productModel.ResponceModel = responceModel;
                return productModel;
            }

            //availability dates
            if (!product.IsAvailable())
            {
                var productModel = new ProductDetailsModel();
                var responceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "Product is not available",
                    Success = false
                };
                productModel.ResponceModel = responceModel;
                return productModel;
            }

            //visible individually?
            if (!product.VisibleIndividually)
            {
                //is this one an associated products?
                var parentGroupedProduct = _productService.GetProductById(product.ParentGroupedProductId);
                if (parentGroupedProduct == null)
                {
                    var productModel = new ProductDetailsModel();
                    var responceModel = new ProductDetailsModel.ProductResponceModel()
                    {
                        ErrorMessage = ErroMessageType.InvokeHttp404,
                        Message = "",
                        Success = false
                    };
                    productModel.ResponceModel = responceModel;
                    return productModel;
                }

                var pDModel = new ProductDetailsModel();
                var pDresponceModel = new ProductDetailsModel.ProductResponceModel()
                {
                    ErrorMessage = ErroMessageType.InvokeHttp404,
                    Message = "",
                    Success = false
                };
                pDModel.ResponceModel = pDresponceModel;
                return pDModel;
            }

            //update existing shopping cart item?
            ShoppingCartItem updatecartitem = null;
            if (_shoppingCartSettings.AllowCartItemEditing && updatecartitemid > 0)
            {
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(x => x.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                updatecartitem = cart.FirstOrDefault(x => x.Id == updatecartitemid);
                //not found?
                if (updatecartitem == null)
                {
                    var productModel = new ProductDetailsModel();
                    var responceModel = new ProductDetailsModel.ProductResponceModel()
                    {
                        ErrorMessage = ErroMessageType.InvokeHttp404,
                        Message = "",
                        Success = false
                    };
                    productModel.ResponceModel = responceModel;
                    return productModel;
                }
                //is it this product?
                if (product.Id != updatecartitem.ProductId)
                {
                    var productModel = new ProductDetailsModel();
                    var responceModel = new ProductDetailsModel.ProductResponceModel()
                    {
                        ErrorMessage = ErroMessageType.InvokeHttp404,
                        Message = "",
                        Success = false
                    };
                    productModel.ResponceModel = responceModel;
                    return productModel;
                }
            }

            //prepare the model
            var model = PrepareProductDetailsPageModel(product, updatecartitem, false);

            //save as recently viewed
            _recentlyViewedProductsService.AddProductToRecentlyViewedList(product.Id);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewProduct", _localizationService.GetResource("ActivityLog.PublicStore.ViewProduct"), product.Name);

            return model;
        }

        [HttpGet]
        public ShoppingCartModel Cart()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                //return RedirectToRoute("HomePage");
                return null;

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            model.OrderTotals = PrepareOrderTotalsModel(cart, model.IsEditable);
            return model;
        }

        [HttpGet]
        public WishlistModel Wishlist(Guid? customerGuid)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableWishlist))
                return null;

            Customer customer = customerGuid.HasValue ?
                _customerService.GetCustomerByGuid(customerGuid.Value)
                : _workContext.CurrentCustomer;
            if (customer == null)
                return null;
            var cart = customer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.Wishlist)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new WishlistModel();
            PrepareWishlistModel(model, cart, !customerGuid.HasValue);
            return model;
        }

        [HttpGet]
        public CategoryModel CategoryDetail(int categoryId, CatalogPagingFilteringModel command)
        {
            if (command == null)
                command = new CatalogPagingFilteringModel();
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return null;

            //Check whether the current user has a "Manage catalog" permission
            //It allows him to preview a category before publishing
            if (!category.Published && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return null;

            //ACL (access control list)
            if (!_aclService.Authorize(category))
                return null;

            //Store mapping
            if (!_storeMappingService.Authorize(category))
                return null;

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            var model = category.ToCModel();

            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                category.AllowCustomersToSelectPageSize,
                category.PageSizeOptions,
                category.PageSize);

            //price ranges
            model.PagingFilteringContext.PriceRangeFilter.LoadPriceRangeFilters(category.PriceRanges, _webHelper, _priceFormatter);
            var selectedPriceRange = model.PagingFilteringContext.PriceRangeFilter.GetSelectedPriceRange(_webHelper, category.PriceRanges);
            decimal? minPriceConverted = null;
            decimal? maxPriceConverted = null;
            if (selectedPriceRange != null)
            {
                if (selectedPriceRange.From.HasValue)
                    minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.From.Value, _workContext.WorkingCurrency);

                if (selectedPriceRange.To.HasValue)
                    maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(selectedPriceRange.To.Value, _workContext.WorkingCurrency);
            }


            //category breadcrumb
            if (_catalogSettings.CategoryBreadcrumbEnabled)
            {
                model.DisplayCategoryBreadcrumb = true;

                model.CategoryBreadcrumb = category
                                            .GetCategoryBreadCrumb(_categoryService, _aclService, _storeMappingService)
                                            .Select(catBr => new CategoryModel
                                            {
                                                Id = catBr.Id,
                                                Name = catBr.GetLocalized(x => x.Name),
                                                SeName = catBr.GetSeName()
                                            })
                                            .ToList();
            }
           
            model.SubCategories = _categoryService.GetAllCategoriesByParentCategoryId(categoryId)
                .Select(x =>
                {
                    var subCatModel = new CategoryModel.SubCategoryModel
                    {
                        Id = x.Id,
                        Name = x.GetLocalized(y => y.Name),
                        SeName = x.GetSeName(),
                    };

                    //prepare picture model
                    int pictureSize = _mediaSettings.CategoryThumbPictureSize;
                    var picture = _pictureService.GetPictureById(x.PictureId);
                    var pictureModel = new PictureModel
                    {
                        FullSizeImageUrl = _pictureService.GetPictureUrl(picture),
                        ImageUrl = _pictureService.GetPictureUrl(picture, pictureSize),
                        Title = string.Format(_localizationService.GetResource("Media.Category.ImageLinkTitleFormat"), subCatModel.Name),
                        AlternateText = string.Format(_localizationService.GetResource("Media.Category.ImageAlternateTextFormat"), subCatModel.Name)
                    };
                    //var categoryPictureCacheKey = string.Format(ModelCacheEventConsumer.CATEGORY_PICTURE_MODEL_KEY, x.Id, pictureSize, true, _workContext.WorkingLanguage.Id, _webHelper.IsCurrentConnectionSecured(), _storeContext.CurrentStore.Id);
                    subCatModel.PictureModel = pictureModel;

                    return subCatModel;
                })
                .ToList();

            //featured products
            if (!_catalogSettings.IgnoreFeaturedProducts)
            {
                //We cache a value indicating whether we have featured products
                IPagedList<Product> featuredProducts = null;
                featuredProducts = _productService.SearchProducts(
                       categoryIds: new List<int> { category.Id },
                       storeId: _storeContext.CurrentStore.Id,
                       visibleIndividuallyOnly: true,
                       featuredProducts: true);
             
                if (featuredProducts != null)
                {
                    model.FeaturedProducts = PrepareProductOverviewModels(featuredProducts).ToList();
                }
            }


            var categoryIds = new List<int>();
            categoryIds.Add(category.Id);
            if (_catalogSettings.ShowProductsFromSubcategories)
            {
                categoryIds.AddRange(GetChildCategoryIds(category.Id));
            }

            //products
            IList<int> alreadyFilteredSpecOptionIds = model.PagingFilteringContext.SpecificationFilter.GetAlreadyFilteredSpecOptionIds(_webHelper);
            IList<int> filterableSpecificationAttributeOptionIds;
            var products = _productService.SearchProducts(out filterableSpecificationAttributeOptionIds, true,
                categoryIds: categoryIds,
                storeId: _storeContext.CurrentStore.Id,
                visibleIndividuallyOnly: true,
                featuredProducts: _catalogSettings.IncludeFeaturedProductsInNormalLists ? null : (bool?)false,
                priceMin: minPriceConverted,
                priceMax: maxPriceConverted,
                filteredSpecs: alreadyFilteredSpecOptionIds,
                orderBy: (ProductSortingEnum)command.OrderBy,
                pageIndex: command.PageNumber - 1,
                pageSize: command.PageSize);
            model.Products = PrepareProductOverviewModels(products).ToList();

            model.PagingFilteringContext.LoadPagedList(products);

            //specs
            model.PagingFilteringContext.SpecificationFilter.PrepareSpecsFilters(alreadyFilteredSpecOptionIds,
                filterableSpecificationAttributeOptionIds,
                _specificationAttributeService, _webHelper, _workContext);


            //template
            var template = _categoryTemplateService.GetCategoryTemplateById(category.CategoryTemplateId);
            if (template == null)
                template = _categoryTemplateService.GetAllCategoryTemplates().FirstOrDefault();
            if (template == null)
                return null;
            var templateViewPath = template.ViewPath;

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewCategory", _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            return model;
        }

        [HttpGet]
        public CategoryNavigationModel CategoryNavigation(int currentCategoryId, int currentProductId)
        {
            //get active category
            int activeCategoryId = 0;
            if (currentCategoryId > 0)
            {
                //category details page
                activeCategoryId = currentCategoryId;
            }
            else if (currentProductId > 0)
            {
                //product details page
                var productCategories = _categoryService.GetProductCategoriesByProductId(currentProductId);
                if (productCategories.Count > 0)
                    activeCategoryId = productCategories[0].CategoryId;
            }

            var catmodel = PrepareCategorySimpleModels(0).ToList();

            var model = new CategoryNavigationModel
            {
                CurrentCategoryId = activeCategoryId,
                Categories = catmodel
            };

            return model;
        }

        [HttpGet]
        public CustomerInfoModel Info()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerInfoModel();
            PrepareCustomerInfoModel(model, customer, false);

            return model;
        }

        [HttpGet]
        public CustomerDownloadableProductsModel DownloadableProducts()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerDownloadableProductsModel();

            //var items = _orderService.GetAllOrderItems(null, customer.Id, null, null,
            //    null, null, null, true);  
            var items = _orderService.GetDownloadableOrderItems(customer.Id); //change3.8
            foreach (var item in items)
            {
                var itemModel = new CustomerDownloadableProductsModel.DownloadableProductsModel
                {
                    OrderItemGuid = item.OrderItemGuid,
                    OrderId = item.OrderId,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(item.Order.CreatedOnUtc, DateTimeKind.Utc),
                    ProductName = item.Product.GetLocalized(x => x.Name),
                    ProductSeName = item.Product.GetSeName(),
                    ProductAttributes = item.AttributeDescription,
                    ProductId = item.ProductId
                };
                model.Items.Add(itemModel);

                if (_downloadService.IsDownloadAllowed(item))
                    itemModel.DownloadId = item.Product.DownloadId;

                if (_downloadService.IsLicenseDownloadAllowed(item))
                    itemModel.LicenseId = item.LicenseDownloadId.HasValue ? item.LicenseDownloadId.Value : 0;
            }

            return model;
        }

        [HttpGet]
        public UserAgreementModel UserAgreement(Guid orderItemId)
        {
            var orderItem = _orderService.GetOrderItemByGuid(orderItemId);
            if (orderItem == null)
                return null;

            var product = orderItem.Product;
            if (product == null || !product.HasUserAgreement)
                return null;

            var model = new UserAgreementModel();
            model.UserAgreementText = product.UserAgreementText;
            model.OrderItemGuid = orderItemId;

            return model;
        }

        [HttpGet]
        public ChangePasswordModel ChangePassword()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var model = new ChangePasswordModel();
            return model;
        }

        [HttpGet]
        public CustomerAddressListModel Addresses()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerAddressListModel();
            var addresses = customer.Addresses
                //enabled for the current store
                .Where(a => a.Country == null || _storeMappingService.Authorize(a.Country))
                .ToList();
            foreach (var address in addresses)
            {
                var addressModel = new AddressModel();
                addressModel.PrepareModelApi(
                    address: address,
                    excludeProperties: false,
                    addressSettings: _addressSettings,
                    localizationService: _localizationService,
                    stateProvinceService: _stateProvinceService,
                    addressAttributeFormatter: _addressAttributeFormatter,
                    loadCountries: () => _countryService.GetAllCountries());
                model.Addresses.Add(addressModel);
            }
            return model;
        }

        [HttpGet]
        public CustomerAddressEditModel AddressAdd()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var model = new CustomerAddressEditModel();
            model.Address.PrepareModelApi(
                address: null,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());

            return model;
        }

        [HttpGet]
        public CustomerAddressEditModel AddressEdit(int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
                return null;

            var model = new CustomerAddressEditModel();
            model.Address.PrepareModelApi(address: address,
                excludeProperties: false,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());

            return model;
        }

        //My account / Orders
        [HttpGet]
        public CustomerOrderListModel CustomerOrders()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var model = PrepareCustomerOrderListModel();
            return model;
        }

        //My account / Reward points
        [HttpGet]
        public CustomerRewardPointsModel CustomerRewardPoints()
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            if (!_rewardPointsSettings.Enabled)
                return null;

            var customer = _workContext.CurrentCustomer;

            var model = new CustomerRewardPointsModel();
            foreach (var rph in _rewardPointService.GetRewardPointsHistory(customer.Id))
            {
                model.RewardPoints.Add(new CustomerRewardPointsModel.RewardPointsHistoryModel
                {
                    Points = rph.Points,
                    PointsBalance = rph.PointsBalance,
                    Message = rph.Message,
                    CreatedOn = _dateTimeHelper.ConvertToUserTime(rph.CreatedOnUtc, DateTimeKind.Utc)
                });
            }
            //current amount/balance
            int rewardPointsBalance = _rewardPointService.GetRewardPointsBalance(customer.Id, _storeContext.CurrentStore.Id);
            decimal rewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(rewardPointsBalance);
            decimal rewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(rewardPointsAmountBase, _workContext.WorkingCurrency);
            model.RewardPointsBalance = rewardPointsBalance;
            model.RewardPointsAmount = _priceFormatter.FormatPrice(rewardPointsAmount, true, false);
            //minimum amount/balance
            int minimumRewardPointsBalance = _rewardPointsSettings.MinimumRewardPointsToUse;
            decimal minimumRewardPointsAmountBase = _orderTotalCalculationService.ConvertRewardPointsToAmount(minimumRewardPointsBalance);
            decimal minimumRewardPointsAmount = _currencyService.ConvertFromPrimaryStoreCurrency(minimumRewardPointsAmountBase, _workContext.WorkingCurrency);
            model.MinimumRewardPointsBalance = minimumRewardPointsBalance;
            model.MinimumRewardPointsAmount = _priceFormatter.FormatPrice(minimumRewardPointsAmount, true, false);
            return model;
        }

        //My account / Order details page
        [HttpGet]
        public OrderDetailsModel Details(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return null;

            var model = PrepareOrderDetailsModel(order);

            return model;
        }

        //My account / Order details page / Print
        [HttpGet]
        public OrderDetailsModel PrintOrderDetails(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return null;

            var model = PrepareOrderDetailsModel(order);
            model.PrintMode = true;

            return model;
        }

        //My account / Order details page / PDF invoice
        //[HttpGet]
        //public ActionResult GetPdfInvoice(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return null;

        //    var orders = new List<Order>();
        //    orders.Add(order);
        //    byte[] bytes;
        //    using (var stream = new System.IO.MemoryStream())
        //    {
        //        _pdfService.PrintOrdersToPdf(stream, orders, _workContext.WorkingLanguage.Id);
        //        bytes = stream.ToArray();
        //    }
        //    return File(bytes, "application/pdf", string.Format("order_{0}.pdf", order.Id));
        //}

        //My account / Order details page / re-order
        [HttpGet]
        public string ReOrder(int orderId)
        {
            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return null;

            _orderProcessingService.ReOrder(order);
            return "ShoppingCart";
        }

        //My account / Order details page / Complete payment
        //[HttpGet]
        //public ActionResult RePostPayment(int orderId)
        //{
        //    var order = _orderService.GetOrderById(orderId);
        //    if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
        //        return new HttpUnauthorizedResult();

        //    if (!_paymentService.CanRePostProcessPayment(order))
        //        return RedirectToRoute("OrderDetails", new { orderId = orderId });

        //    var postProcessPaymentRequest = new PostProcessPaymentRequest
        //    {
        //        Order = order
        //    };
        //    _paymentService.PostProcessPayment(postProcessPaymentRequest);

        //    if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
        //    {
        //        //redirection or POST has been done in PostProcessPayment
        //        return Content("Redirected");
        //    }

        //    //if no redirection has been done (to a third-party payment page)
        //    //theoretically it's not possible
        //    return RedirectToRoute("OrderDetails", new { orderId = orderId });
        //}

        //My account / Order details page / Shipment details page

        [HttpGet]
        public ShipmentDetailsModel ShipmentDetails(int shipmentId)
        {
            var shipment = _shipmentService.GetShipmentById(shipmentId);
            if (shipment == null)
                return null;

            var order = shipment.Order;
            if (order == null || order.Deleted || _workContext.CurrentCustomer.Id != order.CustomerId)
                return null;

            var model = PrepareShipmentDetailsModel(shipment);

            return model;
        }


        #endregion

        #region Post Methods

        [HttpPost]
        public LanguageSelectorResponseModel SetLanguage(int langid, string returnUrl = "")
        {
            LanguageSelectorResponseModel model = new LanguageSelectorResponseModel();

            var language = _languageService.GetLanguageById(langid);
            if (language != null && language.Published)
            {
                _workContext.WorkingLanguage = language;
                model.RightToLeft = language.Rtl;
            }

            //language part in URL
            if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
            {
                string applicationPath = HttpContext.Current.Request.ApplicationPath;
                if (returnUrl.IsLocalizedUrl(applicationPath, true))
                {
                    //already localized URL
                    returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
                }
                returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
            }

            model.ReturenUrl = returnUrl;

            return model;
        }
        //public string SetLanguage(int langid, string returnUrl = "")
        //{
        //    var language = _languageService.GetLanguageById(langid);
        //    if (language != null && language.Published)
        //    {
        //        _workContext.WorkingLanguage = language;
        //    }

        //    //language part in URL
        //    if (_localizationSettings.SeoFriendlyUrlsForLanguagesEnabled)
        //    {
        //        string applicationPath = HttpContext.Current.Request.ApplicationPath;
        //        if (returnUrl.IsLocalizedUrl(applicationPath, true))
        //        {
        //            //already localized URL
        //            returnUrl = returnUrl.RemoveLanguageSeoCodeFromRawUrl(applicationPath);
        //        }
        //        returnUrl = returnUrl.AddLanguageSeoCodeToRawUrl(applicationPath, _workContext.WorkingLanguage);
        //    }
        //    return returnUrl;
        //}

        [HttpPost]
        public string SetCurrency(int customerCurrency, string returnUrl = "")
        {
            var currency = _currencyService.GetCurrencyById(customerCurrency);
            if (currency != null)
                _workContext.WorkingCurrency = currency;

            return returnUrl;
        }

        [HttpPost]
        public SearchModel Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            var searchTerms = model.q;
            if (searchTerms == null)
                searchTerms = "";
            searchTerms = searchTerms.Trim();



            //sorting
            PrepareSortingOptions(model.PagingFilteringContext, command);
            //view mode
            PrepareViewModes(model.PagingFilteringContext, command);
            //page size
            PreparePageSizeOptions(model.PagingFilteringContext, command,
                _catalogSettings.SearchPageAllowCustomersToSelectPageSize,
                _catalogSettings.SearchPagePageSizeOptions,
                _catalogSettings.SearchPageProductsPerPage);



            var categories = new List<SearchModel.CategoryModel>();
            //all categories
            var allCategories = _categoryService.GetAllCategories();
            foreach (var c in allCategories)
            {
                //generate full category name (breadcrumb)
                string categoryBreadcrumb = "";
                var breadcrumb = c.GetCategoryBreadCrumb(allCategories, _aclService, _storeMappingService);
                for (int i = 0; i <= breadcrumb.Count - 1; i++)
                {
                    categoryBreadcrumb += breadcrumb[i].GetLocalized(x => x.Name);
                    if (i != breadcrumb.Count - 1)
                        categoryBreadcrumb += " >> ";
                }
                categories.Add(new SearchModel.CategoryModel
                {
                    Id = c.Id,
                    Breadcrumb = categoryBreadcrumb
                });
            }

            if (categories.Count > 0)
            {
                //first empty entry
                model.AvailableCategories.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                //all other categories
                foreach (var c in categories)
                {
                    model.AvailableCategories.Add(new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = c.Breadcrumb,
                        Selected = model.cid == c.Id
                    });
                }
            }

            var manufacturers = _manufacturerService.GetAllManufacturers();
            if (manufacturers.Count > 0)
            {
                model.AvailableManufacturers.Add(new SelectListItem
                {
                    Value = "0",
                    Text = _localizationService.GetResource("Common.All")
                });
                foreach (var m in manufacturers)
                    model.AvailableManufacturers.Add(new SelectListItem
                    {
                        Value = m.Id.ToString(),
                        Text = m.GetLocalized(x => x.Name),
                        Selected = model.mid == m.Id
                    });
            }

            IPagedList<Product> products = new PagedList<Product>(new List<Product>(), 0, 1);
            // only search if query string search keyword is set (used to avoid searching or displaying search term min length error message on /search page load)
            if (HttpContext.Current.Request.Params["q"] != null)
            {
                if (searchTerms.Length < _catalogSettings.ProductSearchTermMinimumLength)
                {
                    model.Warning = string.Format(_localizationService.GetResource("Search.SearchTermMinimumLengthIsNCharacters"), _catalogSettings.ProductSearchTermMinimumLength);
                }
                else
                {
                    var categoryIds = new List<int>();
                    int manufacturerId = 0;
                    decimal? minPriceConverted = null;
                    decimal? maxPriceConverted = null;
                    bool searchInDescriptions = false;
                    if (model.adv)
                    {
                        //advanced search
                        var categoryId = model.cid;
                        if (categoryId > 0)
                        {
                            categoryIds.Add(categoryId);
                            if (model.isc)
                            {
                                //include subcategories
                                categoryIds.AddRange(GetChildCategoryIds(categoryId));
                            }
                        }


                        manufacturerId = model.mid;

                        //min price
                        if (!string.IsNullOrEmpty(model.pf))
                        {
                            decimal minPrice;
                            if (decimal.TryParse(model.pf, out minPrice))
                                minPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(minPrice, _workContext.WorkingCurrency);
                        }
                        //max price
                        if (!string.IsNullOrEmpty(model.pt))
                        {
                            decimal maxPrice;
                            if (decimal.TryParse(model.pt, out maxPrice))
                                maxPriceConverted = _currencyService.ConvertToPrimaryStoreCurrency(maxPrice, _workContext.WorkingCurrency);
                        }

                        searchInDescriptions = model.sid;
                    }

                    //var searchInProductTags = false;
                    var searchInProductTags = searchInDescriptions;

                    //products
                    products = _productService.SearchProducts(
                        categoryIds: categoryIds,
                        manufacturerId: manufacturerId,
                        storeId: _storeContext.CurrentStore.Id,
                        visibleIndividuallyOnly: true,
                        priceMin: minPriceConverted,
                        priceMax: maxPriceConverted,
                        keywords: searchTerms,
                        searchDescriptions: searchInDescriptions,
                        searchSku: searchInDescriptions,
                        searchProductTags: searchInProductTags,
                        languageId: _workContext.WorkingLanguage.Id,
                        orderBy: (ProductSortingEnum)command.OrderBy,
                        pageIndex: command.PageNumber - 1,
                        pageSize: command.PageSize);
                    model.Products = PrepareProductOverviewModels(products).ToList();

                    model.NoResults = !model.Products.Any();

                    //search term statistics
                    if (!String.IsNullOrEmpty(searchTerms))
                    {
                        var searchTerm = _searchTermService.GetSearchTermByKeyword(searchTerms, _storeContext.CurrentStore.Id);
                        if (searchTerm != null)
                        {
                            searchTerm.Count++;
                            _searchTermService.UpdateSearchTerm(searchTerm);
                        }
                        else
                        {
                            searchTerm = new SearchTerm
                            {
                                Keyword = searchTerms,
                                StoreId = _storeContext.CurrentStore.Id,
                                Count = 1
                            };
                            _searchTermService.InsertSearchTerm(searchTerm);
                        }
                    }

                    //event
                    _eventPublisher.Publish(new ProductSearchEvent
                    {
                        SearchTerm = searchTerms,
                        SearchInDescriptions = searchInDescriptions,
                        CategoryIds = categoryIds,
                        ManufacturerId = manufacturerId,
                        WorkingLanguageId = _workContext.WorkingLanguage.Id
                    });
                }
            }

            model.PagingFilteringContext.LoadPagedList(products);
            return model;
        }

        [HttpPost]
        public string SubscribeNewsletter(string email, bool subscribe)
        {
            string result;
            if (!CommonHelper.IsValidEmail(email))
            {
                result = _localizationService.GetResource("Newsletter.Email.Wrong");
            }
            else
            {
                email = email.Trim();

                var subscription = _newsLetterSubscriptionService.GetNewsLetterSubscriptionByEmailAndStoreId(email, _storeContext.CurrentStore.Id);
                if (subscription != null)
                {
                    if (subscribe)
                    {
                        if (!subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                    }
                    else
                    {
                        if (subscription.Active)
                        {
                            _workflowMessageService.SendNewsLetterSubscriptionDeactivationMessage(subscription, _workContext.WorkingLanguage.Id);
                        }
                        result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                    }
                }
                else if (subscribe)
                {
                    subscription = new NewsLetterSubscription
                    {
                        NewsLetterSubscriptionGuid = Guid.NewGuid(),
                        Email = email,
                        Active = false,
                        StoreId = _storeContext.CurrentStore.Id,
                        CreatedOnUtc = DateTime.UtcNow
                    };
                    _newsLetterSubscriptionService.InsertNewsLetterSubscription(subscription);
                    _workflowMessageService.SendNewsLetterSubscriptionActivationMessage(subscription, _workContext.WorkingLanguage.Id);

                    result = _localizationService.GetResource("Newsletter.SubscribeEmailSent");
                }
                else
                {
                    result = _localizationService.GetResource("Newsletter.UnsubscribeEmailSent");
                }
            }
            return result;
        }

        [HttpPost]
        public string AddProductToCompareList(int productId)
        {
            var message = string.Empty;
            var product = _productService.GetProductById(productId);
            if (product == null || product.Deleted || !product.Published)
                return message = "No product found with the specified ID";

            if (!_catalogSettings.CompareProductsEnabled)
                return message = "Product comparison is disabled";

            _compareProductsService.AddProductToCompareList(productId);

            //activity log
            _customerActivityService.InsertActivity("PublicStore.AddToCompareList", _localizationService.GetResource("ActivityLog.PublicStore.AddToCompareList"), product.Name);

            return message = _localizationService.GetResource("Products.ProductHasBeenAddedToCompareList.Link");
            //use the code below (commented) if you want a customer to be automatically redirected to the compare products page
            //redirect = Url.RouteUrl("CompareProducts"),
        }

        [HttpPost]
        public CustomerLoginResults Login(LoginModel model, string returnUrl, bool captchaValid)
        {
            //validate CAPTCHA
            if (_captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage && !captchaValid)
            {
                ModelState.AddModelError("", _localizationService.GetResource("Common.WrongCaptcha"));
            }

            //if (ModelState.IsValid)
            {
                if (_customerSettings.UsernamesEnabled && model.Username != null)
                {
                    model.Username = model.Username.Trim();
                }
                var loginResult = _customerRegistrationService.ValidateCustomer(_customerSettings.UsernamesEnabled ? model.Username : model.Email, model.Password);
                switch (loginResult)
                {
                    case CustomerLoginResults.Successful:
                        {
                            var customer = _customerSettings.UsernamesEnabled ? _customerService.GetCustomerByUsername(model.Username) : _customerService.GetCustomerByEmail(model.Email);

                            //migrate shopping cart
                            _shoppingCartService.MigrateShoppingCart(_workContext.CurrentCustomer, customer, true);

                            //sign in new customer
                            _authenticationService.SignIn(customer, model.RememberMe);

                            //activity log
                            _customerActivityService.InsertActivity("PublicStore.Login", _localizationService.GetResource("ActivityLog.PublicStore.Login"), customer);

                            //if (string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)) {
                            //}
                            //return RedirectToRoute("HomePage");

                            //return Redirect(returnUrl);
                            return CustomerLoginResults.Successful;
                        }
                    //break;
                    case CustomerLoginResults.CustomerNotExist:
                        return loginResult;
                    case CustomerLoginResults.Deleted:
                        return loginResult;
                    case CustomerLoginResults.NotActive:
                        return loginResult;
                    case CustomerLoginResults.NotRegistered:
                        return loginResult;
                    case CustomerLoginResults.WrongPassword:
                    default:
                        return loginResult;
                }
            }

            //If we got this far, something failed, redisplay form
            //model.UsernamesEnabled = _customerSettings.UsernamesEnabled;
            //model.DisplayCaptcha = _captchaSettings.Enabled && _captchaSettings.ShowOnLoginPage;
            //return new CustomerLoginResults();
        }

        [HttpPost]
        public ShoppingCartModel UpdateCartItem(int ShoppingCartItemId, int Quantity, bool remove = false)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.EnableShoppingCart))
                return null;

            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();

            //var allIdsToRemove = form["removefromcart"] != null ? form["removefromcart"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList() : new List<int>();

            //current warnings <cart item identifier, warnings>
            var innerWarnings = new Dictionary<int, IList<string>>();
            foreach (var sci in cart)
            {
                //bool remove = allIdsToRemove.Contains(sci.Id);
                if (remove && sci.Id == ShoppingCartItemId)
                    _shoppingCartService.DeleteShoppingCartItem(sci, ensureOnlyActiveCheckoutAttributes: true);
                else if (sci.Id == ShoppingCartItemId)
                {

                    var currSciWarnings = _shoppingCartService.UpdateShoppingCartItem(_workContext.CurrentCustomer,
                        sci.Id, sci.AttributesXml, sci.CustomerEnteredPrice,
                        sci.RentalStartDateUtc, sci.RentalEndDateUtc,
                        Quantity, true);
                    innerWarnings.Add(sci.Id, currSciWarnings);
                }
            }

            //updated cart
            cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            var model = new ShoppingCartModel();
            PrepareShoppingCartModel(model, cart);
            //update current warnings
            foreach (var kvp in innerWarnings)
            {
                //kvp = <cart item identifier, warnings>
                var sciId = kvp.Key;
                var warnings = kvp.Value;
                //find model
                var sciModel = model.Items.FirstOrDefault(x => x.Id == sciId);
                if (sciModel != null)
                    foreach (var w in warnings)
                        if (!sciModel.Warnings.Contains(w))
                            sciModel.Warnings.Add(w);
            }
            return model;
        }

        [HttpPost]
        public ChangePasswordModel ChangePassword(ChangePasswordModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;

            if (ModelState.IsValid)
            {
                var changePasswordRequest = new ChangePasswordRequest(customer.Email,
                    true, _customerSettings.DefaultPasswordFormat, model.NewPassword, model.OldPassword);
                var changePasswordResult = _customerRegistrationService.ChangePassword(changePasswordRequest);
                if (changePasswordResult.Success)
                {
                    model.Result = _localizationService.GetResource("Account.ChangePassword.Success");
                    return model;
                }

                //errors
                foreach (var error in changePasswordResult.Errors)
                    ModelState.AddModelError("", error);
            }


            //If we got this far, something failed, redisplay form
            return model;
        }

        [HttpPost]
        public CustomerAddressEditModel AddressAdd(CustomerAddressEditModel model)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;

            ////custom address attributes
            //var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            //var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            //foreach (var error in customAttributeWarnings)
            //{
            //    ModelState.AddModelError("", error);
            //}

            if (ModelState.IsValid)
            {
                var address = model.Address.ToEntity();
                //address.CustomAttributes = customAttributes;
                address.CreatedOnUtc = DateTime.UtcNow;
                //some validation
                if (address.CountryId == 0)
                    address.CountryId = null;
                if (address.StateProvinceId == 0)
                    address.StateProvinceId = null;
                customer.Addresses.Add(address);
                _customerService.UpdateCustomer(customer);

                //return RedirectToRoute("CustomerAddresses");
                //return null;
            }

            //If we got this far, something failed, redisplay form
            model.Address.PrepareModelApi(
                address: null,
                excludeProperties: true,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());

            return model;
        }

        [HttpPost]
        public CustomerAddressEditModel AddressEdit(CustomerAddressEditModel model, int addressId)
        {
            if (!_workContext.CurrentCustomer.IsRegistered())
                return null;

            var customer = _workContext.CurrentCustomer;
            //find address (ensure that it belongs to the current customer)
            var address = customer.Addresses.FirstOrDefault(a => a.Id == addressId);
            if (address == null)
                //address is not found
            return null;

            //_logger.InsertLog(LogLevel.Information, "After find address call edit method");

            //custom address attributes
            //var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
            //var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
            //foreach (var error in customAttributeWarnings)
            //{
            //    ModelState.AddModelError("", error);
            //}

            if (ModelState.IsValid)
            {
                address = model.Address.ToEntity(address);
                //address.CustomAttributes = customAttributes;
                _addressService.UpdateAddress(address);

                //return RedirectToRoute("CustomerAddresses");
                //return Json("CustomerAddresses");
            }


            //If we got this far, something failed, redisplay form
            model.Address.PrepareModelApi(
                address: address,
                excludeProperties: true,
                addressSettings: _addressSettings,
                localizationService: _localizationService,
                stateProvinceService: _stateProvinceService,
                addressAttributeService: _addressAttributeService,
                addressAttributeParser: _addressAttributeParser,
                loadCountries: () => _countryService.GetAllCountries());
            return model;
        }

        //[HttpPost]
        //public 

        #region Methods (one page checkout)

        [NonAction]
        protected System.Web.Mvc.JsonResult OpcLoadStepAfterShippingMethod(List<ShoppingCartItem> cart)
        {
            //Check whether payment workflow is required
            //we ignore reward points during cart total calculation
            bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart, true);
            if (isPaymentWorkflowRequired)
            {
                //filter by country
                int filterByCountryId = 0;
                if (_addressSettings.CountryEnabled &&
                    _workContext.CurrentCustomer.BillingAddress != null &&
                    _workContext.CurrentCustomer.BillingAddress.Country != null)
                {
                    filterByCountryId = _workContext.CurrentCustomer.BillingAddress.Country.Id;
                }

                //payment is required
                var paymentMethodModel = PreparePaymentMethodModel(cart, filterByCountryId);

                if (_paymentSettings.BypassPaymentMethodSelectionIfOnlyOne &&
                    paymentMethodModel.PaymentMethods.Count == 1 && !paymentMethodModel.DisplayRewardPoints)
                {
                    //if we have only one payment method and reward points are disabled or the current customer doesn't have any reward points
                    //so customer doesn't have to choose a payment method

                    var selectedPaymentMethodSystemName = paymentMethodModel.PaymentMethods[0].PaymentMethodSystemName;
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod,
                        selectedPaymentMethodSystemName, _storeContext.CurrentStore.Id);

                    var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(selectedPaymentMethodSystemName);
                    if (paymentMethodInst == null ||
                        !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                        !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                        throw new Exception("Selected payment method can't be parsed");

                    return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
                }

                //customer have to choose a payment method
                return new System.Web.Mvc.JsonResult
                {
                    Data = new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "payment-method",
                            //html = this.RenderPartialViewToString("OpcPaymentMethods", paymentMethodModel),
                        },
                        goto_section = "payment_method",
                        model = paymentMethodModel
                    }
                };
            }

            //payment is not required
            _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

            var confirmOrderModel = PrepareConfirmOrderModel(cart);
            return new System.Web.Mvc.JsonResult
            {
                Data = new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "confirm-order",
                        //html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                    },
                    goto_section = "confirm_order",
                    model = confirmOrderModel
                }
            };
        }

        [NonAction]
        protected System.Web.Mvc.JsonResult OpcLoadStepAfterPaymentMethod(IPaymentMethod paymentMethod, List<ShoppingCartItem> cart)
        {
            if (paymentMethod.SkipPaymentInfo)
            {
                //skip payment info page
                var paymentInfo = new ProcessPaymentRequest();
                //session save
                HttpContext.Current.Session["OrderPaymentInfo"] = paymentInfo;

                var confirmOrderModel = PrepareConfirmOrderModel(cart);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            //html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order",
                        model = confirmOrderModel
                    }
                };
            }


            //return payment info page
            var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
            return new System.Web.Mvc.JsonResult
            {
                Data = new
                {
                    update_section = new UpdateSectionJsonModel
                    {
                        name = "payment-info",
                        //html = this.RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
                    },
                    goto_section = "payment_info",
                    model = paymenInfoModel
                }
            };
        }

        public OnePageCheckoutModel OnePageCheckout()
        {
            //validation
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                .LimitPerStore(_storeContext.CurrentStore.Id)
                .ToList();
            if (cart.Count == 0)
                //return RedirectToRoute("ShoppingCart");
                return null;

            if (!_orderSettings.OnePageCheckoutEnabled)
                //return RedirectToRoute("Checkout");
                return null;

            if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                //return new HttpUnauthorizedResult();
                return null;

            var model = new OnePageCheckoutModel
            {
                ShippingRequired = cart.RequiresShipping(),
                DisableBillingAddressCheckoutStep = _orderSettings.DisableBillingAddressCheckoutStep
            };
            return model;
        }

        //[ChildActionOnly]
        [HttpGet]
        public CheckoutBillingAddressModel OpcBillingForm()
        {
            var billingAddressModel = PrepareBillingAddressModel(prePopulateNewAddressWithCustomerFields: true);
            //return PartialView("OpcBillingAddress", billingAddressModel);
            return billingAddressModel;
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcSaveBilling(System.Web.Mvc.FormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                int billingAddressId;
                int.TryParse(form["billing_address_id"], out billingAddressId);

                if (billingAddressId > 0)
                {
                    //existing address
                    var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == billingAddressId);
                    if (address == null)
                        throw new Exception("Address can't be loaded");

                    _workContext.CurrentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                else
                {
                    //new address
                    var model = new CheckoutBillingAddressModel();
                    //TryUpdateModel(model.NewAddress, "BillingNewAddress");

                    //custom address attributes
                    var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
                    var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    //TryValidateModel(model.NewAddress);
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var billingAddressModel = PrepareBillingAddressModel(selectedCountryId: model.NewAddress.CountryId);
                        billingAddressModel.NewAddressPreselected = true;
                        return new System.Web.Mvc.JsonResult
                        {
                            Data = new
                            {
                                update_section = new UpdateSectionJsonModel
                                {
                                    name = "billing",
                                    //html = this.RenderPartialViewToString("OpcBillingAddress", billingAddressModel)
                                },
                                wrong_billing_address = true,
                                model = billingAddressModel
                            }
                        };
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        //address is not found. let's create a new one
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //some validation
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;
                        if (address.CountryId.HasValue && address.CountryId.Value > 0)
                        {
                            address.Country = _countryService.GetCountryById(address.CountryId.Value);
                        }
                        _workContext.CurrentCustomer.Addresses.Add(address);
                    }
                    _workContext.CurrentCustomer.BillingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }

                if (cart.RequiresShipping())
                {
                    //shipping is required
                    var shippingAddressModel = PrepareShippingAddressModel(prePopulateNewAddressWithCustomerFields: true);
                    return new System.Web.Mvc.JsonResult
                    {
                        Data = new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "shipping",
                                //html = this.RenderPartialViewToString("OpcShippingAddress", shippingAddressModel)
                            },
                            goto_section = "shipping",
                            model = shippingAddressModel
                        }
                    };
                }

                //shipping is not required
                _genericAttributeService.SaveAttribute<ShippingOption>(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, null, _storeContext.CurrentStore.Id);

                //load next step
                return OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new { error = 1, message = exc.Message }
                };
            }
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcSaveShipping(System.Web.Mvc.FormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                //Pick up in store?
                if (_shippingSettings.AllowPickUpInStore)
                {
                    var model = new CheckoutShippingAddressModel();
                    //TryUpdateModel(model);

                    if (model.PickUpInStore)
                    {
                        //customer decided to pick up in store

                        //no shipping address selected
                        _workContext.CurrentCustomer.ShippingAddress = null;
                        _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                        var pickupPoint = form["pickup-points-id"].Split(new[] { "___" }, StringSplitOptions.None);
                        var pickupPoints = _shippingService
                            .GetPickupPoints(_workContext.CurrentCustomer.BillingAddress, pickupPoint[1], _storeContext.CurrentStore.Id).PickupPoints.ToList();
                        var selectedPoint = pickupPoints.FirstOrDefault(x => x.Id.Equals(pickupPoint[0]));

                        //set value indicating that "pick up in store" option has been chosen
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, true, _storeContext.CurrentStore.Id);

                        //save "pick up in store" shipping method
                        var pickUpInStoreShippingOption = new ShippingOption
                        {
                            //Name = _localizationService.GetResource("Checkout.PickUpInStore.MethodName"),
                            //Rate = selectedPoint.PickupFee,
                            //Description = null,
                            //ShippingRateComputationMethodSystemName = null

                             Name = string.Format(_localizationService.GetResource("Checkout.PickupPoints.Name"), selectedPoint.Name),
                            Rate = selectedPoint.PickupFee,
                            Description = selectedPoint.Description,
                            ShippingRateComputationMethodSystemName = selectedPoint.ProviderSystemName

                            
                        };
                        _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedShippingOption,
                        pickUpInStoreShippingOption,
                        _storeContext.CurrentStore.Id);

                        //load next step
                        return OpcLoadStepAfterShippingMethod(cart);
                    }

                    //set value indicating that "pick up in store" option has not been chosen
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedPickupPoint, false, _storeContext.CurrentStore.Id);
                }

                int shippingAddressId;
                int.TryParse(form["shipping_address_id"], out shippingAddressId);

                if (shippingAddressId > 0)
                {
                    //existing address
                    var address = _workContext.CurrentCustomer.Addresses.FirstOrDefault(a => a.Id == shippingAddressId);
                    if (address == null)
                        throw new Exception("Address can't be loaded");

                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }
                else
                {
                    //new address
                    var model = new CheckoutShippingAddressModel();
                    //TryUpdateModel(model.NewAddress, "ShippingNewAddress");

                    //custom address attributes
                    var customAttributes = form.ParseCustomAddressAttributes(_addressAttributeParser, _addressAttributeService);
                    var customAttributeWarnings = _addressAttributeParser.GetAttributeWarnings(customAttributes);
                    foreach (var error in customAttributeWarnings)
                    {
                        ModelState.AddModelError("", error);
                    }

                    //validate model
                    //TryValidateModel(model.NewAddress);
                    if (!ModelState.IsValid)
                    {
                        //model is not valid. redisplay the form with errors
                        var shippingAddressModel = PrepareShippingAddressModel(selectedCountryId: model.NewAddress.CountryId);
                        shippingAddressModel.NewAddressPreselected = true;
                        return new System.Web.Mvc.JsonResult
                        {
                            Data = new
                            {
                                update_section = new UpdateSectionJsonModel
                                {
                                    name = "shipping",
                                    //html = this.RenderPartialViewToString("OpcShippingAddress", shippingAddressModel)
                                },
                                model = shippingAddressModel
                            }
                        };
                    }

                    //try to find an address with the same values (don't duplicate records)
                    var address = _workContext.CurrentCustomer.Addresses.ToList().FindAddress(
                        model.NewAddress.FirstName, model.NewAddress.LastName, model.NewAddress.PhoneNumber,
                        model.NewAddress.Email, model.NewAddress.FaxNumber, model.NewAddress.Company,
                        model.NewAddress.Address1, model.NewAddress.Address2, model.NewAddress.City,
                        model.NewAddress.StateProvinceId, model.NewAddress.ZipPostalCode,
                        model.NewAddress.CountryId, customAttributes);
                    if (address == null)
                    {
                        address = model.NewAddress.ToEntity();
                        address.CustomAttributes = customAttributes;
                        address.CreatedOnUtc = DateTime.UtcNow;
                        //little hack here (TODO: find a better solution)
                        //EF does not load navigation properties for newly created entities (such as this "Address").
                        //we have to load them manually 
                        //otherwise, "Country" property of "Address" entity will be null in shipping rate computation methods
                        if (address.CountryId.HasValue)
                            address.Country = _countryService.GetCountryById(address.CountryId.Value);
                        if (address.StateProvinceId.HasValue)
                            address.StateProvince = _stateProvinceService.GetStateProvinceById(address.StateProvinceId.Value);

                        //other null validations
                        if (address.CountryId == 0)
                            address.CountryId = null;
                        if (address.StateProvinceId == 0)
                            address.StateProvinceId = null;
                        _workContext.CurrentCustomer.Addresses.Add(address);
                    }
                    _workContext.CurrentCustomer.ShippingAddress = address;
                    _customerService.UpdateCustomer(_workContext.CurrentCustomer);
                }

                var shippingMethodModel = PrepareShippingMethodModel(cart, _workContext.CurrentCustomer.ShippingAddress);

                if (_shippingSettings.BypassShippingMethodSelectionIfOnlyOne &&
                    shippingMethodModel.ShippingMethods.Count == 1)
                {
                    //if we have only one shipping method, then a customer doesn't have to choose a shipping method
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedShippingOption,
                        shippingMethodModel.ShippingMethods.First().ShippingOption,
                        _storeContext.CurrentStore.Id);

                    //load next step
                    return OpcLoadStepAfterShippingMethod(cart);
                }


                return new System.Web.Mvc.JsonResult
                {
                    Data = new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "shipping-method",
                            //html = this.RenderPartialViewToString("OpcShippingMethods", shippingMethodModel)
                        },
                        goto_section = "shipping_method",
                        model = shippingMethodModel
                    }
                };
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new { error = 1, message = exc.Message }
                };
            }
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcSaveShippingMethod(System.Web.Mvc.FormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                if (!cart.RequiresShipping())
                    throw new Exception("Shipping is not required");

                //parse selected method 
                string shippingoption = form["shippingoption"];
                if (String.IsNullOrEmpty(shippingoption))
                    throw new Exception("Selected shipping method can't be parsed");
                var splittedOption = shippingoption.Split(new[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                if (splittedOption.Length != 2)
                    throw new Exception("Selected shipping method can't be parsed");
                string selectedName = splittedOption[0];
                string shippingRateComputationMethodSystemName = splittedOption[1];

                //find it
                //performance optimization. try cache first
                var shippingOptions = _workContext.CurrentCustomer.GetAttribute<List<ShippingOption>>(SystemCustomerAttributeNames.OfferedShippingOptions, _storeContext.CurrentStore.Id);
                if (shippingOptions == null || shippingOptions.Count == 0)
                {
                    //not found? let's load them using shipping service
                    shippingOptions = _shippingService
                        .GetShippingOptions(cart, _workContext.CurrentCustomer.ShippingAddress, shippingRateComputationMethodSystemName, _storeContext.CurrentStore.Id)
                        .ShippingOptions
                        .ToList();
                }
                else
                {
                    //loaded cached results. let's filter result by a chosen shipping rate computation method
                    shippingOptions = shippingOptions.Where(so => so.ShippingRateComputationMethodSystemName.Equals(shippingRateComputationMethodSystemName, StringComparison.InvariantCultureIgnoreCase))
                        .ToList();
                }

                var shippingOption = shippingOptions
                    .Find(so => !String.IsNullOrEmpty(so.Name) && so.Name.Equals(selectedName, StringComparison.InvariantCultureIgnoreCase));
                if (shippingOption == null)
                    throw new Exception("Selected shipping method can't be loaded");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, SystemCustomerAttributeNames.SelectedShippingOption, shippingOption, _storeContext.CurrentStore.Id);

                //load next step
                return OpcLoadStepAfterShippingMethod(cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new { error = 1, message = exc.Message }
                };
            }
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcSavePaymentMethod(System.Web.Mvc.FormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                string paymentmethod = form["paymentmethod"];
                //payment method 
                if (String.IsNullOrEmpty(paymentmethod))
                    throw new Exception("Selected payment method can't be parsed");


                var model = new CheckoutPaymentMethodModel();
                //TryUpdateModel(model);

                //reward points
                if (_rewardPointsSettings.Enabled)
                {
                    _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.UseRewardPointsDuringCheckout, model.UseRewardPoints,
                        _storeContext.CurrentStore.Id);
                }

                //Check whether payment workflow is required
                bool isPaymentWorkflowRequired = IsPaymentWorkflowRequired(cart);
                if (!isPaymentWorkflowRequired)
                {
                    //payment is not required
                    _genericAttributeService.SaveAttribute<string>(_workContext.CurrentCustomer,
                        SystemCustomerAttributeNames.SelectedPaymentMethod, null, _storeContext.CurrentStore.Id);

                    var confirmOrderModel = PrepareConfirmOrderModel(cart);
                    return new System.Web.Mvc.JsonResult
                    {
                        Data = new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "confirm-order",
                                //html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                            },
                            goto_section = "confirm_order",
                            model = confirmOrderModel
                        }
                    };
                }

                var paymentMethodInst = _paymentService.LoadPaymentMethodBySystemName(paymentmethod);
                if (paymentMethodInst == null ||
                    !paymentMethodInst.IsPaymentMethodActive(_paymentSettings) ||
                    !_pluginFinder.AuthenticateStore(paymentMethodInst.PluginDescriptor, _storeContext.CurrentStore.Id))
                    throw new Exception("Selected payment method can't be parsed");

                //save
                _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                    SystemCustomerAttributeNames.SelectedPaymentMethod, paymentmethod, _storeContext.CurrentStore.Id);

                return OpcLoadStepAfterPaymentMethod(paymentMethodInst, cart);
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new { error = 1, message = exc.Message }
                };
            }
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcSavePaymentInfo(System.Web.Mvc.FormCollection form)
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                var paymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(paymentMethodSystemName);
                if (paymentMethod == null)
                    throw new Exception("Payment method is not selected");

                var paymentControllerType = paymentMethod.GetControllerType();
                var paymentController = System.Web.Mvc.DependencyResolver.Current.GetService(paymentControllerType) as BasePaymentController;
                if (paymentController == null)
                    throw new Exception("Payment controller cannot be loaded");

                var warnings = paymentController.ValidatePaymentForm(form);
                foreach (var warning in warnings)
                    ModelState.AddModelError("", warning);
                if (ModelState.IsValid)
                {
                    //get payment info
                    var paymentInfo = paymentController.GetPaymentInfo(form);
                    //session save
                    HttpContext.Current.Session["OrderPaymentInfo"] = paymentInfo;

                    var confirmOrderModel = PrepareConfirmOrderModel(cart);
                    return new System.Web.Mvc.JsonResult
                    {
                        Data = new
                        {
                            update_section = new UpdateSectionJsonModel
                            {
                                name = "confirm-order",
                                //html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                            },
                            goto_section = "confirm_order",
                            model = confirmOrderModel
                        }
                    };

                }

                //If we got this far, something failed, redisplay form
                var paymenInfoModel = PreparePaymentInfoModel(paymentMethod);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "payment-info",
                            //html = this.RenderPartialViewToString("OpcPaymentInfo", paymenInfoModel)
                        },
                        model = paymenInfoModel
                    }
                };

            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                //return Json(new { error = 1, message = exc.Message });
                return null;
            }
        }

        [HttpPost]
        public System.Web.Mvc.JsonResult OpcConfirmOrder()
        {
            try
            {
                //validation
                var cart = _workContext.CurrentCustomer.ShoppingCartItems
                    .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                    .LimitPerStore(_storeContext.CurrentStore.Id)
                    .ToList();
                if (cart.Count == 0)
                    throw new Exception("Your cart is empty");

                if (!_orderSettings.OnePageCheckoutEnabled)
                    throw new Exception("One page checkout is disabled");

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    throw new Exception("Anonymous checkout is not allowed");

                //prevent 2 orders being placed within an X seconds time frame
                if (!IsMinimumOrderPlacementIntervalValid(_workContext.CurrentCustomer))
                    throw new Exception(_localizationService.GetResource("Checkout.MinOrderPlacementInterval"));

                //place order
                var processPaymentRequest = HttpContext.Current.Session["OrderPaymentInfo"] as ProcessPaymentRequest;
                if (processPaymentRequest == null)
                {
                    //Check whether payment workflow is required
                    if (IsPaymentWorkflowRequired(cart))
                    {
                        throw new Exception("Payment information is not entered");
                    }
                    else
                        processPaymentRequest = new ProcessPaymentRequest();
                }

                processPaymentRequest.StoreId = _storeContext.CurrentStore.Id;
                processPaymentRequest.CustomerId = _workContext.CurrentCustomer.Id;
                processPaymentRequest.PaymentMethodSystemName = _workContext.CurrentCustomer.GetAttribute<string>(
                    SystemCustomerAttributeNames.SelectedPaymentMethod,
                    _genericAttributeService, _storeContext.CurrentStore.Id);
                var placeOrderResult = _orderProcessingService.PlaceOrder(processPaymentRequest);
                if (placeOrderResult.Success)
                {
                    HttpContext.Current.Session["OrderPaymentInfo"] = null;
                    var postProcessPaymentRequest = new PostProcessPaymentRequest
                    {
                        Order = placeOrderResult.PlacedOrder
                    };


                    var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(placeOrderResult.PlacedOrder.PaymentMethodSystemName);
                    if (paymentMethod == null)
                        //payment method could be null if order total is 0
                        //success
                        new System.Web.Mvc.JsonResult
                        {
                            Data = new { success = 1 }
                        };

                    if (paymentMethod.PaymentMethodType == PaymentMethodType.Redirection)
                    {
                        //Redirection will not work because it's AJAX request.
                        //That's why we don't process it here (we redirect a user to another page where he'll be redirected)

                        //redirect
                        new System.Web.Mvc.JsonResult
                        {
                            Data = new
                            {
                                redirect = string.Format("{0}checkout/OpcCompleteRedirectionPayment", _webHelper.GetStoreLocation())
                            }
                        };
                    }

                    _paymentService.PostProcessPayment(postProcessPaymentRequest);
                    //success
                    return new System.Web.Mvc.JsonResult
                    {
                        Data = new { success = 1 }
                    };
                }

                //error
                var confirmOrderModel = new CheckoutConfirmModel();
                foreach (var error in placeOrderResult.Errors)
                    confirmOrderModel.Warnings.Add(error);

                return new System.Web.Mvc.JsonResult
                {
                    Data = new
                    {
                        update_section = new UpdateSectionJsonModel
                        {
                            name = "confirm-order",
                            //html = this.RenderPartialViewToString("OpcConfirmOrder", confirmOrderModel)
                        },
                        goto_section = "confirm_order",
                        model = confirmOrderModel
                    }
                };
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                return new System.Web.Mvc.JsonResult
                {
                    Data = new { error = 1, message = exc.Message }
                };
            }
        }

        public CustomJsonResult OpcCompleteRedirectionPayment()
        {
            try
            {
                //validation
                if (!_orderSettings.OnePageCheckoutEnabled)
                    //return RedirectToRoute("HomePage");
                    return new CustomJsonResult()
                    {
                        redirect = "Home",
                        success = false
                    };

                if ((_workContext.CurrentCustomer.IsGuest() && !_orderSettings.AnonymousCheckoutAllowed))
                    //return new HttpUnauthorizedResult();
                    return new CustomJsonResult()
                    {
                        success = false,
                        redirect = ""
                    };

                //get the order
                var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1)
                    .FirstOrDefault();
                if (order == null)
                    //return RedirectToRoute("HomePage");
                    return new CustomJsonResult()
                    {
                        redirect = "Home",
                        success = false
                    };


                var paymentMethod = _paymentService.LoadPaymentMethodBySystemName(order.PaymentMethodSystemName);
                if (paymentMethod == null)
                    //return RedirectToRoute("HomePage");
                    return new CustomJsonResult()
                    {
                        redirect = "Home",
                        success = false
                    };
                if (paymentMethod.PaymentMethodType != PaymentMethodType.Redirection)
                    //return RedirectToRoute("HomePage");
                    return new CustomJsonResult()
                    {
                        redirect = "Home",
                        success = false
                    };

                //ensure that order has been just placed
                if ((DateTime.UtcNow - order.CreatedOnUtc).TotalMinutes > 3)
                    //return RedirectToRoute("HomePage");
                    return new CustomJsonResult()
                    {
                        redirect = "Home",
                        success = false
                    };


                //Redirection will not work on one page checkout page because it's AJAX request.
                //That's why we process it here
                var postProcessPaymentRequest = new PostProcessPaymentRequest
                {
                    Order = order
                };

                _paymentService.PostProcessPayment(postProcessPaymentRequest);

                if (_webHelper.IsRequestBeingRedirected || _webHelper.IsPostBeingDone)
                {
                    //redirection or POST has been done in PostProcessPayment
                    //return Content("Redirected");
                    //return null;
                    return new CustomJsonResult()
                    {
                        //redirect = "Home",
                        success = false,
                        message = "Redirected",
                    };
                }

                //if no redirection has been done (to a third-party payment page)
                //theoretically it's not possible
                //return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
                return new CustomJsonResult()
                {
                    //redirect = "Home",
                    success = false,
                    message = "CheckoutCompleted",
                };
            }
            catch (Exception exc)
            {
                _logger.Warning(exc.Message, exc, _workContext.CurrentCustomer);
                //return Content(exc.Message);
                return new CustomJsonResult()
                {
                    //redirect = "Home",
                    success = false,
                    message = exc.Message,
                };

            }
        }


        #endregion



        #endregion

        #endregion
    }
}

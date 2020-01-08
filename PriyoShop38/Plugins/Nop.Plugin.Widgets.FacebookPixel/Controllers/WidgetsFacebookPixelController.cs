using System.Web.Mvc;
using Nop.Plugin.Widgets.FacebookPixel.Extension;
using Nop.Plugin.Widgets.FacebookPixel.Models;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Tax;
using Nop.Web.Framework.Controllers;
using System;
using System.Linq;
using System.Globalization;
using Nop.Core.Domain.Orders;
using Nop.Core;
using Nop.Services.Orders;
using Nop.Services.Logging;
using Nop.Services.Catalog;
using System.Text;
using Nop.Core.Domain;
using Nop.Services.Stores;
using System.Collections.Generic;
using System.Web;
using Nop.Core.Domain.Discounts;
using Nop.Core.Domain.Tax;

namespace Nop.Plugin.Widgets.FacebookPixel.Controllers
{
    public class WidgetsFacebookPixelController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;
        private readonly IProductService _productService;
        private readonly StoreInformationSettings _storeInformationSettings;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;

        private readonly CultureInfo _usCulture;

        public WidgetsFacebookPixelController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            IOrderService orderService,
            IOrderTotalCalculationService orderTotalCalculationService,
            ICategoryService categoryService,
            ILogger logger,
            StoreInformationSettings storeInformationSettings,
            IProductService productService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ITaxService taxService,
            ICurrencyService currencyService
            )
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._orderService = orderService;
            this._orderTotalCalculationService = orderTotalCalculationService;
            this._categoryService = categoryService;
            this._logger = logger;
            this._storeInformationSettings = storeInformationSettings;
            this._usCulture = new CultureInfo("en-US");
            this._productService = productService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._localizationService = localizationService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._permissionService = permissionService;
        }

        [AdminAuthorize]
        public ActionResult Configure()
        {
            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookPixelSettings = _settingService.LoadSetting<FacebookPixelSettings>(storeScope);
            var model = new ConfigurationModel();
            model.FacebookPixelScriptFirstPart = facebookPixelSettings.FacebookPixelScriptFirstPart;
            model.FacebookPixelScriptLastPart = facebookPixelSettings.FacebookPixelScriptLastPart;
            model.DefaultFacebookPixelScriptLastPart = FacebookPixelPlugin.FACEBOOKPIXEL_SCRIPT_LASTPART;
            model.DefaultFacebookPixelScriptFirstPart = FacebookPixelPlugin.FACEBOOKPIXEL_SCRIPT_FIRSTPART;

            model.ActiveStoreScopeConfiguration = storeScope;

            if (storeScope > 0)
            {
                model.FacebookPixelScriptFirstPart_OverrideForStore = _settingService.SettingExists(facebookPixelSettings, x => x.FacebookPixelScriptFirstPart, storeScope);
                model.FacebookPixelScriptLastPart_OverrideForStore = _settingService.SettingExists(facebookPixelSettings, x => x.FacebookPixelScriptLastPart, storeScope);
            }

            return View("~/Plugins/Widgets.FacebookPixel/Views/WidgetsFacebookPixel/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var facebookPixelSettings = _settingService.LoadSetting<FacebookPixelSettings>(storeScope);

            //save settings
            facebookPixelSettings.FacebookPixelScriptFirstPart = model.FacebookPixelScriptFirstPart;
            facebookPixelSettings.FacebookPixelScriptLastPart = model.FacebookPixelScriptLastPart;

            /* We do not clear cache after each setting update.
          * This behavior can increase performance because cached settings will not be cleared 
          * and loaded from database after each update */

            if (model.FacebookPixelScriptFirstPart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(facebookPixelSettings, x => x.FacebookPixelScriptFirstPart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(facebookPixelSettings, x => x.FacebookPixelScriptFirstPart, storeScope);
            if (model.FacebookPixelScriptLastPart_OverrideForStore || storeScope == 0)
                _settingService.SaveSetting(facebookPixelSettings, x => x.FacebookPixelScriptLastPart, storeScope, false);
            else if (storeScope > 0)
                _settingService.DeleteSetting(facebookPixelSettings, x => x.FacebookPixelScriptLastPart, storeScope);
            //now clear settings cache
            _settingService.ClearCache();

            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            string globalScript = "";
            var routeData = ((System.Web.UI.Page)this.HttpContext.CurrentHandler).RouteData;
            string controller = routeData.Values["controller"].ToString().ToLowerInvariant();
            string action = routeData.Values["action"].ToString().ToLowerInvariant();

            try
            {
                switch (controller)
                {
                    case "home":
                        if (action.Equals("index", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("home", null, null, null);
                            globalScript += GetFacebookPixelScriptLastPart("home", null, null, null);
                        }
                        break;

                    case "catalog":
                        if (action.Equals("Category", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Category", null, null, null);
                            globalScript += GetFacebookPixelScriptLastPart("Category", null, null, null);
                        }
                        else if (action.Equals("Manufacturer", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Manufacturer", null, null, null);
                            globalScript += GetFacebookPixelScriptLastPart("Manufacturer", null, null, null);
                        }

                        else if (action.Equals("Vendor", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Vendor", null, null, null);
                            globalScript += GetFacebookPixelScriptLastPart("Vendor", null, null, null);
                        }
                        else if (action.Equals("search", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("search", null, null, null);
                            globalScript += "fbq('track', 'Search');";
                            globalScript += GetFacebookPixelScriptLastPart("search", null, null, null);
                        }
                        break;

                    case "product":
                        if (action.Equals("ProductDetails", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("ProductDetails", null, null, null);
                            globalScript += "fbq('track', 'ViewContent'); \r\n";
                            globalScript += GetFacebookPixelScriptLastPart("ProductDetails", null, null, null);
                        }
                        break;

                    case "shoppingcart":
                        if (action.Equals("Cart", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Cart", null, null, null);
                            globalScript += "fbq('track', 'AddToCart');";
                            globalScript += GetFacebookPixelScriptLastPart("Cart", null, null, null);
                        }
                        else if (action.Equals("Wishlist", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Wishlist", null, null, null);
                            globalScript += "fbq('track', 'AddToWishlist');";
                            globalScript += GetFacebookPixelScriptLastPart("Wishlist", null, null, null);
                        }
                        break;

                    case "checkout":
                    case "misconepagecheckout":

                        if (action.Equals("OpcSavePaymentInfo", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("OpcSavePaymentInfo", null, null, null);
                            globalScript += "fbq('track', 'AddPaymentInfo');";
                            globalScript += GetFacebookPixelScriptLastPart("OpcSavePaymentInfo", null, null, null);
                        }
                        else if (action.Equals("OnePageCheckout", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("OnePageCheckout", null, null, null);
                            globalScript += "fbq('track', 'InitiateCheckout');";
                            globalScript += GetFacebookPixelScriptLastPart("OnePageCheckout", null, null, null);
                        }

                        else if (action.Equals("Completed", StringComparison.InvariantCultureIgnoreCase))
                        {
                            var lastOrder = GetLastOrder();
                            var orderTotal =  1.00;
                            if (lastOrder != null)
                            {
                                orderTotal = (double) lastOrder.OrderTotal;
                            }
                            globalScript += GetFacebookPixelScriptFirstPart("Completed", null, null, null);
                            globalScript += "fbq('track', 'Purchase', {value:'"+ orderTotal+"', currency: 'BDT'});";
                            globalScript += GetFacebookPixelScriptLastPart("Completed", null, null, null);
                        }                        
                        break;

                    case "customer":
                        if (action.Equals("Login", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Login", null, null, null);
                            globalScript += "fbq('track', 'Lead');";
                            globalScript += GetFacebookPixelScriptLastPart("Login", null, null, null);
                        }
                        else if (action.Equals("RegisterResult", StringComparison.InvariantCultureIgnoreCase))
                        {
                            globalScript += GetFacebookPixelScriptFirstPart("Register", null, null, null);
                            globalScript += "fbq('track', 'CompleteRegistration');";
                            globalScript += GetFacebookPixelScriptLastPart("Register", null, null, null);
                        }
                        break;
                    default:
                        globalScript += "";
                        break;
                }
                //globalScript = HttpUtility.HtmlEncode(globalScript);
            }
            catch (Exception ex)
            {
                _logger.InsertLog(Core.Domain.Logging.LogLevel.Error, "Error creating scripts for google adwords convertion tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        private Order GetLastOrder()
        {
            var order = _orderService.SearchOrders(storeId: _storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();
            return order;
        }



        private string GetFacebookPixelScriptFirstPart(string pageType, string productId, string categoryName, IList<ShoppingCartItem> cart)
        {
            FacebookPixelSettings facebookPixelSettings = _settingService.LoadSetting<FacebookPixelSettings>(_storeContext.CurrentStore.Id);
            return InjectValuesInScript(facebookPixelSettings.FacebookPixelScriptFirstPart, "0", pageType, null, productId, categoryName, null, cart);
        }
        private string GetFacebookPixelScriptLastPart(string pageType, string productId, string categoryName, IList<ShoppingCartItem> cart)
        {
            FacebookPixelSettings facebookPixelSettings = _settingService.LoadSetting<FacebookPixelSettings>(_storeContext.CurrentStore.Id);
            return InjectValuesInScript(facebookPixelSettings.FacebookPixelScriptLastPart, "0", pageType, null, productId, categoryName, null, cart);
        }

        private string InjectValuesInScript(string script, string conversionId, string pageType, string label, string productId, string categoryName, Order order, IList<ShoppingCartItem> cart)
        {
            //Set default or empty values
            String totalFormated = ToScriptFormat(0);
            String prodIdsFormated =   productId ;
            //String valuesFormated = "'" + 0 + "'";
            if (productId != null)
            {
                try
                {
                    int id = Int32.Parse(productId);
                    var product = _productService.GetProductById(id);
                    var price = product.PreparePrice(_workContext, _storeContext,
                        _productService, _priceCalculationService,
                        _priceFormatter, _permissionService,
                        _localizationService, _taxService, _currencyService);
                    decimal value = 0;
                    try
                    {
                        value = Convert.ToDecimal(price);
                    }
                    catch (Exception)
                    {
                        value = 0;
                    }

                    totalFormated = ToScriptFormat(value);
                }
                catch
                {
                    prodIdsFormated = "'" + prodIdsFormated + "'";
                }
                
            }
            //In case we have an order
            if (order != null)
            {
                totalFormated = ToScriptFormat(order.OrderTotal);
                prodIdsFormated = ToScriptFormat((from c in order.OrderItems select c.ProductId.ToString()).ToArray());
            }

            //In case we have a cart
            if (cart != null)
            {
                decimal subTotalWithoutDiscountBase = decimal.Zero;
                if (cart.Count > 0)
                {
                    decimal orderSubTotalDiscountAmountBase = decimal.Zero;
                   // Discount orderSubTotalAppliedDiscount = null;
                    List<Discount> orderSubTotalAppliedDiscount;
                    decimal subTotalWithDiscountBase = decimal.Zero;
                    var subTotalIncludingTax = _workContext.TaxDisplayType == TaxDisplayType.IncludingTax;
                    _orderTotalCalculationService.GetShoppingCartSubTotal(cart, subTotalIncludingTax, out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscount, out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                                                                         // (cart, subTotalIncludingTax,out orderSubTotalDiscountAmountBase, out orderSubTotalAppliedDiscounts,out subTotalWithoutDiscountBase, out subTotalWithDiscountBase);
                    totalFormated = ToScriptFormat(subTotalWithoutDiscountBase);
                    prodIdsFormated = ToScriptFormat((from c in cart select c.ProductId.ToString()).ToArray());
                }

               
            }



            if (this.HttpContext.Request.Url.Scheme == "https")
                script = script.Replace("http://", "https://");
           // script = RemoveLastComma(script);
            return script + "\n";
        }

        private string RemoveLastComma(string value)
        {
            int index=0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == ',')
                    index = i;
            }
            if (index > 0)
            {
                value = value.Remove(index, 1);
            }

            return value;
        }

        private string ToScriptFormat(decimal value)
        {
            return value.ToString("0.00", _usCulture);
        }
        private string ToScriptFormat(String[] strings)
        {
            if (strings == null || strings.Length == 0)
            {
                return "''";
            }
            else
            {
                StringBuilder strBuilder = new StringBuilder(strings.Length * 3 + 5);
                //strBuilder.Append("'");
                strBuilder.Append(strings[0]);
                //strBuilder.Append("'");
                if (strings.Length > 1)
                {
                    strBuilder.Insert(0, "[");
                    for (int i = 1; i < strings.Length; i++)
                    {
                        strBuilder.Append(",");
                        strBuilder.Append(strings[i]);
                        //strBuilder.Append("'");
                    }
                    strBuilder.Append("]");
                }
                return strBuilder.ToString();
            }
        }




    }
}
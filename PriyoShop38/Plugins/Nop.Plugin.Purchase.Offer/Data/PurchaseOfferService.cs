using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Cms;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Core.Plugins;
using Nop.Plugin.Purchase.Offer.Domain;
using Nop.Plugin.Purchase.Offer.ViewModel;
using Nop.Services;
using Nop.Services.Catalog;
using Nop.Services.Cms;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.CustomFooter.Data
{
    public class PurchaseOfferService : IPurchaseOfferService
    {
        #region Fields
        //private const string dummyImage = "/Plugins/Purchase.Offer/Content/dummy.jpg";
        //private const string rootPath = "/Plugins/Purchase.Offer/Content";

        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IRepository<PurchaseOffer> _offerRepo;
        private readonly IRepository<PurchaseOfferOption> _optionRepo;
        private readonly IRepository<Product> _productRepo;
        private readonly ICategoryService _categoryService;
        private readonly ILocalizationService _localizationService;
        private readonly IVendorService _vendorService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IStoreService _storeService;
        private readonly ICacheManager _cacheManager;
        private readonly IWidgetService _widgetService;
        private readonly WidgetSettings _widgetSettings;
        private readonly ILogger _logger;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;
        private readonly IPictureService _pictureService;
        private readonly IPriceFormatter _priceFormatter;
        #endregion

        #region CTor
        public PurchaseOfferService(IRepository<PurchaseOffer> offerRepo, IRepository<Product> productRepo,
            IStoreContext storeContext, IWorkContext workContext, IRepository<PurchaseOfferOption> optionRepo,
            ICategoryService categoryService, ICacheManager cacheManager,
            ILocalizationService localizationService, IVendorService vendorService,
            IManufacturerService manufacturerService, IStoreService storeService,
            IWidgetService widgetService, WidgetSettings widgetSettings,
            ILogger logger, IOrderTotalCalculationService orderTotalCalculationService,
            IPictureService pictureService, IPriceFormatter priceFormatter)
        {
            _offerRepo = offerRepo;
            _productRepo = productRepo;
            _storeContext = storeContext;
            _workContext = workContext;
            _optionRepo = optionRepo;
            _categoryService = categoryService;
            _localizationService = localizationService;
            _vendorService = vendorService;
            _manufacturerService = manufacturerService;
            _storeService = storeService;
            _cacheManager = cacheManager;
            _categoryService = categoryService;
            _widgetService = widgetService;
            _widgetSettings = widgetSettings;
            _logger = logger;
            _orderTotalCalculationService = orderTotalCalculationService;
            _pictureService = pictureService;
            _priceFormatter = priceFormatter;
        }
        #endregion

        #region Methods
        public bool CreateOrUpdateOffer(PurchaseOfferViewModel data)
        {
            try
            {
                var offer = _offerRepo.Table.FirstOrDefault();
                if (offer == null)
                {
                    var obj = new PurchaseOffer()
                    {
                        CreatedOnUtc = DateTime.UtcNow,
                        Name = data.Name,
                        UpdatedOnUtc = DateTime.UtcNow,
                        ValidFrom = data.ValidFrom,
                        ValidTo = data.ValidTo,
                        Description = data.Description,
                        IsActive = data.IsActive,
                        ShowNotificationOnCart = data.ShowNotificationOnCart
                    };
                    _offerRepo.Insert(obj);
                }
                else
                {
                    offer.Name = data.Name;
                    offer.UpdatedOnUtc = DateTime.UtcNow;
                    offer.ValidFrom = data.ValidFrom;
                    offer.ValidTo = data.ValidTo;
                    offer.Description = data.Description;
                    offer.IsActive = data.IsActive;
                    offer.ShowNotificationOnCart = data.ShowNotificationOnCart;
                    _offerRepo.Update(offer);
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return false;
            }
        }

        public PurchaseOfferViewModel GetOfferDetails()
        {
            try
            {
                var offer = _offerRepo.Table.FirstOrDefault();
                var data = new PurchaseOfferViewModel();
                if (offer != null)
                {
                    data.IsActive = offer.IsActive;
                    data.CreatedOnUtc = offer.CreatedOnUtc;
                    data.Description = offer.Description;
                    data.Id = offer.Id;
                    data.UpdatedOnUtc = offer.UpdatedOnUtc;
                    data.Name = offer.Name;
                    data.ValidFrom = offer.ValidFrom;
                    data.ValidTo = offer.ValidTo;
                    data.ShowNotificationOnCart = offer.ShowNotificationOnCart;
                }
                return data;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new PurchaseOfferViewModel();
            }
        }

        public List<PurchaseOfferOptionViewModel> GetOptions()
        {
            try
            {
                var data = _optionRepo.Table.OrderBy(x => x.MinimumPurchaseAmount);
                var model = new List<PurchaseOfferOptionViewModel>();
                foreach (var item in data)
                {
                    var picture = _pictureService.GetPictureById(item.PictureId);
                    model.Add(new PurchaseOfferOptionViewModel()
                    {
                        MinimumPurchaseAmount = item.MinimumPurchaseAmount,
                        Note = item.Note,
                        ProductImage = _pictureService.GetPictureUrl(picture, 75),
                        Quantity = item.Quantity,
                        ProductName = item.ProductName,
                        Id = item.Id,
                        MinimumPurchaseAmountStr = _priceFormatter.FormatPrice(item.MinimumPurchaseAmount)
                    });
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return new List<PurchaseOfferOptionViewModel>();
            }
        }

        //private bool IsValidPicture(string productImage)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(productImage))
        //        {
        //            string absolute = Path.Combine(HttpContext.Current.Server.MapPath(rootPath), productImage);
        //            if (File.Exists(absolute))
        //                return true;
        //        }
        //    }
        //    catch { }
        //    return false;
        //}

        public AddOptionModel GetPopUpModel(int? optionId)
        {
            var model = new AddOptionModel();
            //model.AvailableCategories.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });

            //var categoryListItems = _categoryService.GetAllCategories();
            //var categories = new List<SelectListItem>();
            //foreach (var item in categoryListItems)
            //{
            //    categories.Add(new SelectListItem
            //    {
            //        Text = item.Name,
            //        Value = item.Id.ToString()
            //    });
            //}

            if (optionId.HasValue)
            {
                var option = _optionRepo.GetById(optionId);
                if (option != null)
                {
                    var picture = _pictureService.GetPictureById(option.PictureId);
                    model.ProductImage = _pictureService.GetPictureUrl(picture, 75);
                    //if (!string.IsNullOrWhiteSpace(option.ProductImage))
                    //{
                    //    string filePath = Path.Combine(HttpContext.Current.Server.MapPath(rootPath), option.ProductImage);
                    //    model.ProductImage = File.Exists(filePath) ? rootPath + "/" + option.ProductImage : dummyImage;
                    //}
                    model.ProductName = option.ProductName;
                    model.Note = option.Note;
                    model.Quantity = option.Quantity;
                    model.Id = option.Id;
                    model.PictureId = option.PictureId;
                    model.MinimumPurchaseAmount = option.MinimumPurchaseAmount;
                }
            }

            //foreach (var c in categories)
            //    model.AvailableCategories.Add(c);

            //model.AvailableManufacturers.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var m in _manufacturerService.GetAllManufacturers(showHidden: true))
            //    model.AvailableManufacturers.Add(new SelectListItem { Text = m.Name, Value = m.Id.ToString() });

            //model.AvailableStores.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var s in _storeService.GetAllStores())
            //    model.AvailableStores.Add(new SelectListItem { Text = s.Name, Value = s.Id.ToString() });

            //model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            //foreach (var v in _vendorService.GetAllVendors(showHidden: true))
            //    model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            //model.AvailableProductTypes = ProductType.SimpleProduct.ToSelectList(false).ToList();
            //model.AvailableProductTypes.Insert(0, new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            return model;
        }

        public void AddOrUpdateOption(AddOptionModel model)
        {
            try
            {
                //string filePath = SaveFile(model.ImageBaseFile);
                if (model.Id > 0)
                {
                    var option = _optionRepo.GetById(model.Id);
                    option.MinimumPurchaseAmount = model.MinimumPurchaseAmount;
                    option.Note = model.Note;
                    option.PictureId = model.PictureId;
                    option.Quantity = model.Quantity;
                    option.ProductName = model.ProductName;
                    //if (!string.IsNullOrWhiteSpace(filePath))
                    //{
                    //    DeleteOldImage(option.ProductImage);
                    //    option.ProductImage = filePath;
                    //}
                    _optionRepo.Update(option);
                }
                else
                {
                    var option = new PurchaseOfferOption()
                    {
                        MinimumPurchaseAmount = model.MinimumPurchaseAmount,
                        Note = model.Note,
                        Quantity = model.Quantity,
                        //ProductImage = filePath,
                        ProductName = model.ProductName,
                        PictureId = model.PictureId
                    };
                    _optionRepo.Insert(option);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        //private static void DeleteOldImage(string img)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrWhiteSpace(img))
        //        {
        //            string oldPath = Path.Combine(HttpContext.Current.Server.MapPath(rootPath), img);
        //            if (File.Exists(oldPath))
        //                File.Delete(oldPath);
        //        }
        //    }
        //    catch { }
        //}

        //private string SaveFile(HttpPostedFileBase imageBaseFile)
        //{
        //    try
        //    {
        //        var ext = Path.GetExtension(imageBaseFile.FileName).ToLower();
        //        if (ext == ".jpg" || ext == ".png")
        //        {
        //            var file = Guid.NewGuid().ToString() + Path.GetExtension(imageBaseFile.FileName);
        //            string path = Path.Combine(HttpContext.Current.Server.MapPath(rootPath), file);

        //            imageBaseFile.SaveAs(path);
        //            return file;
        //        }
        //    }
        //    catch { }
        //    return "";
        //}

        public void DeleteOption(int id)
        {
            try
            {
                _optionRepo.Delete(_optionRepo.GetById(id));
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
        }

        public PurchaseOfferOptionViewModel GetPublicInfo()
        {
            try
            {
                var total = GetCartTotal();

                var model = new PurchaseOfferOptionViewModel();
                var offer = _offerRepo.Table.Where(x => x.IsActive && x.ValidFrom <= DateTime.UtcNow && x.ValidTo >= DateTime.UtcNow).FirstOrDefault();
                if (offer != null)
                {
                    model.ShowNotificationOnCart = offer.ShowNotificationOnCart;
                    var options = _optionRepo.Table.OrderByDescending(x => x.MinimumPurchaseAmount);
                    var option = options.Where(x => x.MinimumPurchaseAmount <= total).FirstOrDefault();
                    model.OfferActive = true;

                    if (option != null)
                    {
                        var picture = _pictureService.GetPictureById(option.PictureId);
                        model.ProductImage = _pictureService.GetPictureUrl(picture, 75);

                        //model.ProductImage = dummyImage;
                        //if (!string.IsNullOrWhiteSpace(option.ProductImage))
                        //{
                        //    string filePath = Path.Combine(HttpContext.Current.Server.MapPath(rootPath), option.ProductImage);
                        //    model.ProductImage = File.Exists(filePath) ? rootPath + "/" + option.ProductImage : dummyImage;
                        //}
                        model.ProductName = option.ProductName;
                        model.Note = option.Note;
                        model.Quantity = option.Quantity;
                        model.Id = option.Id;
                        model.MinimumPurchaseAmount = option.MinimumPurchaseAmount;
                        model.MinimumPurchaseAmountStr = _priceFormatter.FormatPrice(option.MinimumPurchaseAmount);
                        model.OfferAvailable = true;
                        model.OfferName = offer.Name;

                        var nextOption = options.OrderBy(x => x.MinimumPurchaseAmount).Where(x => x.MinimumPurchaseAmount > total).FirstOrDefault();
                        if (nextOption != null)
                            model.Message = "Purchase <span style='color:indianred'>" + _priceFormatter.FormatPrice(nextOption.MinimumPurchaseAmount - total) + "</span> more (Total <span>" + nextOption.MinimumPurchaseAmount.ToString("N0") + " Tk</span>) to get <span style='color:indianred'>" + nextOption.ProductName + "</span> for free.";
                    }
                    else
                    {
                        var nextOption = options.OrderBy(x => x.MinimumPurchaseAmount).FirstOrDefault();
                        if (nextOption != null)
                            model.Message = "Purchase <span style='color:indianred'>" + _priceFormatter.FormatPrice(nextOption.MinimumPurchaseAmount - total) + "</span> more (Total <span>" + nextOption.MinimumPurchaseAmount.ToString("N0") + " Tk</span>) to get <span style='color:indianred'>" + nextOption.ProductName + "</span> for free.";
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }
            return null;
        }

        private decimal GetCartTotal()
        {
            var cart = _workContext.CurrentCustomer.ShoppingCartItems
                        .Where(sci => sci.ShoppingCartType == ShoppingCartType.ShoppingCart)
                        .LimitPerStore(_storeContext.CurrentStore.Id)
                        .ToList();
            _orderTotalCalculationService.GetShoppingCartSubTotal(cart, true, out _, out _, out _, out decimal total);
            return total;
        }
        #endregion
    }
}
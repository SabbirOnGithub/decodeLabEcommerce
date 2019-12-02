using Nop.Admin.Models.Catalog;
using Nop.Services.Security;
using Nop.Services.Vendors;
using System.Web.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Web.Framework.Kendoui;
using System.Linq;
using Nop.Core.Domain.Catalog;
using Nop.Admin.Extensions;
using Nop.Core;
using Nop.Services.Media;

namespace Nop.Admin.Controllers
{
    public class ProductUnpublishRequestByVendorController : BaseAdminController
    {
        #region Fields
        private readonly IPermissionService _permissionService;
        private readonly IVendorService _vendorService;
        private readonly ILocalizationService _localizationService;
        private readonly IProductUnpublishRequestByVendorService _productUnpublishRequestByVendorService;
        private readonly IPictureService _pictureService;
        private readonly IWorkContext _workContext;
        #endregion

        #region Ctor

        public ProductUnpublishRequestByVendorController(
            IPermissionService permissionService,
            IVendorService vendorService,
            ILocalizationService localizationService,
            IProductUnpublishRequestByVendorService productUnpublishRequestByVendorService,
            IPictureService pictureService,
            IWorkContext workContext
            )
        {
            this._permissionService = permissionService;
            this._vendorService = vendorService;
            this._localizationService = localizationService;
            this._productUnpublishRequestByVendorService = productUnpublishRequestByVendorService;
            this._pictureService = pictureService;
            this._workContext = workContext;
        }
        #endregion

        #region Utilities

        protected virtual ProductUnpublishRequestByVendorModel PrepareUnpublishRequestModel(
            ProductUnpublishRequestByVendor unpublishRequest)
        {
            var model = new ProductUnpublishRequestByVendorModel();

            if(unpublishRequest.Product.VendorId > 0)
                model.Vendor = _vendorService.GetVendorById(unpublishRequest.Product.VendorId).ToModel();

            model.Product = unpublishRequest.Product.ToModel();
            model.Product.FullDescription = "";
            var defaultProductPicture = _pictureService.GetPicturesByProductId(unpublishRequest.ProductId, 1).FirstOrDefault();
            model.Product.PictureThumbnailUrl = _pictureService.GetPictureUrl(defaultProductPicture, 75, true);
            //product type
            model.Product.ProductTypeName = unpublishRequest.Product.ProductType.GetLocalizedEnum(_localizationService, _workContext);
            //friendly stock qantity
            //if a simple product AND "manage inventory" is "Track inventory", then display
            if (unpublishRequest.Product.ProductType == ProductType.SimpleProduct && unpublishRequest.Product.ManageInventoryMethod == ManageInventoryMethod.ManageStock)
                model.Product.StockQuantityStr = unpublishRequest.Product.GetTotalStockQuantity().ToString();

            model.CreatedOnUtc = unpublishRequest.CreatedOnUtc;

            return model;
        }
        #endregion

        #region Action Methods
        // GET: ProductUnpublishRequestByVendor
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorProductsActivity))
                return AccessDeniedView();

            var model = new ProductUnpublishRequestByVendorListModel();

            //vendors
            model.AvailableVendors.Add(new SelectListItem { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            foreach (var v in _vendorService.GetAllVendors(showHidden: true))
                model.AvailableVendors.Add(new SelectListItem { Text = v.Name, Value = v.Id.ToString() });

            return View(model);
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, ProductUnpublishRequestByVendorListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageVendorProductsActivity))
                return AccessDeniedView();

            var unpublishRequests = _productUnpublishRequestByVendorService.SearchUnpublishRequests(
                vendorId: model.SearchVendorId,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize
            );

            var gridModel = new DataSourceResult();
            gridModel.Data = unpublishRequests.Select(x =>
            {
                return PrepareUnpublishRequestModel(x);
            });
            gridModel.Total = unpublishRequests.TotalCount;

            return Json(gridModel);
        }
        #endregion
    }
}
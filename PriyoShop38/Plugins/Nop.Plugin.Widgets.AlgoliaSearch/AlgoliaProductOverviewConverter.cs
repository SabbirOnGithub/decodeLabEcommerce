using Nop.Core.Domain.Catalog;
using Nop.Plugin.Widgets.AlgoliaSearch.Models.AlgoliaProduct;
using Nop.Services.Catalog;
using Nop.Services.Media;
using Nop.Services.Seo;
using Nop.Web.Models.Catalog;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Plugin.Widgets.AlgoliaSearch
{
    public static class AlgoliaProductOverviewConverter
    {
        public static AlgoliaProductOverviewModel ToAlgoliaModel(
            this ProductOverviewModel model, 
            Product product,
            IPictureService pictureService,
            ICategoryService categoryService,
            IManufacturerService manufacturerService,
            AlgoliaSettings algoliaSettings)
        {
            var algoliaModel = new AlgoliaProductOverviewModel(model);

            algoliaModel.Specifications = algoliaModel.SpecificationAttributeModels.ToAlgoliaModel();
            algoliaModel.DisableBuyButton = product.DisableBuyButton;
            algoliaModel.DisableWishlistButton = product.DisableWishlistButton;
            algoliaModel.objectID = product.Id.ToString();
            algoliaModel.Categories = product.GetCategoryModel(categoryService);
            algoliaModel.Price = product.Price;
            algoliaModel.OldPrice = product.OldPrice;
            algoliaModel.FullDescription = string.Empty;
            algoliaModel.Vendor = product.PrepareVendorModel();
            algoliaModel.Manufacturers = product.PrepareManufacturerModel(manufacturerService);
            algoliaModel.EnableEmi = product.EnableEmi;
            algoliaModel.Sku = product.Sku;
            algoliaModel.SoldOut = product.GetTotalStockQuantity() < 1;

            if (model.ReviewOverviewModel.TotalReviews > 0)
            {
                algoliaModel.Rating = model.ReviewOverviewModel.RatingSum / model.ReviewOverviewModel.TotalReviews;
            }

            var picture = pictureService.GetPicturesByProductId(product.Id, 1).FirstOrDefault();
            algoliaModel.AutoCompleteImageUrl = pictureService.GetPictureUrl(picture, algoliaSettings.SearchBoxThumbnailSize);

            return algoliaModel;
        }

        private static List<AlgoliaManufacturerModel> PrepareManufacturerModel(this Product product, IManufacturerService manufacturerService)
        {
            int productId = product.Id;
            var manufacturers = new List<AlgoliaManufacturerModel>();
            var productManufacturers = manufacturerService.GetProductManufacturersByProductId(productId);

            if (productManufacturers != null && productManufacturers.Count > 0)
            {
                foreach (var productManufacturer in productManufacturers)
                {
                    var manf = productManufacturer.Manufacturer;
                    if (manf == null)
                        manf = manufacturerService.GetManufacturerById(productManufacturer.ManufacturerId);

                    manufacturers.Add(new AlgoliaManufacturerModel()
                    {
                        Name = manf.Name,
                        SeName = manf.GetSeName(),
                        Id = productManufacturer.ManufacturerId,
                        IdName = productManufacturer.ManufacturerId + "--" + manf.Name
                    });
                }
            }
            return manufacturers;
        }

        private static AlgoliaVendorModel PrepareVendorModel(this Product product)
        {
            var vendorModel = new AlgoliaVendorModel();
            var vendorService = EngineContext.Current.Resolve<IVendorService>();

            var vendor = vendorService.GetVendorById(product.VendorId);
            if (vendor != null)
            {
                vendorModel.Id = product.VendorId;
                vendorModel.SeName = vendor.GetSeName();
                vendorModel.Name = vendor.Name;
                vendorModel.IdName = vendor.Id + "--" + vendor.Name;
            }

            return vendorModel;
        }

        private static IList<AlgoliaCategoryModel> GetCategoryModel(this Product product, 
            ICategoryService categoryService)
        {
            var categoryModel = new List<AlgoliaCategoryModel>();
            var productCategories = categoryService.GetProductCategoriesByProductId(product.Id);

            foreach (var productCategory in productCategories)
            {
                var cat = productCategory.Category;
                categoryModel.Add(new AlgoliaCategoryModel()
                {
                    Name = cat.Name,
                    SeName = cat.GetSeName(),
                    Id = cat.Id,
                    NameSeName = cat.Name + "--" + cat.GetSeName(),
                    IdName = cat.Id + "--" + cat.Name
                });
            }
            return categoryModel;
        }

        public static IList<AlgoliaSpecificationModel> ToAlgoliaModel(this IList<ProductSpecificationModel> specifications)
        {
            var specModel = new List<AlgoliaSpecificationModel>();
            if (specifications != null && specifications.Count > 0)
            {
                foreach (var specification in specifications)
                {
                    var sp = new AlgoliaSpecificationModel(specification);

                    sp.IdOption = specification.SpecificationAttributeId + "--" + specification.ValueRaw;
                    sp.IdOptionGroup = specification.SpecificationAttributeId + "--" + specification.ValueRaw + "--" + specification.SpecificationAttributeName;
                    specModel.Add(sp);
                }
            }
            return specModel;
        }
    }
}

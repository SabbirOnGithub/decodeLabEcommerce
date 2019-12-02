using System.Linq;
using Nop.Core.Domain.Orders;
using Nop.Core.Infrastructure;
using Nop.Plugin.Payments.EblSkyPay.Models;
using Nop.Services.Vendors;

namespace Nop.Plugin.Payments.EblSkyPay.Extensions
{
    //here we have some methods shared between payment methods plugins
    public static class OrderExtensions
    {
        public static DataLayerTrxModel ToDataLayerTrxModel(this Order order, string list = "")
        {
            var model = new DataLayerTrxModel();
            var vendorService = EngineContext.Current.Resolve<IVendorService>();

            model.OrderId = order.Id;
            model.List = list;

            foreach (var x in order.OrderItems)
            {
                var vendor = vendorService.GetVendorById(x.Product.VendorId);
                model.Products.Add(new MinProductModel()
                {
                    Id = x.Product.Id,
                    Name = x.Product.Name,
                    CategoryName = x.Product.ProductCategories.FirstOrDefault()?.Category.Name,
                    ManufacturerName = x.Product.ProductManufacturers.FirstOrDefault()?.Manufacturer.Name,
                    VendorName = vendor == null ? "" : vendor.Name,
                    ProductCost = x.OriginalProductCost,
                    Price = x.PriceInclTax,
                    Quantity = x.Quantity
                });
            }
            return model;
        }
    }
}

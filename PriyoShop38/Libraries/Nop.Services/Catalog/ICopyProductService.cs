
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    /// <summary>
    /// Copy product service
    /// </summary>
    public partial interface ICopyProductService
    {
        /// <summary>
        /// Create a copy of product with all depended data
        /// </summary>
        /// <param name="product">The product to copy</param>
        /// <param name="newName">The name of product duplicate</param>
        /// <param name="copyPriceProperties"> Indicating whether to copy all the properties related to price</param>
        /// <param name="isPublished">A value indicating whether the product duplicate should be published</param>
        /// <param name="copyImages">A value indicating whether the product images should be copied</param>
        /// <param name="copyAssociatedProducts">A value indicating whether the copy associated products</param>
        /// <returns>Product copy</returns>
        Product CopyProduct(Product product, string newName, bool isPublished = true,
            bool copyImages = true, bool copyAssociatedProducts = true, bool copyPriceProperties = true,
            decimal newPrice = 0, decimal newProductCost = 0);
    }
}

using Nop.Core;
using Nop.Core.Domain.Catalog;

namespace Nop.Services.Catalog
{
    public partial interface IProductUnpublishRequestByVendorService
    {
        void InsertUnpublishRequest(ProductUnpublishRequestByVendor unpublishRequest);
        ProductUnpublishRequestByVendor GetUnpublishRequestProductByVendor(int vendorId, int productId);
        IPagedList<ProductUnpublishRequestByVendor> SearchUnpublishRequests(
            int vendorId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue);
        void DeleteUnpublishRequest(ProductUnpublishRequestByVendor unpublishRequest);
    }
}

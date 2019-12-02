using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Events;
using System;
using System.Linq;

namespace Nop.Services.Catalog
{
    public partial class ProductUnpublishRequestByVendorService : IProductUnpublishRequestByVendorService
    {
        #region Fields
        private readonly IRepository<ProductUnpublishRequestByVendor> _unpublishRequestRipository;
        private readonly IEventPublisher _eventPublisher;
        #endregion

        #region Ctor

        public ProductUnpublishRequestByVendorService(
            IRepository<ProductUnpublishRequestByVendor> unpublishRequestRipository,
            IEventPublisher eventPublisher
            )
        {
            this._unpublishRequestRipository = unpublishRequestRipository;
            this._eventPublisher = eventPublisher;
        }
        #endregion

        #region Methods

        public virtual void InsertUnpublishRequest(ProductUnpublishRequestByVendor unpublishRequest)
        {
            if (unpublishRequest == null)
                throw new ArgumentNullException("unpublishRequest");

            //insert
            _unpublishRequestRipository.Insert(unpublishRequest);

            //event notification
            _eventPublisher.EntityInserted(unpublishRequest);
        }

        public virtual ProductUnpublishRequestByVendor GetUnpublishRequestProductByVendor(int vendorId, int productId)
        {
            if (productId == 0 && vendorId == 0)
                return null;

            var query = _unpublishRequestRipository.Table
                .Where(ur => ur.Product.VendorId == vendorId && ur.ProductId == productId);

            var unpublishRequest = query.FirstOrDefault();

            return unpublishRequest;
        }

        public virtual IPagedList<ProductUnpublishRequestByVendor> SearchUnpublishRequests(
            int vendorId = 0,
            int pageIndex = 0,
            int pageSize = int.MaxValue)
        {
            var query = _unpublishRequestRipository.Table;

            if (vendorId > 0)
                query = query.Where(ur => ur.Product.VendorId == vendorId);

            query = query.OrderBy(ur => ur.Product.VendorId)
                .ThenBy(ur => ur.CreatedOnUtc)
                .ThenBy(ur => ur.Product.Name);

            var unpublishRequests = new PagedList<ProductUnpublishRequestByVendor>(query, pageIndex, pageSize);

            return unpublishRequests;
        }

        public virtual void DeleteUnpublishRequest(ProductUnpublishRequestByVendor unpublishRequest)
        {
            if (unpublishRequest == null)
                throw new ArgumentNullException("unpublishRequest");

            //delete from database
            _unpublishRequestRipository.Delete(unpublishRequest);

            //event notification
            _eventPublisher.EntityDeleted(unpublishRequest);
        }
        #endregion
    }
}

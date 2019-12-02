using Nop.Core;
using Nop.Core.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class BsNopMobilePluginService : IBsNopMobilePluginService
    {
        #region Fields

        private readonly IRepository<BS_FeaturedProducts> _fetureProductRepository;
       

        #endregion

        #region Ctor

        public BsNopMobilePluginService(IRepository<BS_FeaturedProducts> fetureProductRepository)
        {
            this._fetureProductRepository = fetureProductRepository;          
        }

        #endregion

        #region Methods

        public virtual IPagedList<BS_FeaturedProducts> GetAllPluginFeatureProducts(int pageIndex = 0, int pageSize = int.MaxValue)
        {
            var featureproduct = _fetureProductRepository.Table.ToList();
            return new PagedList<BS_FeaturedProducts>(featureproduct, pageIndex, pageSize);
        }

        public virtual BS_FeaturedProducts GetPluginFeatureProductsById(int FeatureProductsId)
        {
            if (FeatureProductsId == 0)
                return null;

            return _fetureProductRepository.Table.Where(x=>x.ProductId==FeatureProductsId).FirstOrDefault();
        }
        
        public virtual void DeleteFeatureProducts(BS_FeaturedProducts FeatureProducts)
        {
            if (FeatureProducts == null)
                throw new ArgumentNullException("FeatureProducts");

            _fetureProductRepository.Delete(FeatureProducts);
        }

        public virtual void InsertFeatureProducts(BS_FeaturedProducts FeatureProducts)
        {
            if (FeatureProducts == null)
                throw new ArgumentNullException("FeatureProducts");

            _fetureProductRepository.Insert(FeatureProducts);
        }

        public virtual void UpdateFeatureProducts(BS_FeaturedProducts FeatureProducts)
        {
            if (FeatureProducts == null)
                throw new ArgumentNullException("FeatureProducts");

            _fetureProductRepository.Update(FeatureProducts);
        }
        #endregion
    }
}

using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Domain;


namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial interface IBsNopMobilePluginService
    {
        IPagedList<BS_FeaturedProducts> GetAllPluginFeatureProducts(int pageIndex = 0, int pageSize = int.MaxValue);

        BS_FeaturedProducts GetPluginFeatureProductsById(int FeatureProductsId);

        void InsertFeatureProducts(BS_FeaturedProducts FeatureProducts);

        void UpdateFeatureProducts(BS_FeaturedProducts FeatureProducts);

        void DeleteFeatureProducts(BS_FeaturedProducts FeatureProducts);
    }
}

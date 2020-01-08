using Nop.Core;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface ICategoryIconService
    {
        IPagedList<BS_CategoryIcon> GetAllCategoryIcons(int pageIndex = 0, int pageSize = int.MaxValue);

        BS_CategoryIcon GetCategoryIconByCategoryId(int CategoryId);

        BS_CategoryIcon GetCategoryIconById(int id);

        void InsertCategoryIcon(BS_CategoryIcon CategoryIcons);

        void UpdateCategoryIcon(BS_CategoryIcon CategoryIcons);

        void DeleteCategoryIcon(BS_CategoryIcon CategoryIcons);
    }
}

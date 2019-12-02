using Nop.Core.Domain.Catalog;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public interface IThemeDataService
    {
        IList<Category> PopularCategories();
    }
}

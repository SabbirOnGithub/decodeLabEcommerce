using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Services.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Data;
using System.Data;
using Nop.Core.Data;
using Nop.Services.Logging;

namespace BS.Plugin.NopStation.MobileWebApi.Services
{
    public partial class ThemeDataService : IThemeDataService
    {
        private readonly MobileWebApiObjectContext _context;
        private readonly IDataProvider _dataProvider;
        private readonly ILogger _logger;
        private readonly ICategoryService _categoryService;

        public ThemeDataService(MobileWebApiObjectContext context,
            IDataProvider dataProvider,
            ILogger logger,
            ICategoryService categoryService)
        {
            this._context = context;
            this._dataProvider = dataProvider;
            this._logger = logger;
            this._categoryService = categoryService;
        }

        public virtual IList<Category> PopularCategories()
        {
            try
            {
                var pEntityId = _dataProvider.GetParameter();
                pEntityId.ParameterName = "EntityId";
                pEntityId.Value = 17;
                pEntityId.DbType = DbType.Int32;

                var pEntityType = _dataProvider.GetParameter();
                pEntityType.ParameterName = "EntityType";
                pEntityType.Value = 35;
                pEntityType.DbType = DbType.Int32;

                var categoryIds =
                    _context.SqlQuery<int>(
                        @"SELECT MappedEntityId FROM SS_MAP_EntityMapping WHERE EntityId = @EntityId AND EntityType = @EntityType",
                        pEntityId, pEntityType);

                var categories = categoryIds
                    .Select(x => _categoryService.GetCategoryById(x))
                    .ToList();

                return categories;
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return null;
            }            
        }
    }
}

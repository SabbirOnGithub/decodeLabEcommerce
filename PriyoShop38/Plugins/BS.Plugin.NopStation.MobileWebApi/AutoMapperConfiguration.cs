using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BS.Plugin.NopStation.MobileWebApi.Models.Catalog;
using Nop.Core.Domain.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using BS.Plugin.NopStation.MobileWebApi.Models._QueryModel.Customer;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Customer;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public static class AutoMapperConfiguration
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;

        public static void Init()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Category, CategoryNavigationModelApi>();

                cfg.CreateMap<Category, CategoryOverViewModelApi>();
                cfg.CreateMap<CategoryModelApi, CategoryDetailFeaturedProductAndSubcategoryResponseModel>();
                cfg.CreateMap<Manufacturer, MenufactureOverViewModelApi>();
                cfg.CreateMap<ManuFactureModelApi, ManufacturerDetailFeaturedProductResponseModel>();
                cfg.CreateMap<CustomerInfoQueryModel, CustomerInfoResponseModel>();
                cfg.CreateMap<Manufacturer, MenuFacturerModelShortDetailApi>();
                cfg.CreateMap<Manufacturer, MenufactureOverViewModelApi>();
            });
            _mapper = _mapperConfiguration.CreateMapper();

        }

        public static IMapper Mapper
        {
            get
            {
                return _mapper;
            }
        }
        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration
        {
            get
            {
                return _mapperConfiguration;
            }
        }
    }
}

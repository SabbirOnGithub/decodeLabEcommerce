using Autofac;
using Autofac.Core;
using Nop.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using BS.Plugin.NopStation.MobileWebApi.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Services.Common;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_MobileWebApi";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder,NopConfig config)
        {

            this.RegisterPluginDataContext<MobileWebApiObjectContext>(builder, CONTEXT_NAME);
            builder.RegisterType<ProductServiceApi>().As<IProductServiceApi>().InstancePerLifetimeScope();
            builder.RegisterType<CustomerServiceApi>().As<ICustomerServiceApi>().InstancePerLifetimeScope();
            builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<BS_SliderService>().As<IBS_SliderService>().InstancePerLifetimeScope();

            //seven spikes theme data
            builder.RegisterType<ThemeDataService>().As<IThemeDataService>().InstancePerLifetimeScope();

            //work context
            builder.RegisterType<ApiWebWorkContext>().As<IWorkContext>().InstancePerLifetimeScope();
            builder.RegisterType<ExternalAuthorizerApi>().As<IExternalAuthorizerApi>().InstancePerLifetimeScope();

            //data context
            builder.RegisterType<GenericAttributeServiceApi>().As<IGenericAttributeService>().InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<Device>>()
                .As<IRepository<Device>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_Slider>>()
                .As<IRepository<BS_Slider>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 4; }
        }

      

        
    }
}

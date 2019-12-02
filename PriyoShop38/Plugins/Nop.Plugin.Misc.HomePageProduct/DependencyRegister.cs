using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.HomePageProduct.Data;
using Nop.Plugin.Misc.HomePageProduct.Domain;
using Nop.Plugin.Misc.HomePageProduct.Services;

namespace Nop.Plugin.Misc.HomePageProduct
{
    public partial class DependencyRegister : IDependencyRegistrar
    {
        #region Field

        private const string ContextName = "nop_object_context_homepage_product";

        #endregion

        #region Register

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //Load custom data settings
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();

            //Register custom object context
            builder.Register<IDbContext>(c => RegisterIDbContext(c, dataSettings)).Named<IDbContext>(ContextName).InstancePerHttpRequest();
            builder.Register(c => RegisterIDbContext(c, dataSettings)).InstancePerHttpRequest();

            //Register services
            builder.RegisterType<HomePageProductService>().As<IHomePageProductService>();
            builder.RegisterType<HomePageProductCategoryImageService>().As<IHomePageProductCategoryImageService>();
            builder.RegisterType<HomePageCategoryService>().As<IHomePageCategoryService>();
            builder.RegisterType<HomePageSubCategoryService>().As<IHomePageSubCategoryService>();

            
            //Override the repository injection
            builder.RegisterType<EfRepository<HomePageProductCategory>>().As<IRepository<HomePageProductCategory>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName)).InstancePerHttpRequest();
            builder.RegisterType<EfRepository<HomePageProductCategoryImage>>().As<IRepository<HomePageProductCategoryImage>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName)).InstancePerHttpRequest();
            builder.RegisterType<EfRepository<HomePageCategory>>().As<IRepository<HomePageCategory>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName)).InstancePerHttpRequest();
            builder.RegisterType<EfRepository<HomePageSubCategory>>().As<IRepository<HomePageSubCategory>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName)).InstancePerHttpRequest();


        }

        #endregion

        #region DB

        public int Order
        {
            get { return 0; }
        }

        private HomePageProductObjectContext RegisterIDbContext(IComponentContext componentContext,
                                                                DataSettings dataSettings)
        {
            string dataConnectionStrings;

            if (dataSettings != null && dataSettings.IsValid())
            {
                dataConnectionStrings = dataSettings.DataConnectionString;
            }
            else
            {
                dataConnectionStrings = componentContext.Resolve<DataSettings>().DataConnectionString;
            }

            return new HomePageProductObjectContext(dataConnectionStrings);
        }

        #endregion

       
    }
}
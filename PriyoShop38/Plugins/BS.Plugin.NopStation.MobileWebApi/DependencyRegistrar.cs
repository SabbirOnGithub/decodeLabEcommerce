using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using BS.Plugin.NopStation.MobileWebApi.Data;
using BS.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Services;
using Nop.Web.Framework.Mvc;
using Nop.Core.Caching;
using BS.Plugin.NopStation.MobileWebApi.Controllers;
using BS.Plugin.NopStation.MobileWebApi.Services.Agent;
using BS.Plugin.NopStation.MobileWebApi.Domain.Agent;

namespace BS.Plugin.NopStation.MobileWebApi
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_mobilewebapi";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BsNopMobilePluginService>().As<IBsNopMobilePluginService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementService>().As<IContentManagementService>().InstancePerLifetimeScope();
            builder.RegisterType<ContentManagementTemplateService>().As<IContentManagementTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<CategoryIconService>().As<ICategoryIconService>().InstancePerLifetimeScope();
            builder.RegisterType<BS_RecentlyViewedProductsApiService>().As<IBS_RecentlyViewedProductsApiService>().InstancePerLifetimeScope();
            builder.RegisterType<BS_HomePageCategoryService>().As<IBS_HomePageCategoryService>().InstancePerLifetimeScope();

            #region Agent
            builder.RegisterType<AgentService>().As<IAgentService>().InstancePerLifetimeScope();
            #endregion

            //data context
            this.RegisterPluginDataContext<MobileWebApiObjectContext>(builder, CONTEXT_NAME);

            //override required repository with our custom context
            builder.RegisterType<EfRepository<BS_FeaturedProducts>>()
                .As<IRepository<BS_FeaturedProducts>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagement>>()
             .As<IRepository<BS_ContentManagement>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_ContentManagementTemplate>>()
             .As<IRepository<BS_ContentManagementTemplate>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_CategoryIcon>>()
             .As<IRepository<BS_CategoryIcon>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_RecentlyViewedProductsApi>>()
                .As<IRepository<BS_RecentlyViewedProductsApi>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_HomePageCategory>>()
                .As<IRepository<BS_HomePageCategory>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<BS_HomePageCategoryProduct>>()
                .As<IRepository<BS_HomePageCategoryProduct>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            #region Agent
            builder.RegisterType<EfRepository<AgentMasterInformationTemp>>()
             .As<IRepository<AgentMasterInformationTemp>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AgentImageInformationTemp>>()
             .As<IRepository<AgentImageInformationTemp>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();
            #endregion
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

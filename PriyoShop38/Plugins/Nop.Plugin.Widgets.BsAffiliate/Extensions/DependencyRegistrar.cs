using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.BsAffiliate.Domain;
using Nop.Plugin.Widgets.BsAffiliate.Services;
using Nop.Web.Framework.Mvc;
using System.Reflection;

namespace Nop.Plugin.Widgets.BsAffiliate.Extensions
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_affiliate";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            this.RegisterPluginDataContext<BsAffiliateObjectContext>(builder, CONTEXT_NAME);
            builder.RegisterType<AffiliateConfigureService>().As<IAffiliateConfigureService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliatePublicService>().As<IAffiliatePublicService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateCustomerMapService>().As<IAffiliateCustomerMapService>().InstancePerLifetimeScope();
            builder.RegisterType<AffiliateCommissionRateService>().As<IAffiliateCommissionRateService>().InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AffiliateUserCommission>>()
                .As<IRepository<AffiliateUserCommission>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AffiliatedOrderCommission>>()
                .As<IRepository<AffiliatedOrderCommission>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AffiliateCustomerMapping>>()
                .As<IRepository<AffiliateCustomerMapping>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<AffiliateCommissionRate>>()
                .As<IRepository<AffiliateCommissionRate>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get
            {
                return 1;
            }
        }
    }
}

using Autofac;
using Autofac.Core;
using Autofac.Integration.WebApi;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.EkShopA2I.Domain;
using Nop.Plugin.Widgets.EkShopA2I.Services;
using Nop.Web.Framework.Mvc;
using System.Reflection;

namespace Nop.Plugin.Widgets.EkShopA2I.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_ekshop";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder,NopConfig config)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            builder.RegisterType<ConfigureService>().As<IConfigureService>().InstancePerLifetimeScope();
            builder.RegisterType<EkshopOrderService>().As<IEkshopOrderService>().InstancePerLifetimeScope();
            builder.RegisterType<CommissionRateService>().As<ICommissionRateService>().InstancePerLifetimeScope();
            builder.RegisterType<EkshopEpService>().As<IEkshopEpService>().InstancePerLifetimeScope();

            this.RegisterPluginDataContext<EkshopObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<EfRepository<EsOrder>>()
                .As<IRepository<EsOrder>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<EsUdcCommissionRate>>()
                .As<IRepository<EsUdcCommissionRate>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 4; }
        }
    }
}

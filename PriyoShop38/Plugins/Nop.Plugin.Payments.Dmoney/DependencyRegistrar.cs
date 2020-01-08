using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.Dmoney.Data;
using Nop.Plugin.Payments.Dmoney.Domains;
using Nop.Plugin.Payments.Dmoney.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Payments.Dmoney
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_dmoney";

        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder,NopConfig config)
        {
            builder.RegisterType<DmoneyPaymentService>().As<IDmoneyPaymentService>().InstancePerLifetimeScope();
            builder.RegisterType<DmoneyTransactionService>().As<IDmoneyTransactionService>().InstancePerLifetimeScope();

            this.RegisterPluginDataContext<DmoneyObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<EfRepository<DmoneyTransaction>>()
                .As<IRepository<DmoneyTransaction>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }

        public int Order
        {
            get { return 4; }
        }
    }
}

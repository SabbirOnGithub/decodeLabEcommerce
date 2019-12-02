using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Purchase.Offer.Data;
using Nop.Plugin.Purchase.Offer.Domain;
using Nop.Plugin.Widgets.CustomFooter.Data;
using Nop.Web.Framework.Mvc;
using System.Reflection;
using Autofac.Integration.WebApi;

namespace Nop.Plugin.Purchase.Offer
{
    public class PurchaseOfferDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_purchase_offer";
        public int Order => 1;

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());

            this.RegisterPluginDataContext<PurchaseOfferObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<PurchaseOfferService>().As<IPurchaseOfferService>().InstancePerLifetimeScope();

            builder.RegisterType<EfRepository<PurchaseOffer>>()
                .As<IRepository<PurchaseOffer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<PurchaseOfferOption>>()
                .As<IRepository<PurchaseOfferOption>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
        }
    }
}

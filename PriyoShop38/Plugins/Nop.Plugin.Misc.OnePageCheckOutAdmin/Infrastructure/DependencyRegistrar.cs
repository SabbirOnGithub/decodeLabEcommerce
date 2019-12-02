using Autofac;
using Autofac.Core;
using Nop.Core.Caching;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Controllers;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Orders;
using Nop.Plugin.Misc.OnePageCheckOutAdmin.Services;

namespace Nop.Plugin.Misc.OnePageCheckOutAdmin.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BSCustomerService>().As<IBSCustomerService>().InstancePerLifetimeScope();
            builder.RegisterType<OrderProcessingServiceForOnePageCheckout>().As<IOrderProcessingServiceOnePageCheckOut>().InstancePerLifetimeScope();
            //we cache presentation models between requests
            builder.RegisterType<MiscOnePageCheckOutAdminController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));

            builder.RegisterType<DelivaryDateTimeController>()
                .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }

       
    }
}

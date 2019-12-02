using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Purchase.UserAgent.Services;

namespace Nop.Plugin.Widgets.CustomFooter
{
    public class PurchaseUserAgentDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_purchase_user_agent";
        public int Order
        {
            get
            {
                return 1;
            }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<PurchaseUserAgentService>().As<IPurchaseUserAgentService>().InstancePerLifetimeScope();
        }
    }
}

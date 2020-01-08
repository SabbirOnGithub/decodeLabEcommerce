using Autofac;
using Autofac.Core;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.BsMegaMenu.Controllers;
using Nop.Core.Configuration;
namespace Nop.Plugin.Widgets.BsMegaMenu.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, Core.Configuration.NopConfig config)
        {
            builder.RegisterType<BsMegaMenuController>()
               .WithParameter(ResolvedParameter.ForNamed<ICacheManager>("nop_cache_static"));
        }

        public int Order
        {
            get { return 2; }
        }




       
    }
}

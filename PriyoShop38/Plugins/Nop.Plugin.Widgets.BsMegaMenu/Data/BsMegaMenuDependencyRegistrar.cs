using Nop.Core.Infrastructure.DependencyManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Web.Framework.Mvc;
using Nop.Plugin.Widgets.BsMegaMenu.Domain;
using Nop.Data;
using Nop.Core.Data;
using Nop.Plugin.Widgets.BsMegaMenu.Services;

namespace Nop.Plugin.Widgets.BsMegaMenu.Data
{
    public class BsMegaMenuDependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_Bs_Mega_Menu";
        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            this.RegisterPluginDataContext<BsMegaMenuObjectContext>(builder, CONTEXT_NAME);

            builder.RegisterType<EfRepository<BsMegaMenuDomain>>()
                .As<IRepository<BsMegaMenuDomain>>()
                .WithParameter(Autofac.Core.ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();

            builder.RegisterType<BsMegaMenuService>()
                .As<IBsMegaMenuService>().
                InstancePerLifetimeScope();
        }
    }
}

using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Widgets.MobileLogin.Data;
using Nop.Plugin.Widgets.MobileLogin.Domain;
using Nop.Plugin.Widgets.MobileLogin.Services;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.MobileLogin
{
    /// <summary>
    /// Dependency registrar
    /// </summary>
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region Field
        private const string ContextName = "nop_object_context_mobile_login";
        #endregion
        /// <summary>
        /// Register services and interfaces
        /// </summary>
        /// <param name="builder">Container builder</param>
        /// <param name="typeFinder">Type finder</param>
        /// <param name="config">Config</param>
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {           
            //Register services
            builder.RegisterType<MobileLoginService>().As<IMobileLoginService>();

            //data context
            this.RegisterPluginDataContext<MobileLoginObjectContext>(builder, ContextName);

            //Override the repository injection
            builder.RegisterType<EfRepository<MobileLoginCustomer>>()
                .As<IRepository<MobileLoginCustomer>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(ContextName))
                .InstancePerLifetimeScope();
        }

        /// <summary>
        /// Order of this dependency registrar implementation
        /// </summary>
        public int Order
        {
            get { return 1; }
        }
    }
}

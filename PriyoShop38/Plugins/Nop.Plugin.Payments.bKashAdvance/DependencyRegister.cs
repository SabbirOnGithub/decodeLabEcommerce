using Autofac;
using Nop.Core.Configuration;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Payments.bKashAdvance.Services;

namespace Nop.Plugin.Payments.bKashAdvance
{
    public partial class DependencyRegister : IDependencyRegistrar
    {
        #region Register

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            builder.RegisterType<BkashAdvanceService>().As<IBkashAdvanceService>();
        }

        #endregion

        #region DB

        public int Order
        {
            get { return 0; }
        }
        #endregion

       
    }
}
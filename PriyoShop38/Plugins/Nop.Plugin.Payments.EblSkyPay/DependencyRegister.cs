using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Payments.EblSkyPay.Data;
using Nop.Plugin.Payments.EblSkyPay.Services;

namespace Nop.Plugin.Payments.EblSkyPay
{
    public partial class DependencyRegister : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "nop_object_context_eblskypay";

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //Load custom data settings
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();
            
            //Register custom object context
            builder.Register<IDbContext>(c => RegisterIDbContext(c, dataSettings)).Named<IDbContext>(CONTEXT_NAME).InstancePerHttpRequest();
            builder.Register(c => RegisterIDbContext(c, dataSettings)).InstancePerHttpRequest();

            //Register services
            builder.RegisterType<SkyPayService>().As<ISkyPayService>().InstancePerHttpRequest();
            builder.RegisterType<EblSkyPayPaymentProcessor>().As<EblSkyPayPaymentProcessor>().InstancePerHttpRequest();

            //Override the repository injection
            builder.RegisterType<EfRepository<Domain.EblSkyPay>>().As<IRepository<Domain.EblSkyPay>>().WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME)).InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 0; }
        }

        private EblSkyPayObjectContext RegisterIDbContext(IComponentContext componentContext, DataSettings dataSettings)
        {
            string dataConnectionStrings;

            if (dataSettings != null && dataSettings.IsValid())
            {
                dataConnectionStrings = dataSettings.DataConnectionString;
            }
            else
            {
                dataConnectionStrings = componentContext.Resolve<DataSettings>().DataConnectionString;
            }

            return new EblSkyPayObjectContext(dataConnectionStrings);
        }
    }
}
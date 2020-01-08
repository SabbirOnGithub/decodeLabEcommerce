using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using Nop.Plugin.Misc.ProductDetailsById.Data;

namespace Nop.Plugin.Misc.ProductDetailsById
{
    public partial class DependencyRegister : IDependencyRegistrar
    {
        #region Field

        private const string ContextName = "nop_object_product_details_byid";

        #endregion

        #region Register

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //Load custom data settings
            var dataSettingsManager = new DataSettingsManager();
            var dataSettings = dataSettingsManager.LoadSettings();

            //Register custom object context
            builder.Register<IDbContext>(c => RegisterIDbContext(c, dataSettings)).Named<IDbContext>(ContextName).InstancePerHttpRequest();
            builder.Register(c => RegisterIDbContext(c, dataSettings)).InstancePerHttpRequest();

            builder.RegisterType<Nop.Plugin.Misc.ProductDetailsById.Controllers.ProductDetailsByIdController>().As<Nop.Web.Controllers.ProductController>();
            builder.RegisterType<Nop.Plugin.Misc.ProductDetailsById.Controllers.VendorCustomController>().As<Nop.Web.Controllers.CatalogController>();
            //Register services

            //builder.RegisterType<LiveAnnouncementService>().As<ILiveAnnouncementService>();


            //builder.RegisterType<AnnouncementService>().As<IAnnouncementService>();
            //builder.RegisterType<AppBuilder>().As<IAppBuilder>();

        }

        #endregion

        #region DB

        public int Order
        {
            get { return 0; }
        }

        private ProductDetailsByIdObjectContext RegisterIDbContext(IComponentContext componentContext,
                                                       DataSettings dataSettings)
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

            return new ProductDetailsByIdObjectContext(dataConnectionStrings);
        }

        #endregion

       
    }
}
using Autofac;
using Autofac.Core;
using Nop.Core.Configuration;
using Nop.Core.Data;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Data;
using BS.Plugin.NopStation.MobileApp.Data;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Services;
using Nop.Web.Framework.Mvc;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        private const string CONTEXT_NAME = "bs_object_context_nopstation_mobileapp";
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder, NopConfig config)
        {
            //Service
            builder.RegisterType<SmartGroupService>().As<ISmartGroupService>().InstancePerLifetimeScope();
            //Service
            builder.RegisterType<NotificationMessageTemplateService>().As<INotificationMessageTemplateService>().InstancePerLifetimeScope();
            builder.RegisterType<ScheduledNotificationService>().As<IScheduledNotificationService>().InstancePerLifetimeScope();
           // builder.RegisterType<DeviceService>().As<IDeviceService>().InstancePerLifetimeScope();
            builder.RegisterType<QueuedNotificationService>().As<IQueuedNotificationService>().InstancePerLifetimeScope();

            builder.RegisterType<LoggerNotification>().As<ILoggerNotification>().InstancePerLifetimeScope();
            
            //data context
            this.RegisterPluginDataContext<MobileAppObjectContext>(builder, CONTEXT_NAME);
           
            //override required repository with our custom context

            //builder.RegisterGeneric(typeof(EfRepository<>)).As(typeof(IRepository<>)).WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME)).InstancePerLifetimeScope();

            //repository
            //builder.RegisterType<EfRepository<Device>>()
            //    .As<IRepository<Device>>()
            //    .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
            //    .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<NotificationMessageTemplate>>()
             .As<IRepository<NotificationMessageTemplate>>()
             .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
             .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<SmartGroup>>()
                .As<IRepository<SmartGroup>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
                .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<ScheduledNotification>>()
               .As<IRepository<ScheduledNotification>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();
            builder.RegisterType<EfRepository<QueuedNotification>>()
               .As<IRepository<QueuedNotification>>()
               .WithParameter(ResolvedParameter.ForNamed<IDbContext>(CONTEXT_NAME))
               .InstancePerLifetimeScope();

            //data context
            this.RegisterPluginDataContext<MobileAppObjectContext>(builder, CONTEXT_NAME);
            
        }

        public int Order
        {
            get { return 1; }
        }
    }
}

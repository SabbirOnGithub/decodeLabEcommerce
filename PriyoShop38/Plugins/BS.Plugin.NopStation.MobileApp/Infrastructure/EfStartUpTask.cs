using System.Data.Entity;
using Nop.Core.Infrastructure;
using BS.Plugin.NopStation.MobileApp.Domain;
using System.Web.Mvc;
using AutoMapper;
using BS.Plugin.NopStation.MobileApp.Data;
using BS.Plugin.NopStation.MobileApp.Models;
//using Nop.Plugin.NopStation.MobileWebApi.Domain;
//using Nop.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure
{
    public class EfStartUpTask : IStartupTask
    {
        public void Execute()
        {

            //It's required to set initializer to null (for SQL Server Compact).
            //otherwise, you'll get something like "The model backing the 'your context name' context has changed since the database was created. Consider using Code First Migrations to update the database"
            Database.SetInitializer<MobileAppObjectContext>(null);
            ModelBinderProviders.BinderProviders.Add(new EFModelBinderProvider());

            #region AutoMapperStartupTask
            //message template
            //Mapper.CreateMap<NotificationMessageTemplate, NotificationMessageTemplateModel>()
            //    .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
            //    .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
            //    .ForMember(dest => dest.Locales, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
            //    .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());
            //Mapper.CreateMap<NotificationMessageTemplateModel, NotificationMessageTemplate>();
            //schedule
            //Mapper.CreateMap<ScheduledNotification, ScheduledNotificationModel>()
            //    .ForMember(dest => dest.GroupName, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableGroups, mo => mo.Ignore())
            //    .ForMember(dest => dest.AvailableMessageTemplates, mo => mo.Ignore())
            //    .ForMember(dest => dest.NotificationType, mo => mo.Ignore());
                
            //Mapper.CreateMap<ScheduledNotificationModel, ScheduledNotification>()
            //    .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

            //queued notification
            //Mapper.CreateMap<QueuedNotification, QueuedNotificationModel>()
            //    .ForMember(dest => dest.GroupName, mo => mo.Ignore())
            //    .ForMember(dest => dest.ToCustomerName, mo => mo.Ignore())
            //    .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
            //    .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

            //Mapper.CreateMap<QueuedNotificationModel, QueuedNotification>()
            //    .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
            //    .ForMember(dest => dest.NotificationType, mo => mo.Ignore());
            //device
            //Mapper.CreateMap<Device, DeviceModel>()
            //    .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
            //    .ForMember(dest => dest.CustomerName, mo => mo.Ignore());

            //Mapper.CreateMap<DeviceModel, Device>();
            #endregion
        }

        public int Order
        {
            //ensure that this task is run first 
            get { return 0; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.Domain;
//using Nop.Plugin.NopStation.MobileWebApi.Domain;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileApp
{
    public static class AutoMapperConfiguration
    {
        private static MapperConfiguration _mapperConfiguration;
        private static IMapper _mapper;

        public static void Init()
        {
            _mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<NotificationMessageTemplate, NotificationMessageTemplateModel>()
                .ForMember(dest => dest.AllowedTokens, mo => mo.Ignore())
                .ForMember(dest => dest.HasAttachedDownload, mo => mo.Ignore())
                .ForMember(dest => dest.Locales, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableStores, mo => mo.Ignore())
                .ForMember(dest => dest.ListOfStores, mo => mo.Ignore())
                .ForMember(dest => dest.SelectedStoreIds, mo => mo.Ignore())
                .ForMember(dest => dest.CustomProperties, mo => mo.Ignore());

                cfg.CreateMap<NotificationMessageTemplateModel, NotificationMessageTemplate>();

                cfg.CreateMap<ScheduledNotification, ScheduledNotificationModel>()
                .ForMember(dest => dest.GroupName, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableGroups, mo => mo.Ignore())
                .ForMember(dest => dest.AvailableMessageTemplates, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

                cfg.CreateMap<ScheduledNotificationModel, ScheduledNotification>()
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

                cfg.CreateMap<QueuedNotification, QueuedNotificationModel>()
                .ForMember(dest => dest.GroupName, mo => mo.Ignore())
                .ForMember(dest => dest.ToCustomerName, mo => mo.Ignore())
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

                cfg.CreateMap<QueuedNotificationModel, QueuedNotification>()
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.NotificationType, mo => mo.Ignore());

                cfg.CreateMap<Device, DeviceModel>()
                .ForMember(dest => dest.DeviceType, mo => mo.Ignore())
                .ForMember(dest => dest.CustomerName, mo => mo.Ignore());

                 cfg.CreateMap<DeviceModel, Device>();
            });
            _mapper = _mapperConfiguration.CreateMapper();
        }

        public static IMapper Mapper
        {
            get
            {
                return _mapper;
            }
        }
        /// <summary>
        /// Mapper configuration
        /// </summary>
        public static MapperConfiguration MapperConfiguration
        {
            get
            {
                return _mapperConfiguration;
            }
        }
    }
}

using System;
using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.ModelsApi.ResponseModel;
using Nop.Services.Media;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileWebApi.Domain;

namespace BS.Plugin.NopStation.MobileApp.Extensions
{
    public static class MappingExtension
    {
        public static TDestination MapTo<TSource, TDestination>(this TSource source)
        {
            return AutoMapperConfiguration.Mapper.Map<TSource, TDestination>(source);
        }

        public static TDestination MapTo<TSource, TDestination>(this TSource source, TDestination destination)
        {
            return AutoMapperConfiguration.Mapper.Map(source, destination);
        }

        #region Message templates

        public static NotificationMessageTemplateModel ToModel(this NotificationMessageTemplate entity)
        {
            return entity.MapTo<NotificationMessageTemplate, NotificationMessageTemplateModel>();
        }

        public static NotificationMessageTemplate ToEntity(this NotificationMessageTemplateModel model)
        {
            return model.MapTo<NotificationMessageTemplateModel, NotificationMessageTemplate>();
        }

        public static NotificationMessageTemplate ToEntity(this NotificationMessageTemplateModel model, NotificationMessageTemplate destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Smart Group Criteria
        public static CriteriaModel ToCriteriaModel(this SmartGroup entity)
        {
            CriteriaModel criteriaModel = new CriteriaModel();
            criteriaModel.Id = entity.Id;
            criteriaModel.Name = entity.Name;
            criteriaModel.Columns = entity.Columns;
            criteriaModel.Conditions = entity.Conditions;
            criteriaModel.KeyWord = entity.KeyWord;
            criteriaModel.AndOr = entity.AndOr;
            criteriaModel.Query = entity.Query;
            return criteriaModel;
        }

        public static SmartGroup ToSmartGroupEntity(this CriteriaModel model)
        {
            SmartGroup entity = new SmartGroup();

            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Columns = model.Columns;
            entity.Conditions = model.Conditions;
            entity.KeyWord = model.KeyWord;
            entity.AndOr = model.AndOr;
            entity.Query = model.Query;

            return entity;
        }

        public static SmartGroup ToSmartGroupEntity(this CriteriaModel model, SmartGroup destination)
        {
            SmartGroup entity = destination;

            entity.Id = model.Id;
            entity.Name = model.Name;
            entity.Columns = model.Columns;
            entity.Conditions = model.Conditions;
            entity.KeyWord = model.KeyWord;
            entity.AndOr = model.AndOr;
            entity.Query = model.Query;

            return entity;
        }

        #endregion

        #region Schedule

        public static ScheduledNotificationModel ToModel(this ScheduledNotification entity)
        {
            return entity.MapTo<ScheduledNotification, ScheduledNotificationModel>();
        }

        public static ScheduledNotification ToEntity(this ScheduledNotificationModel model)
        {
            return model.MapTo<ScheduledNotificationModel, ScheduledNotification>();
        }

        public static ScheduledNotification ToEntity(this ScheduledNotificationModel model, ScheduledNotification destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Queued notification

        public static QueuedNotificationModel ToModel(this QueuedNotification entity)
        {
            return entity.MapTo<QueuedNotification, QueuedNotificationModel>();
        }

        public static QueuedNotification ToEntity(this QueuedNotificationModel model)
        {
            return model.MapTo<QueuedNotificationModel, QueuedNotification>();
        }

        public static QueuedNotification ToEntity(this QueuedNotificationModel model, QueuedNotification destination)
        {
            return model.MapTo(destination);
        }

        #endregion

        #region Device

        public static DeviceModel ToModel(this Device entity)
        {
            return entity.MapTo<Device, DeviceModel>();
        }

        public static Device ToEntity(this DeviceModel model)
        {
            return model.MapTo<DeviceModel, Device>();
        }

        public static Device ToEntity(this DeviceModel model, Device destination)
        {
            return model.MapTo(destination);
        }

        #endregion
    }
}

using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using BS.Plugin.NopStation.MobileApp.Infrastructure.WebApi;
using Nop.Web.Framework.Localization;
using Nop.Web.Framework.Mvc.Routes;
using BS.Plugin.NopStation.MobileApp.ViewEngines;

namespace BS.Plugin.NopStation.MobileApp.Infrastructure
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {

           
            System.Web.Mvc.ViewEngines.Engines.Insert(0, new CustomViewEngine());
            
            #region Admin

            #region notification message template

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.NotificationMessageTemplateList", "Admin/Plugin/NopStation/MobileApp/MessageTemplateList",
                   new { controller = "BsNotificationMessageTemplate", action = "NotificationMessageTemplateList" },
                   new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.NotificationMessageTemplateCreate", "Admin/Plugin/NopStation/MobileApp/NotificationMessageTemplateCreate",
                  new { controller = "BsNotificationMessageTemplate", action = "NotificationMessageTemplateCreate" },
                  new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.NotificationMessageTemplateEdit", "Admin/Plugin/NopStation/MobileApp/NotificationMessageTemplateEdit/{id}",
                 new { controller = "BsNotificationMessageTemplate", action = "NotificationMessageTemplateEdit" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.DeleteNotificationMessageTemplate", "Admin/Plugin/NopStation/MobileApp/NotificationMessageTemplateListDelete/{id}",
                 new { controller = "BsNotificationMessageTemplate", action = "Delete" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            #endregion

            #region test push route
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.TestPushNotification", "Admin/Plugin/NopStation/MobileApp/Test",
                 new { controller = "BsNotificationAdmin", action = "TestPushNotification" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            #endregion

            #region FCM pushNotification route
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.FcmPushNotification", "Admin/Plugin/NopStation/MobileApp/FcmPush",
                new { controller = "BsNotificationAdmin", action = "FcmPushNotification" },
                new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            #endregion

            #region device
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.DeviceList", "Admin/Plugin/NopStation/MobileApp/DeviceList",
                   new { controller = "BsNotificationAdmin", action = "Device" },
                   new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.CreateDevice", "Admin/Plugin/NopStation/MobileApp/CreateDevice",
                  new { controller = "BsNotificationAdmin", action = "CreateDevice" },
                  new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.EditDevice", "Admin/Plugin/NopStation/MobileApp/EditDevice/{id}",
                 new { controller = "BsNotificationAdmin", action = "EditDevice" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            #endregion

            #region queued notification
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.QueuedList", "Admin/Plugin/NopStation/MobileApp/QueuedList",
                   new { controller = "BsNotificationAdmin", action = "Queued" },
                   new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.CreateQueued", "Admin/Plugin/NopStation/MobileApp/CreateQueued",
                  new { controller = "BsNotificationAdmin", action = "CreateQueued" },
                  new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.EditQueued", "Admin/Plugin/NopStation/MobileApp/EditQueued/{id}",
                 new { controller = "BsNotificationAdmin", action = "EditQueued" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            #endregion

            #region schedule
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.ScheduleList", "Admin/Plugin/NopStation/MobileApp/ScheduleList",
                   new { controller = "BsNotificationAdmin", action = "Schedule" },
                   new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.CreateSchedule", "Admin/Plugin/NopStation/MobileApp/CreateSchedule",
                  new { controller = "BsNotificationAdmin", action = "CreateSchedule" },
                  new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.EditSchedule", "Admin/Plugin/NopStation/MobileApp/EditSchedule/{id}",
                 new { controller = "BsNotificationAdmin", action = "EditSchedule" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.DeleteSchedule", "Admin/Plugin/NopStation/MobileApp/DeleteSchedule/{id}",
                 new { controller = "BsNotificationAdmin", action = "DeleteSchedule" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            #endregion

            #region group
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.SmartGroups", "Admin/Plugin/NopStation/MobileApp/SmartGroups",
                   new { controller = "BsNotificationAdmin", action = "Group" },
                   new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.CreateGroup", "Admin/Plugin/NopStation/MobileApp/CreateGroup",
                  new { controller = "BsNotificationAdmin", action = "CreateGroup" },
                  new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");
            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.EditGroup", "Admin/Plugin/NopStation/MobileApp/EditGroup/{id}",
                 new { controller = "BsNotificationAdmin", action = "EditGroup" },
                 new { id = @"\d+" },
                 new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            routes.MapRoute("Admin.Plugin.NopStation.MobileApp.SmartGroup", "Admin/Plugin/NopStation/MobileApp/SmartGroup/{id}",
                new { controller = "BsNotificationAdmin", action = "SmartGroup" },
                new { id = @"\d+" },
                new[] { "BS.Plugin.NopStation.MobileApp.Controllers" }).DataTokens.Add("area", "admin");

            #endregion
           

            #endregion

            //#region web api 2

            // var config = GlobalConfiguration.Configuration;
            //WebApiConfig.Register(config);
            //#endregion

        }
        public int Priority
        {
            get
            {
                return 2;
            }
        }
    }
}

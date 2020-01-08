using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web.Http;
using Newtonsoft.Json;
using BS.Plugin.NopStation.MobileApp.Domain;
using BS.Plugin.NopStation.MobileApp.Models;
using BS.Plugin.NopStation.MobileApp.ModelsApi.QueryModel;
using BS.Plugin.NopStation.MobileApp.ModelsApi.ResponseModel;
using BS.Plugin.NopStation.MobileApp.Services;
using BS.Plugin.NopStation.MobileApp.Extensions;
using Nop.Services.Media;

namespace BS.Plugin.NopStation.MobileApp.Controllers
{
    public class BsNotificationApiController: BaseApiController
    {

       
        private readonly IPictureService _pictureService;


        #region ctor
        public BsNotificationApiController(
            IPictureService pictureService
            )
        {
            this._pictureService = pictureService;
        }
        #endregion
        
       
       
        // PrePareModelForOfferList(user.TotalFollowers,user.TotalFollowers,null,user.BsInstagramUserId,_instagramOfferService,_pictureService);
    }
}

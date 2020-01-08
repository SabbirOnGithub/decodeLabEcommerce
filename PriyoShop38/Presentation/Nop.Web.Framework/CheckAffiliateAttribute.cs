using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Affiliates;
using Nop.Core.Domain.Logging;
using Nop.Core.Infrastructure;
using Nop.Services.Affiliates;
using Nop.Services.Customers;
using Nop.Services.Logging;

namespace Nop.Web.Framework
{
    public class CheckAffiliateAttribute : ActionFilterAttribute
    {
        private const string AFFILIATE_ID_QUERY_PARAMETER_NAME = "affiliateid";
        private const string AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME = "affiliate";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            Affiliate affiliate = null;

            if (request.QueryString != null)
            {
                //try to find by ID ("affiliateId" parameter)
                if (request.QueryString[AFFILIATE_ID_QUERY_PARAMETER_NAME] != null)
                {
                    var affiliateId = Convert.ToInt32(request.QueryString[AFFILIATE_ID_QUERY_PARAMETER_NAME]);
                    if (affiliateId > 0)
                    {
                        var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                        affiliate = affiliateService.GetAffiliateById(affiliateId);
                    }
                }
                //try to find by friendly name ("affiliate" parameter)
                else if (request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME] != null)
                {
                    var friendlyUrlName = request.QueryString[AFFILIATE_FRIENDLYURLNAME_QUERY_PARAMETER_NAME];
                    if (!String.IsNullOrEmpty(friendlyUrlName))
                    {
                        var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                        affiliate = affiliateService.GetAffiliateByFriendlyUrlName(friendlyUrlName);
                    }
                }

                #region BS-23

                if (affiliate == null || affiliate.Deleted || !affiliate.Active)
                {
                    var affiliateTypes = GetAffiliateTypes();
                    var logger = EngineContext.Current.Resolve<ILogger>();

                    foreach (var item in affiliateTypes)
                    {
                        //try to find by ID ("affiliateId" parameter)
                        if (request.QueryString[item.IdUrlParameter] != null)
                        {
                            try
                            {
                                var affiliateId = Convert.ToInt32(request.QueryString[item.IdUrlParameter]);
                                if (affiliateId > 0)
                                {
                                    var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                                    affiliate = affiliateService.GetAffiliateById(affiliateId);
                                }
                                break;
                            }
                            catch (FormatException formatException)
                            {
                                logger.InsertLog(LogLevel.Error,
                                    "PriyoAffiliate | IdUrlParameter could not be converted to Int32",
                                    formatException.Message);
                            }
                        }
                        else if (request.QueryString[item.NameUrlParameter] != null)
                        {
                            var friendlyUrlName = request.QueryString[item.NameUrlParameter];
                            if (!String.IsNullOrEmpty(friendlyUrlName))
                            {
                                var affiliateService = EngineContext.Current.Resolve<IAffiliateService>();
                                affiliate = affiliateService.GetAffiliateByFriendlyUrlName(friendlyUrlName);
                            }
                            break;
                        }
                    }
                }

                #endregion
            }


            if (affiliate != null && !affiliate.Deleted && affiliate.Active)
            {
                var workContext = EngineContext.Current.Resolve<IWorkContext>();
                if (workContext.CurrentCustomer.AffiliateId != affiliate.Id)
                {
                    workContext.CurrentCustomer.AffiliateId = affiliate.Id;
                    var customerService = EngineContext.Current.Resolve<ICustomerService>();
                    customerService.UpdateCustomer(workContext.CurrentCustomer);
                }
            }
        }

        #region BS-23

        private List<AffiliateTypeAttributeModel> GetAffiliateTypes()
        {
            var list = new List<AffiliateTypeAttributeModel>();
            try
            {
                string affiliateTypeStr = null;
                var filePath = CommonHelper.MapPath("~/ApiJson/affiliate-type-json.json");

                try
                {
                    affiliateTypeStr = System.IO.File.ReadAllText(filePath);
                }
                catch { }

                if (string.IsNullOrWhiteSpace(affiliateTypeStr))
                {
                    try
                    {
                        filePath = CommonHelper.MapPath("~/ApiJson/affiliate-type-backup-json.json");
                        affiliateTypeStr = System.IO.File.ReadAllText(filePath);
                    }
                    catch { }
                }

                if (!string.IsNullOrWhiteSpace(affiliateTypeStr))
                {
                    list = JsonConvert.DeserializeObject<List<AffiliateTypeAttributeModel>>(affiliateTypeStr);
                }
            }
            catch
            {

            }
            return list;
        }

        #endregion
    }
}

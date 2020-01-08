using BS.Plugin.NopStation.MobileWebApi.Extensions;
using BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog;
using Nop.Core;
using Nop.Services.Topics;
using System.Web.Http;

namespace BS.Plugin.NopStation.MobileWebApi.Controllers
{
    public class TopicController : WebApiController
    {
        private readonly ITopicService _topicService;
        private readonly IWorkContext _workContext;

        public TopicController(ITopicService topicService,
            IWorkContext workContext)
        {
            this._topicService = topicService;
            this._workContext = workContext;
        }

        #region Topics 
        [Route("api/topic/{SystemName}")]
        public IHttpActionResult GetTopicMessage(string SystemName, string password = "")
        {
            var response = new TopicResponseModel();
            if (!string.IsNullOrWhiteSpace(SystemName))
            {
                var topic = _topicService.GetTopicBySystemName(SystemName);
                if (topic != null && (!topic.IsPasswordProtected || topic.Password.Equals(password)))
                {
                    response.TopicModel.Body = topic.Body;
                    response.TopicModel.IsPasswordProtected = topic.IsPasswordProtected;
                    response.TopicModel.MetaDescription = topic.MetaDescription;
                    response.TopicModel.MetaKeywords = topic.MetaKeywords;
                    response.TopicModel.MetaTitle = topic.MetaTitle;
                    response.TopicModel.SystemName = topic.SystemName;
                    response.TopicModel.Title = topic.Title;
                    response.TopicModel.TopicTemplateId = topic.TopicTemplateId;

                    response.StatusCode = (int)ErrorType.Ok;
                    response.SuccessMessage = "Success";

                    return Ok(response);
                }
            }
            response.StatusCode = (int)ErrorType.NotOk;
            response.SuccessMessage = "Topic not found or protected";
            return Ok(response);
        }
        #endregion
    }
}

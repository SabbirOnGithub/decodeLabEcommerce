using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Models._ResponseModel.Catalog
{
    public class TopicResponseModel : BaseResponse
    {
        public TopicResponseModel()
        {
            TopicModel = new TopicApiModel();
        }

        public TopicApiModel TopicModel { get; set; }
    }

    public class TopicApiModel
    {
        public int Id { get; set; }
        public string SystemName { get; set; }
        public bool IsPasswordProtected { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public int TopicTemplateId { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaTitle { get; set; }
    }
}

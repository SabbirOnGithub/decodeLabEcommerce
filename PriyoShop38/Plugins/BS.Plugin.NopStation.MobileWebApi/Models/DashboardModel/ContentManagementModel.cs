using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.Collections.Generic;

namespace BS.Plugin.NopStation.MobileWebApi.Models.DashboardModel
{
    public class ContentManagementModel : BaseNopModel
    {
        public ContentManagementModel()
        {
            AvailableStores = new List<SelectListItem>();
        }
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.NopStation.MobileWebApi.DefaultNopFlowSameAs")]
        public bool DefaultNopFlowSameAs { get; set; }
        public bool DefaultNopFlowSameAs_OverrideForStore { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Topics.List.SearchStore")]
        public int SearchStoreId { get; set; }
        public IList<SelectListItem> AvailableStores { get; set; }
    }
}
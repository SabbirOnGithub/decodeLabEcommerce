
using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Web.Framework.Menu;
using System;
using System.Collections.Generic;
using System.Web.Routing;
using System.Linq;
using Nop.Plugin.Misc.ProductDetailsById.Data;

namespace Nop.Plugin.Misc.ProductDetailsById
{
    public class ProductDetailsByIdPlugin : BasePlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ProductDetailsByIdObjectContext _context;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingContext;


        private const string HeaderWidget = "header";

        #endregion


        #region Ctr

        public ProductDetailsByIdPlugin(ProductDetailsByIdObjectContext context, ILocalizationService localizationService, ISettingService settingContext)
        {
            _context = context;
            _localizationService = localizationService;
            _settingContext = settingContext;
        }

        #endregion

        #region Install / Uninstall


        public override void Install()
        {

            //base install
            base.Install();
        }
        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }

        #endregion

        #region Menu Builder

        public bool Authenticate()
        {
            return true;
        }

        #endregion


        //public void ManageSiteMap(SiteMapNode rootNode)
        //{
        //    var mainnode = new SiteMapNode()
        //    {
        //        Title = _localizationService.GetResource("Plugin"),
        //        Visible = true,
        //        RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
        //    };
        //    rootNode.ChildNodes.Add(mainnode);
        //    var subnode = new SiteMapNode()
        //    {
        //        Title = _localizationService.GetResource("Misc.ToastDesign"),
        //        Visible = true,
        //        Url = "~/LiveAnnouncement/ToastDesign",
        //        RouteValues = new RouteValueDictionary() { { "area", "Admin" } }
        //    };
        //    mainnode.ChildNodes.Add(subnode);
        //}


        public void ManageSiteMap(SiteMapNode rootNode)
        {
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            { 
               // "body_end_html_tag_before"
               HeaderWidget
            };
        }


    }
}
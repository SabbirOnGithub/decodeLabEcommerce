using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Plugin.Widgets.BsMegaMenu.Models;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.BsMegaMenu.Models
{
    public class BsMegaMenuModel : BaseNopEntityModel
    {
        public BsMegaMenuModel()
        {
            CategoryList = new List<CategoryMenuModel>();
            Manufactures = new List<ManufacturerModel>();
            BsMegaMenuSettingsModel = new BsMegaMenuSettingsModel();
        }

        public List<CategoryMenuModel> CategoryList{ get; set; }
        public IList<ManufacturerModel> Manufactures { get; set; }
        public BsMegaMenuSettingsModel BsMegaMenuSettingsModel { get; set; }        
    }
}
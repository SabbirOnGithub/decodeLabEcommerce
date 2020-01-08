using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{
    public partial class CustomerListModel : BaseNopModel
    {                
        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchEmail")]
        [AllowHtml]
        public string SearchEmail { get; set; }        

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchName")]
        [AllowHtml]
        public string SearchName { get; set; }       

        [NopResourceDisplayName("Admin.Customers.Customers.List.SearchPhone")]
        [AllowHtml]
        public string SearchMobileNumber { get; set; }        
    }
}
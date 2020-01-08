using System.Web.Mvc;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;

namespace Nop.Plugin.Widgets.MobileLogin.Models
{    
    public partial class MobileLoginRecordModel : BaseNopModel
    {
        public int CustomerId { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.MobileNumber")]
        public string MobileNumber { get; set; }

        [NopResourceDisplayName("Account.Login.Fields.Email")]
        [AllowHtml]
        public string Email { get; set; }

        [NopResourceDisplayName("Account.Fields.Name")]
        [AllowHtml]
        public string Name { get; set; }        

        public int Used { get; set; }

        [NopResourceDisplayName("Admin.Customers.Customers.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }
    }
}
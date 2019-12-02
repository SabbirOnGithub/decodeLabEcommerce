using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Common;

namespace Nop.Web.Models.Common
{
    [Validator(typeof(SaleWithUsValidator))]
    public partial class SaleWithUsModel : BaseNopModel
    {
        public string FullName { get; set; }
        public string CompanyName { get; set; }
        [AllowHtml]        
        public string Email { get; set; }
        public string Phone { get; set; }
        [AllowHtml]        
        public string Address { get; set; }
        public string Website { get; set; }        
        
        public bool SuccessfullySent { get; set; }
        public string Result { get; set; }
    }
}
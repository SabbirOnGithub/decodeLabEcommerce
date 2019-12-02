using FluentValidation.Attributes;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Nop.Plugin.Widgets.BsMegaMenu.Domain
{
    [Validator(typeof(BsMegaMenuValidator))]
    public class BsMegaMenuDomain : BaseEntity
    {
        public BsMegaMenuDomain()
        {
            CategoryList = new List<SelectListItem>();
        }
        public int CategoryId { get; set; }
        public IList<SelectListItem> CategoryList { get; set; }
        public string SelectedCategory { get; set; }
        public int ParentCategoryId { get; set; }
        public int NumberOfColums { get; set; }
        public int ColumnPosition { get; set; }
    }
}

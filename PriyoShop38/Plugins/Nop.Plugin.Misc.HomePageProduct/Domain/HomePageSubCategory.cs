using System;
using Nop.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nop.Plugin.Misc.HomePageProduct.Domain
{
    public class HomePageSubCategory : BaseEntity
    {
        public int CategoryId { get; set; }
        public string TabName { get; set; }
        public int SubCategoryPriority { get; set; }
        public int SubCategoryId { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdateOnUtc { get; set; }


        //public virtual HomePageCategory HomePageCategory { get; set; }

    }
}
using System;
using Nop.Core;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Domain
{
    public class HomePageProductCategory : BaseEntity
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public int Priority { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdateOnUtc { get; set; }



        //public virtual HomePageCategory HomePageCategory { get; set; }

    }
}
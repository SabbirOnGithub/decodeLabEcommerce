using System;
using Nop.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Nop.Plugin.Misc.HomePageProduct.Domain
{
    public class HomePageCategory : BaseEntity
    {
        public int CategoryId { get; set; }
        public int CategoryPriority { get; set; }
        public bool Publish { get; set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime UpdateOnUtc { get; set; }
        public string CategoryDisplayName { get; set; }
    }
}
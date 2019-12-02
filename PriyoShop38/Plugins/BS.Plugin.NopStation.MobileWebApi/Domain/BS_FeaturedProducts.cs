using Nop.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Domain
{
    /// <summary>
    /// Represents featured product records
    /// </summary>
    public partial class BS_FeaturedProducts : BaseEntity
    {
        public int ProductId { get; set; }
    }
}

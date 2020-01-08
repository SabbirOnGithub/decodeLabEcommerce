using BS.Plugin.NopStation.MobileWebApi.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BS.Plugin.NopStation.MobileWebApi.Data
{
    public partial class BS_CategoryIconMap : EntityTypeConfiguration<BS_CategoryIcon>
    {
        public BS_CategoryIconMap()
        {
            this.ToTable("BS_CategoryIcons");
            this.HasKey(x => x.Id);
        }
    }
}

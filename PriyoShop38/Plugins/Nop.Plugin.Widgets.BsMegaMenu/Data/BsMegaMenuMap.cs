using Nop.Plugin.Widgets.BsMegaMenu.Domain;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.BsMegaMenu.Data
{
    public class BsMegaMenuMap : EntityTypeConfiguration<BsMegaMenuDomain>
    {
        public BsMegaMenuMap()
        {
            ToTable("BsMegaMenu_ItemOrganizer");

            HasKey(m => m.Id);

            Property(m => m.CategoryId);
            Property(m => m.NumberOfColums);
            Property(m => m.ColumnPosition);

            this.Ignore(m => m.CategoryList);
            this.Ignore(m => m.SelectedCategory);
            this.Ignore(m => m.ParentCategoryId);
        }
    }
}

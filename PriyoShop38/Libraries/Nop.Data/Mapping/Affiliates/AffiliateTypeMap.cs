using Nop.Core.Domain.Affiliates;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace Nop.Data.Mapping.Affiliates
{
    public partial class AffiliateTypeMap : NopEntityTypeConfiguration<AffiliateType>
    {
        public AffiliateTypeMap()
        {
            this.ToTable("AffiliateType");
            this.HasKey(a => a.Id);

            this.Property(t => t.IdUrlParameter)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_AffiliateType_1") { IsUnique = true })
                );

            this.Property(t => t.NameUrlParameter)
                .HasColumnAnnotation(
                    "Index",
                    new IndexAnnotation(new IndexAttribute("IX_AffiliateType_2") { IsUnique = true })
                );
        }
    }
}

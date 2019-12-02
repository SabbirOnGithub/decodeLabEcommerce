using Nop.Data.Mapping;
using BS.Plugin.NopStation.MobileApp.Domain;

namespace BS.Plugin.NopStation.MobileApp.Data
{
    public partial class MessageTemplateMap : NopEntityTypeConfiguration<NotificationMessageTemplate>
    {
        public MessageTemplateMap()
        {

            this.ToTable("Bs_NotificationMessageTemplate");
            this.HasKey(mt => mt.Id);

            this.Property(mt => mt.Name).IsRequired().HasMaxLength(200);
            this.Property(mt => mt.Subject).HasMaxLength(1000);
        }
    }
}
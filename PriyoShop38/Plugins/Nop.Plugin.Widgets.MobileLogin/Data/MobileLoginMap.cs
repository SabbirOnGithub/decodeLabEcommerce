using Nop.Data.Mapping;
using Nop.Plugin.Widgets.MobileLogin.Domain;

namespace Nop.Plugin.Widgets.MobileLogin.Data
{
    public partial class MobileLoginMap : NopEntityTypeConfiguration<MobileLoginCustomer>
    {
        public MobileLoginMap()
        {
            this.ToTable("MobileLoginCustomer__CS");
            this.HasKey(x => x.Id);
        }
    }
}
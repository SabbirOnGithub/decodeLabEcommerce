using Nop.Data.Mapping;

namespace Nop.Plugin.Payments.EblSkyPay.Data
{
    public class EblSkyPayMap : NopEntityTypeConfiguration<Domain.EblSkyPay>
    {
        public EblSkyPayMap()
        {
            this.ToTable("EblSkyPay");
            this.HasKey(sc => sc.Id);            
            this.Property(p => p.OrderId);
            this.Property(p => p.Merchant);
            this.Property(p => p.Result);
            this.Property(p => p.SessionId);
            this.Property(p => p.SessionUpdateStatus);
            this.Property(p => p.SessionVersion);
            this.Property(p => p.SuccessIndicator);
            this.Property(p => p.CreatedOnUtc);
            this.Property(p => p.OrderRetriveResponse);
            this.Ignore(p => p.PaymentStatus);
            this.Property(p => p.PaymentStatusId);
            this.Property(p => p.Active);
            this.Property(p => p.PaymentDate);
        }
    }
}
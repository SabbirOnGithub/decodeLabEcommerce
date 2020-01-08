namespace Nop.Plugin.Widgets.EkShopA2I.Models
{
    public class NotificationModel
    {
        public bool ShowBadge { get; set; }
        public bool IsVendorRestricted { get; set; }
        public string Message { get; set; }
        public int ProductId { get; set; }
        public decimal CommissionRate { get; set; }
    }
}

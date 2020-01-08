using Nop.Core;
using System;

namespace Nop.Plugin.Widgets.MobileLogin.Domain
{
    /// <summary>
    /// Represents mobile login
    /// </summary>
    public partial class MobileLoginCustomer : BaseEntity
    {
        public int CustomerId { get; set; }
        public bool IsRegistered { get; set; }
        public string MobileNumber { get; set; }
        public string Token { get; set; }
        public bool IsTokenValid { get; set; }
        public int Used { get; set; }
        public DateTime CreatedOnUtc { get; set; }        
        public DateTime UpdatedOnUtc { get; set; }
    }
}
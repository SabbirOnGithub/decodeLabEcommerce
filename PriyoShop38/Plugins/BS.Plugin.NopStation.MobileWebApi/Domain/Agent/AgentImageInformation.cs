using System;

using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain.Agent
{
    public partial class AgentImageInformation : BaseEntity
    {
        public long AgentImageId { get; set; }
        public long AgentId { get; set; }
        public byte[] AgentImage { get; set; }
        public DateTime EntryDate { get; set; }
    }
}

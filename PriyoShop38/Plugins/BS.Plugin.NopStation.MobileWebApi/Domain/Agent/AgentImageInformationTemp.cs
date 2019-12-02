using System;

using Nop.Core;

namespace BS.Plugin.NopStation.MobileWebApi.Domain.Agent
{
    public partial class AgentImageInformationTemp : BaseEntity
    {
        public int AgentMasterId { get; set; }
        public byte[] AgentImage { get; set; }
        public DateTime EntryDate { get; set; }
    }
}

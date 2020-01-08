using System.Collections.Generic;

namespace Nop.Core.Domain.Affiliates
{
    public partial class AffiliateType : BaseEntity
    {
        private ICollection<Affiliate> _affiliates;

        public string Name { get; set; }

        public string NameUrlParameter { get; set; }

        public string IdUrlParameter { get; set; }

        public bool Active { get; set; }


        public virtual ICollection<Affiliate> Affiliates
        {
            get { return _affiliates ?? (_affiliates = new List<Affiliate>()); }
            protected set { _affiliates = value; }
        }
    }
}

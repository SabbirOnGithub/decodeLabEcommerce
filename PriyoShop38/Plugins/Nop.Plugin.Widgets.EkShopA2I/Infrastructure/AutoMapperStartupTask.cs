using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Misc.EkShopA2I.Infrastructure
{
   public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
        }

        public int Order
        {
            get { return 0; }
        }
    }
}

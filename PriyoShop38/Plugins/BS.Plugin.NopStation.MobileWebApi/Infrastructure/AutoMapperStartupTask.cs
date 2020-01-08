using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Infrastructure;

namespace BS.Plugin.NopStation.MobileWebApi.Infrastructure
{
   public class AutoMapperStartupTask : IStartupTask
    {
        public void Execute()
        {
            AutoMapperConfiguration.Init();
        }

        public int Order
        {
            get { return 0; }
        }
    }
}

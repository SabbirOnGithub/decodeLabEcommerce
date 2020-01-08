using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Nop.Web.Application_Start))]

namespace Nop.Web
{
    public partial class Application_Start
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(InfoDex.Startup))]
namespace InfoDex
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EmploiASP_2.Startup))]
namespace EmploiASP_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

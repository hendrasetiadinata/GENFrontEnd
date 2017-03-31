using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GENFrontEnd.Startup))]
namespace GENFrontEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

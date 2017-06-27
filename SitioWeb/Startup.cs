using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SitioWeb.Startup))]
namespace SitioWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

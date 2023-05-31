using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TichdiemDrGreen.Startup))]
namespace TichdiemDrGreen
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

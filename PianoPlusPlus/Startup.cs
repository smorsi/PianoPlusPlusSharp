using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PianoPlusPlus.Startup))]
namespace PianoPlusPlus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

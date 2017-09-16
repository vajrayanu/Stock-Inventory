using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ITST.Startup))]
namespace ITST
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

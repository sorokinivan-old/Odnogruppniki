using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Odnogruppniki.Startup))]
namespace Odnogruppniki
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

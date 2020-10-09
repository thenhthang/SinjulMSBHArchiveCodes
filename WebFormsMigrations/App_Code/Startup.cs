using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsMigrations.Startup))]
namespace WebFormsMigrations
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

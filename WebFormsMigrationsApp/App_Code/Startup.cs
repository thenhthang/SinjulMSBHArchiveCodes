using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFormsMigrationsApp.Startup))]
namespace WebFormsMigrationsApp
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}

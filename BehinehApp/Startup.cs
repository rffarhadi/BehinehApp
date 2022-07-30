using DLLCore.DBContext;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BehinehApp.Startup))]
namespace BehinehApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var t = 0;
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);
            ConfigureAuth(app);
            //test
        }
    }
}

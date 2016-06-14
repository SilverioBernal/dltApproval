using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Orkidea.Deloitte.Approval.FrontEnd.Startup))]
namespace Orkidea.Deloitte.Approval.FrontEnd
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}

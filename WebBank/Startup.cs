using Microsoft.Owin;
using Owin;


[assembly: OwinStartup(typeof(WebBank.Startup))]
namespace WebBank
{
    public class Startup
    {
        
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
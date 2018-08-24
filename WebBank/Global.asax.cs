using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Http;
using Akka.Actor;
using Akka.Pattern;
using Akka.Routing;

namespace WebBank
{
    public class Global : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
            var sys = ActorSystem.Create("WebBank");
            ActorSystemRefs.ActorSystem = sys;
            SystemActors.AccountsFacade = sys.ActorOf(Props.Empty.WithRouter(FromConfig.Instance), "accounts");
        }
        
        protected async void Application_End()
        {
            await CoordinatedShutdown.Get(ActorSystemRefs.ActorSystem).Run();
        }
    }
}
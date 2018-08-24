using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Shared;
using WebBank.Hubs;

namespace WebBank.Actors
{
    public class Notifications: ReceiveActor
    {
        #region Messages

        public class DebugCluster
        {
            public DebugCluster(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        #endregion

        private NotificationHub _hub;

        public Notifications()
        {
//            Receive<INotificationEnvelope>(envelope =>
//            {
//                switch (envelope.DestinationType)
//                {
//                        default:
//                            break;
//                }
//            });
        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("notificationHub") as NotificationHub;
        }


    }
}
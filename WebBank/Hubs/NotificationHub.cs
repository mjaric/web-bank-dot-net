using System.Text.RegularExpressions;
using System.Xml.Serialization;
using Microsoft.AspNet.SignalR;
using Shared;

namespace WebBank.Hubs
{
    public class NotificationHub : Hub
    {
        
        public bool SubscribeAccount(string account)
        {
            Groups.Add(Context.ConnectionId, account).Wait();
            return true;
        }

//        public void Send(INotificationEnvelope envelope)
//        {
//            Group(envelope.Account)
//        }
        
        
    }
}
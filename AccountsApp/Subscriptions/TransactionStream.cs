using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Persistence.EventStore;
using EventStore.ClientAPI;

namespace AccountsApp.Subscriptions
{
    public class TransactionStream : ReceiveActor
    {
        private EventStoreJournalSettings _settings;
        private ILoggingAdapter _log;
        private IEventStoreConnection _eventStoreConnection;

        private Dictionary<string, object> subscriptions = new Dictionary<string, object>();
        private EventStoreStreamCatchUpSubscription _subscription;

        public TransactionStream()
        {
            _settings = EventStorePersistence.Get(Context.System).JournalSettings;
            _log = Context.GetLogger();
        }

        protected override void PreStart()
        {
            base.PreStart();
            var connectionString = _settings.ConnectionString;
            var connectionName = _settings.ConnectionName;
            _eventStoreConnection = EventStoreConnection.Create(connectionString, connectionName);

            var self = Self;
            var stream = "accounts";
            var from = 0;
            var settings = new CatchUpSubscriptionSettings(500, 500, true, true);
            var credentials = _eventStoreConnection.Settings.DefaultUserCredentials;
            _subscription = _eventStoreConnection.SubscribeToStreamFrom(
                stream,
                from,
                settings,
                (s, e) => EventAppeared(self, s, e),
                s => LiveProcessingStarted(self, s),
                (s, r, ex) => SubscriptionDropped(self, s, r, ex),
                credentials
            );
        }

        protected override void PostStop()
        {
            _subscription?.Stop();
            base.PostStop();
        }

        private void SubscriptionDropped(
            IActorRef self,
            EventStoreCatchUpSubscription subscription,
            SubscriptionDropReason reason,
            Exception exception)
        {
            self.Tell(Kill.Instance);
        }

        private void LiveProcessingStarted(IActorRef self, EventStoreCatchUpSubscription subscription)
        {
            // ok
        }

        private Task EventAppeared(IActorRef self, EventStoreCatchUpSubscription subscription,
            ResolvedEvent resolvedEvent)
        {
            self.Tell(resolvedEvent);
            return Task.CompletedTask;
        }
    }
}
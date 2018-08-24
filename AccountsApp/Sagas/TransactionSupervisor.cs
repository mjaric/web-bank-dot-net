using System;
using Akka.Actor;
using Shared.Events;


namespace AccountsApp.Sagas
{
    public class TransactionSupervisor : ReceiveActor
    {
        private readonly IActorRef _accountsRef;

        public TransactionSupervisor(IActorRef accountsRef)
        {
            _accountsRef = accountsRef;
            Receive<IAccountTransaction>(msg =>
            {
                var child = Context.Child(msg.TransactionId);
                if (child.IsNobody())
                {
                    child = Context.ActorOf(Props.Create(() => new TransactionSaga(_accountsRef, msg.TransactionId)),
                        msg.TransactionId);
                }

                child.Forward(msg);
            });
        }

        protected override void PreStart()
        {
            Context.System.EventStream.Subscribe(Self, typeof(IAccountTransaction));
            base.PreStart();
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(exc =>
            {
                switch (exc)
                {
                    case NotImplementedException _:
                        return Directive.Stop;
                    case DomainException _:
                        return Directive.Resume;
                    default:
                        return Directive.Restart;
                }
            });
        }

        protected override void PreRestart(Exception reason, object message)
        {
            PostStop();
        }
    }
}
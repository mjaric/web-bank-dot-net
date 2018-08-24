using System;
using Akka.Actor;
using Shared.Messages;

namespace AccountsApp.Aggregates
{
    public class AccountSupervisor : ReceiveActor
    {
        public AccountSupervisor()
        {
            Receive<CommandEnvelope>(cmd =>
            {
                var child = Context.Child(cmd.AggregateId);
                if (child.IsNobody())
                {
                    child = Context.ActorOf(Props.Create(() => new AccountActor(cmd.AggregateId)), cmd.AggregateId);
                }

                child.Forward(cmd.Command);
            });
        }

        protected override SupervisorStrategy SupervisorStrategy()
        {
            return new OneForOneStrategy(exc =>
            {
                switch (exc)
                {
                    case NotImplementedException _:
                        return Directive.Stop;
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
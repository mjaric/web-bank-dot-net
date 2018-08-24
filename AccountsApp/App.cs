using System;
using System.IO;
using AccountsApp.Aggregates;
using AccountsApp.Sagas;
using Akka.Actor;
using Akka.Configuration;
using Shared.Commands;
using Shared.Messages;

namespace AccountsApp
{
    public class App
    {
        public void Start()
        {
            var config = ConfigurationFactory.ParseString(File.ReadAllText("akka.hocon"));
            var sys = ActorSystem.Create("WebBank", config);
            
            var accounts = AppAkkaRefs.AccountsRef = sys.ActorOf<AccountSupervisor>("accounts");
            var transactions = sys.ActorOf(Props.Create(() => new TransactionSupervisor(accounts)), "transactions");
            
            AppAkkaRefs.Cluster = sys;
            Test(accounts);
        }

        private void Test(IActorRef accounts)
        {
            var acc1 = "1";
            var acc2 = "2";
            var transactionId = Guid.NewGuid().ToString();

            var create1 = new CreateAccount
            {
                AccountNumber = acc1,
                InitialBalance = 0
            };

            AppAkkaRefs
                    .Cluster
                    .Scheduler
                    .ScheduleTellOnce(
                        TimeSpan.FromSeconds(5),
                        accounts,
                        new CommandEnvelope(acc1, create1),
                        ActorRefs.Nobody);

            
             
            var create2 = new CreateAccount
            {
                AccountNumber = acc2,
                InitialBalance = 1000
            };
            AppAkkaRefs
                    .Cluster
                    .Scheduler
                    .ScheduleTellOnce(
                        TimeSpan.FromSeconds(7),
                        accounts,
                        new CommandEnvelope(acc2, create2),
                        ActorRefs.Nobody);

            
            var withrowal = new Withrowal
            {
                Amount = 10,
                From = acc2,
                To = acc1,
                TransactionId = transactionId
            };
            AppAkkaRefs
                    .Cluster
                    .Scheduler
                    .ScheduleTellOnce(
                        TimeSpan.FromSeconds(12),
                        accounts,
                        new CommandEnvelope(acc2, withrowal),
                        ActorRefs.Nobody);
            /*
            var deposit = new Deposit
            {
                Amount = 10,
                CurrencyDate = DateTime.UtcNow,
                From = acc2,
                To = acc1,
                TransactionId = transactionId
            };
            AppAkkaRefs
                    .Cluster
                    .Scheduler
                    .ScheduleTellOnce(
                        TimeSpan.FromSeconds(13),
                        accounts,
                        new CommandEnvelope(acc1, deposit),
                        ActorRefs.Nobody);
            
            var commit = new CommitTransaction
            {
                AccountNumber = acc2,
                TransactionId = transactionId
            };
            AppAkkaRefs
                    .Cluster
                    .Scheduler
                    .ScheduleTellOnce(
                        TimeSpan.FromSeconds(14),
                        accounts,
                        new CommandEnvelope(acc2, commit),
                        ActorRefs.Nobody);
                        */
            
        }

        public async void Stop()
        {
            await CoordinatedShutdown.Get(AppAkkaRefs.Cluster).Run();
        }
    }

    public class AppAkkaRefs
    {
        public static ActorSystem Cluster;
        public static IActorRef SignalR;
        public static IActorRef AccountsRef;
    }
}
using Akka.Actor;
using Akka.Event;
using Shared.Commands;
using Shared.Events;
using Shared.Messages;

namespace AccountsApp.Sagas
{
    public class TransactionSaga : ReceiveActor
    {
        private readonly IActorRef _accountsRef;
        
        private string _transactionId;
        private string _fromAccount;
        private string _toAcount;
        private decimal _amount;

        private ILoggingAdapter _log = Context.GetLogger();

        public TransactionSaga(IActorRef accountsRef, string transactionId)
        {
            _accountsRef = accountsRef;
            
            Receive<AccountDebited>(msg =>
            {
                _log.Info("----------DEBITED---------");
                _fromAccount = msg.From;
                _toAcount = msg.To;
                _amount = msg.Amount;
                _transactionId = msg.TransactionId;
                
                var cmd = new Deposit
                {
                    Amount = msg.Amount,
                    From = msg.From,
                    To = msg.To,
                    TransactionId = msg.TransactionId
                };
                _accountsRef.Tell(new CommandEnvelope(msg.To, cmd));
            });
            
            Receive<AccountCredited>(msg =>
            {
                _log.Info("----------CREDITED---------");
                var cmd = new CommitTransaction
                {
                    AccountNumber = msg.From,
                    TransactionId = msg.TransactionId
                };
                _accountsRef.Tell(new CommandEnvelope(cmd.AccountNumber, cmd));        
            });
            
            Receive<AccountDebitCommited>(msg =>
            {
                _log.Info("----------COMMITED---------");
                _log.Info($"Transaction {msg.TransactionId} successfully finished.");
                Self.Tell(PoisonPill.Instance);
            });
            
            Receive<AccountDebitRolledBack>(msg =>
            {
                _log.Info("----------ROLLBACK---------");
                _log.Info($"Transaction {msg.TransactionId} rolled back");
                Self.Tell(PoisonPill.Instance);
            });
        }

       
    }
}
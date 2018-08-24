using System.Collections.Generic;
using Shared.Events;

namespace AccountsApp.Aggregates
{
    public class Account
    {
        public string AccountNumber { get; private set; }
        public decimal Balance { get; private set; }
        public decimal AvailableBalance { get; private set; }

        public Dictionary<string, decimal> OutstandingTransaction { get; } = new Dictionary<string, decimal>();


        public void Apply(AccountCreated @event)
        {
            AccountNumber = @event.AccountNumber;
            Balance = @event.InitialFunds;
            AvailableBalance = @event.InitialFunds;
        }

        public void Apply(AccountCredited @event)
        {
            Balance += @event.Amount;
            AvailableBalance += @event.Amount;
        }

        public void Apply(AccountDebited @event)
        {
            AvailableBalance -= @event.Amount;
            OutstandingTransaction.Add(@event.TransactionId, @event.Amount);
        }

        public void Apply(AccountDebitCommited @event)
        {
            Balance -= @event.Amount;
            OutstandingTransaction.Remove(@event.TransactionId);
        }

        public void Apply(AccountDebitRolledBack @event)
        {
            AvailableBalance += @event.Amount;
            OutstandingTransaction.Remove(@event.TransactionId);
        }

        public override string ToString()
        {
            return $@"
            AccountNumber  = {AccountNumber}
            Balance  = {Balance}
            AvailableBalance = {AvailableBalance}
            ";
        }
    }
}
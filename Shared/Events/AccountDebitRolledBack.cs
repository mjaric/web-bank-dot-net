namespace Shared.Events
{
    public class AccountDebitRolledBack : IDomainEvent, IAccountTransaction
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public string Reason { get; set; }
        public decimal Amount { get; set; }
    }
}
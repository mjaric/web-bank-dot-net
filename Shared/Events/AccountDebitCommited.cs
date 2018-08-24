using System;

namespace Shared.Events
{
    public class AccountDebitCommited : IDomainEvent, IAccountTransaction
    {
        public string TransactionId { get; set; }
        public decimal Amount { get; set; }
        public DateTime EndedDate { get; set; }
    }
}
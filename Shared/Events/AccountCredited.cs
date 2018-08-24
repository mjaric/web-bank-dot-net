using System;

namespace Shared.Events
{
    public class AccountCredited : IDomainEvent, IAccountTransaction
    {
        public string TransactionId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public DateTime CurrencyDate { get; set; }
    }
}
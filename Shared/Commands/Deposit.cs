using System;

namespace Shared.Commands
{
    public class Deposit: ICommand
    {
        public string TransactionId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public DateTime CurrencyDate { get; set; }
    }
}
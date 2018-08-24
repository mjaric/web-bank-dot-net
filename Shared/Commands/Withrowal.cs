namespace Shared.Commands
{
    public class Withrowal : ICommand
    {
        public string TransactionId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
    }
}
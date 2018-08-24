namespace Shared.Commands
{
    public class RollbackTransaction: ICommand
    {
        public string AccountNumber { get; set; }
        public string TransactionId { get; set; }
        public string Reason { get; set; }
    }
}
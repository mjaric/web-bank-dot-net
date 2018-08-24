namespace Shared.Commands
{
    public class CommitTransaction : ICommand
    {
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
    }
}
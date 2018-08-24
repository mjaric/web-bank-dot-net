namespace Shared.Commands
{
    public class CreateAccount: ICommand
    {
        public string AccountNumber { get; set; }
        public decimal InitialBalance { get; set; }
    }
}
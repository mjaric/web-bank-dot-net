namespace Shared.Events
{
    public class AccountCreated : IDomainEvent
    {
        public string AccountNumber { get; set; }
        public decimal InitialFunds { get; set; }
    }
}
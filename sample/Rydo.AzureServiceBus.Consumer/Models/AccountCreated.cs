namespace Rydo.AzureServiceBus.Consumer.Models
{
    public class AccountCreated
    {
        public string AccountNumber { get; }
        public DateTime CreatedAt { get; }
        
        public AccountCreated(string accountNumber)
        {
            CreatedAt = DateTime.Now;
            AccountNumber = accountNumber;
        }   
    }
}
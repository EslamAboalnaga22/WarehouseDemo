namespace Warehouse.SharedLibrary.Configuration
{
    public class RabbitMqConfiguration
    {
        public string Server { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string QueueName { get; set; } = string.Empty;
        public string ExchangeName { get; set; } = string.Empty;
        public string BindingKey { get; set; } = string.Empty;
    }
}

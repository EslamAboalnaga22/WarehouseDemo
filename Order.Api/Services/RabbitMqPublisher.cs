using Order.Api.Events;
using RabbitMQ.Client;
using Warehouse.SharedLibrary.Configuration;
using System.Text.Json;

namespace Order.Api.Services
{
    public class RabbitMqPublisher: IRabbitMqPublisher
    {
        private readonly RabbitMqConfiguration _rabbitMqConfig;
        private readonly ConnectionFactory _factory;
        public RabbitMqPublisher(RabbitMqConfiguration rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig;
            _factory = new ConnectionFactory
            {
                HostName = _rabbitMqConfig.Server,
                UserName = _rabbitMqConfig.UserName,
                Password = _rabbitMqConfig.Password

            };
        }
        public async Task Publish(OrderCreatedEvent message)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _rabbitMqConfig.ExchangeName, 
                type: ExchangeType.Fanout);

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            var props = new BasicProperties();

            await channel.BasicPublishAsync(
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: "",
                mandatory: false,
                basicProperties: props,
                body: body);
        }
    }
}

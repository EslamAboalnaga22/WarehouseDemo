using Order.Api.Events;
using RabbitMQ.Client;
using System.Text.Json;
using Warehouse.SharedLibrary.Configuration;

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
        public async Task Publish(OrderCreatedEvent message, CancellationToken stoppingToken)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _rabbitMqConfig.ExchangeName,
                type: ExchangeType.Direct
            );

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            await channel.QueueDeclareAsync(
                 queue: _rabbitMqConfig.QueueName,
                 durable: true,
                 exclusive: false,
                 autoDelete: false
            );


            await channel.QueueBindAsync(
                queue: _rabbitMqConfig.QueueName,
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: "");

            await channel.BasicPublishAsync(
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: "",
                mandatory: false,
                basicProperties: new BasicProperties(),
                body: body,
                cancellationToken: stoppingToken);
      

            // Wait for Inventory.API response
            //await tcs.Task.WaitAsync(
            //    TimeSpan.FromSeconds(10),
            //    stoppingToken);

            //await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

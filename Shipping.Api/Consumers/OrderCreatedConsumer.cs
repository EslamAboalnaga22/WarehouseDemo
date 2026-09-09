using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shipping.Api.Events;
using System.Text.Json;
using Warehouse.SharedLibrary.Configuration;

namespace Shipping.Api.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {

        private readonly RabbitMqConfiguration _rabbitMqConfig;
        private readonly ConnectionFactory _factory;
        public OrderCreatedConsumer(RabbitMqConfiguration rabbitMqConfig)
        {
            _rabbitMqConfig = rabbitMqConfig;
            _factory = new ConnectionFactory
            {
                HostName = _rabbitMqConfig.Server,
                UserName = _rabbitMqConfig.UserName,
                Password = _rabbitMqConfig.Password

            };
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var connection = await _factory.CreateConnectionAsync();

            var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(
                exchange: _rabbitMqConfig.ExchangeName,
                type: ExchangeType.Topic);

            //var queueName = (await channel.QueueDeclareAsync()).QueueName;
            var queueName = "shipping.order-created";

            await channel.QueueDeclareAsync(
                 queue: queueName,
                 durable: true,
                 exclusive: false,
                 autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: queueName,
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: _rabbitMqConfig.BindingKey,
                arguments: null
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

                Console.WriteLine("Topic Exchange");
                Console.WriteLine("===============");
                Console.WriteLine($"[Shipping.API] \n Received OrderCreatedEvent (using routing key --> {_rabbitMqConfig.BindingKey}) : \n OrderId={message.OrderId},\n ProductId={message.ProductId}, \n Quantity={message.Quantity}");
            };

            await channel.BasicConsumeAsync(
                queue: queueName,
                autoAck: true,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

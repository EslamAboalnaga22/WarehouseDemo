using Inventory.Api.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;
using Warehouse.SharedLibrary.Configuration;

namespace Inventory.Api.Consumers
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

            await channel.ExchangeDeclareAsync(exchange: _rabbitMqConfig.ExchangeName,
                type: ExchangeType.Direct);

            await channel.ExchangeDeclareAsync(exchange: _rabbitMqConfig.DLExchangeName,
                 type: ExchangeType.Fanout);

            var arguments = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", _rabbitMqConfig.DLExchangeName },
                { "x-message-ttl", 1000 } // 1 seconds
            };

            var queueName = await channel.QueueDeclareAsync(
                 queue: _rabbitMqConfig.QueueName,
                 durable: true,
                 exclusive: false,
                 autoDelete: false,
                 arguments: arguments
            );

            await channel.QueueBindAsync(
                queue: _rabbitMqConfig.QueueName,
                exchange: _rabbitMqConfig.ExchangeName,
                routingKey: "");

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {      
                var body = ea.Body.ToArray();

                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

                Console.WriteLine($"[Inventory.API] \n OrderId={message.OrderId},\n ProductId={message.ProductId}, \n Quantity={message.Quantity}");
            };

            //await channel.BasicConsumeAsync(
            //    queue: _rabbitMqConfig.QueueName,
            //    autoAck: true,
            //    consumer: consumer
            //);


            // Dead Letter Exchange Consumer
            await channel.QueueDeclareAsync(
                queue: _rabbitMqConfig.DLQExchangeName,
                durable: true,
                exclusive: false,
                autoDelete: false
            );

            await channel.QueueBindAsync(
                queue: _rabbitMqConfig.DLQExchangeName,
                exchange: _rabbitMqConfig.DLExchangeName,
                routingKey: "");

            var dlxConsumer = new AsyncEventingBasicConsumer(channel);

            dlxConsumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();

                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

                Console.WriteLine("====================");
                Console.WriteLine("Dead Letter Exchange");
                Console.WriteLine("====================");
                Console.WriteLine("(That is from Dead Letter Exchange)");
                Console.WriteLine($"[Inventory.API] \n OrderId={message.OrderId},\n ProductId={message.ProductId}, \n Quantity={message.Quantity}");
            };

            await channel.BasicConsumeAsync(
                queue: _rabbitMqConfig.DLQExchangeName,
                autoAck: true,
                consumer: dlxConsumer
            );

            Console.WriteLine("Consumeing..");


            //await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

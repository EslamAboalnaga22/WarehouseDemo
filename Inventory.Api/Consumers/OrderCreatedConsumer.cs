using Inventory.Api.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
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

            //var queueName = (await channel.QueueDeclareAsync()).QueueName;
            var requestQueueName = "order-created-queue";

            var replyQueue = await channel.QueueDeclareAsync(
                 queue: requestQueueName,
                 durable: true,
                 exclusive: false,
                 autoDelete: false
            );

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                await Task.Delay(5000, stoppingToken);

                var body = ea.Body.ToArray();

                var message = JsonSerializer.Deserialize<OrderCreatedEvent>(body);

                Console.WriteLine("=====================");
                Console.WriteLine("Request-Reply Pattern");
                Console.WriteLine("=====================");
                Console.WriteLine("(Request Received.)");
                Console.WriteLine($"[Inventory.API] \n OrderId={message.OrderId},\n ProductId={message.ProductId}, \n Quantity={message.Quantity}");
                Console.WriteLine("=====================");
                Console.WriteLine("Sending Reply...");
                Console.WriteLine("=====================");

                var responseMessage = "Done From the [Inventory.API]";
                var resposneBody = Encoding.UTF8.GetBytes(responseMessage);

                var props = new BasicProperties
                {
                    CorrelationId = ea.BasicProperties.CorrelationId
                };

                await Task.Delay(5000, stoppingToken);

                await channel.BasicPublishAsync(
                    exchange: "",
                    routingKey: ea.BasicProperties.ReplyTo,
                    false,
                    basicProperties: props,
                    body: resposneBody);

                Console.WriteLine("=================================");
                Console.WriteLine("[Inventory.API] Reply Sent");
                //Console.WriteLine(responseMessage);
                Console.WriteLine("=================================");

            };

            await channel.BasicConsumeAsync(
                queue: requestQueueName,
                autoAck: true,
                consumer: consumer
            );

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

using Order.Api.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
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

            var requestQueueName = "order-created-queue";
            var replyQueueName = "order-reply-queue";

            await channel.QueueDeclareAsync(
                 queue: requestQueueName,
                 durable: true,
                 exclusive: false,
                 autoDelete: false
            );

            
            await channel.QueueDeclareAsync(
                queue: replyQueueName,
                durable: false,
                exclusive: true,
                autoDelete: true);


            var correlationId = Guid.NewGuid().ToString();

            var tcs = new TaskCompletionSource<string>(
                TaskCreationOptions.RunContinuationsAsynchronously);


            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                if (ea.BasicProperties.CorrelationId != correlationId)
                    return;

                var body = ea.Body.ToArray();

                var response = Encoding.UTF8.GetString(ea.Body.ToArray());

                tcs.TrySetResult(response);

                Console.WriteLine("=================================");
                Console.WriteLine($"Response: {response}");
                Console.WriteLine("=================================");
                Console.WriteLine("Reply Received From [Inventory.API]");
                Console.WriteLine("=================================");

                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(
                queue: replyQueueName,
                autoAck: true,
                consumer: consumer
            );

            // Request properties
            var props = new BasicProperties
            {
                ReplyTo = replyQueueName,
                CorrelationId = correlationId
            };

            var body = JsonSerializer.SerializeToUtf8Bytes(message);

            await Task.Delay(5000, stoppingToken);

            await channel.BasicPublishAsync(
            exchange: "",
            routingKey: requestQueueName,
            mandatory: false,
            basicProperties: props,
            body: body);

            // Wait for Inventory.API response
            //await tcs.Task.WaitAsync(
            //    TimeSpan.FromSeconds(10),
            //    stoppingToken);

            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}

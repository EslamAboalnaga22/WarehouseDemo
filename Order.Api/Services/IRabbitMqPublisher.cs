using Order.Api.Events;

namespace Order.Api.Services
{
    public interface IRabbitMqPublisher
    {
        Task Publish(OrderCreatedEvent message);
    }
}

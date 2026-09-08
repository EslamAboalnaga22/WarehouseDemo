namespace Inventory.Api.Events
{
    public record OrderCreatedEvent(
    int OrderId,
    int ProductId,
    int Quantity
    );
}

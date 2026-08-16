namespace Order.Api.Requests
{
    public class CreateOrderRequest
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}

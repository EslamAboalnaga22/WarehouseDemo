namespace Shipping.Api.Models
{
    public class Shipment
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string Status { get; set; } = "Created";
        public DateTime CreatedAt { get; set; }
    }
}

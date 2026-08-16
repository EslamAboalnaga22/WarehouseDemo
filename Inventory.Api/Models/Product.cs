namespace Inventory.Api.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
        public int Stock { get; set; } = 0;
    }
}

using Inventory.Api.Models;

namespace Inventory.Api.Data
{
    public static class VirtualProducts
    {
        public static List<Product> GetProducts()
        {
            return
            [
                new Product { Id = 1, Name = "Laptop", Price = 100.0m, Stock = 10 },
                new Product { Id = 2, Name = "Phone", Price = 200.0m, Stock = 20 },
                new Product { Id = 3, Name = "Tablet", Price = 300.0m, Stock = 30 },
                new Product { Id = 4, Name = "Monitor", Price = 400.0m, Stock = 40 },
                new Product { Id = 5, Name = "Keyboard", Price = 500.0m, Stock = 50 }
            ];
        }
    }
}

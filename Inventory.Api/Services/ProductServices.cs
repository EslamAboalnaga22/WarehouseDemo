using Inventory.Api.Data;
using Inventory.Api.Models;
using System.Linq.Expressions;
using Warehouse.SharedLibrary.Responses;

namespace Inventory.Api.Services
{
    public class ProductServices : IProductServices
    {
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            //throw new TimeoutException("Service Not Available");
            await Task.Delay(10000);

            var products =  VirtualProducts.GetProducts();

            return products is not null ? products : Enumerable.Empty<Product>();
        }

        public async Task<Product> GetByAsync(Expression<Func<Product, bool>> predicate)
        {
            var product =  VirtualProducts.GetProducts().FirstOrDefault(x => x.Name == predicate.Name);

            return product is not null ? product : null!;
        }

        public async Task<Product> GetByIdAsync(int id)
        {
            var product = VirtualProducts.GetProducts().FirstOrDefault(x => x.Id == id);

            return product is not null ? product : null!;
        }

                public async Task<Response> CreateAsync(Product entity)
        {
            if(entity is null)
                return new Response(false, "Product cannot be null.");

            VirtualProducts.GetProducts().Add(entity);

            return new Response(true, "Product created successfully.");
        }

        public async Task<Response> DeleteAsync(int id)
        {
            if (id <= 0)
                return new Response(false, "Invalid product ID.");

            var product = VirtualProducts.GetProducts().FirstOrDefault(p => p.Id == id);

            if (product is null)
                return new Response(false, "Product not found.");

            VirtualProducts.GetProducts().Remove(product);

            return new Response(true, "Product deleted successfully.");
        }

        public async Task<Response> UpdateAsync(Product entity)
        {
            if (entity is null)
                return new Response(false, "Product cannot be null.");

            var product = VirtualProducts.GetProducts().FirstOrDefault(p => p.Id == entity.Id);

            if (product is null)
                return new Response(false, "Product not found.");

            VirtualProducts.GetProducts().Remove(product);
            VirtualProducts.GetProducts().Add(entity);

            return new Response(true, "Product updated successfully.");
        }

        public async Task<Response> UpdateStockAsync(StockRequest request)
        {
            var product = VirtualProducts.GetProducts().FirstOrDefault(p => p.Id == request.ProductId);

            if (product is null)
                return new Response(false, "Product not found.");

            if (product.Stock < request.Quantity)
                return new Response(false, "Not enough stock");

            product.Stock -= request.Quantity;

            return new Response(true, "Stock updated successfully.");
        }
    }
}


using Inventory.Api.Models;
using Warehouse.SharedLibrary.Interface;
using Warehouse.SharedLibrary.Responses;

namespace Inventory.Api.Services
{
    public interface IProductServices : IGenericInterface<Product>
    {
        Task<Response> UpdateStockAsync(StockRequest request);
    }
}

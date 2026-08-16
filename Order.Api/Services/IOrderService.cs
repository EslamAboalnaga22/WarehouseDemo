using Order.Api.Requests;
using Order.Api.Requests_Responses;

namespace Order.Api.Services
{
    public interface IOrderService
    {
        Task<List<ProductResponse>> GetProductAsync();
        Task<Models.Order> CreateOrderAsync(CreateOrderRequest request);
    }
}

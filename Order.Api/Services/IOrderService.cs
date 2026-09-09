using Order.Api.Requests;
using Order.Api.Requests_Responses;

namespace Order.Api.Services
{
    public interface IOrderService
    {
        Task<List<ProductResponse>> GetProductAsync();
        Task<List<ProductResponse>> GetTestAlternativeProductAsync();
        Task<List<ProductResponse>> GetTestAuthorizationAsync();
        Task<Models.Order> CreateOrderAsync(CreateOrderRequest request , string bindingKey);
    }
}

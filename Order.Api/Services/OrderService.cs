using Order.Api.Requests;
using Order.Api.Requests_Responses;

namespace Order.Api.Services
{
    public class OrderService(HttpClient httpClient) : IOrderService
    {       
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<ProductResponse>> GetProductAsync()
        {
            var responseProducts = await _httpClient
                .GetFromJsonAsync<List<ProductResponse>>($"api/inventory");

            if (responseProducts == null || !responseProducts.Any())
                throw new Exception("No products found.");

            return responseProducts is not null ? responseProducts : null!;
        }
        public async Task<Models.Order> CreateOrderAsync(CreateOrderRequest request)
        {
            var response = await _httpClient
                .PostAsJsonAsync("api/inventory/Stock", request);    

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Could not reserve stock");
            }

            // Simulate order creation logic
            var order = new Models.Order
            {
                Id = new Random().Next(1, 1000),
                ProductId = request.ProductId,
                Quantity = request.Quantity,
                Status = "Created"
            };

            return order;
        }
    }
}

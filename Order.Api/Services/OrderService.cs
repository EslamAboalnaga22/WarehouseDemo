using Order.Api.Requests;
using Order.Api.Requests_Responses;

namespace Order.Api.Services
{
    public class OrderService(IHttpClientFactory httpClientFactory) : IOrderService
    {
        private readonly HttpClient _OrderClient = httpClientFactory.CreateClient("OrderClient");
        private readonly HttpClient _AlternativeClient = httpClientFactory.CreateClient("AlternativeClient");

        public async Task<List<ProductResponse>> GetProductAsync()
        {
            var responseProducts = await _OrderClient
                .GetFromJsonAsync<List<ProductResponse>>($"api/inventory");

            if (responseProducts == null || !responseProducts.Any())
                throw new Exception("No products found.");

            return responseProducts is not null ? responseProducts : null!;
        }
        public async Task<List<ProductResponse>> GetTestAlternativeProductAsync()
        {
            var responseProducts = await _AlternativeClient
                .GetFromJsonAsync<List<ProductResponse>>($"api/shipping");

            if (responseProducts == null || !responseProducts.Any())
                throw new Exception("No products found.");

            return responseProducts is not null ? responseProducts : null!;
        }

        public async Task<Models.Order> CreateOrderAsync(CreateOrderRequest request)
        {
            var response = await _OrderClient
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

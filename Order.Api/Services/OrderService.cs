using Order.Api.Events;
using Order.Api.Requests;
using Order.Api.Requests_Responses;

namespace Order.Api.Services
{
    public class OrderService(IHttpClientFactory httpClientFactory, IRabbitMqPublisher rabbitMqPublisher) : IOrderService
    {
        private readonly HttpClient _OrderClient = httpClientFactory.CreateClient("OrderClient");
        private readonly HttpClient _AlternativeClient = httpClientFactory.CreateClient("AlternativeClient");

        private readonly IRabbitMqPublisher _rabbitMqPublisher = rabbitMqPublisher;

        public async Task<List<ProductResponse>> GetProductAsync()
        {
            //var responseProducts = await _OrderClient
            //    .GetFromJsonAsync<List<ProductResponse>>($"api/inventory");
            var responseProducts = await _OrderClient
                .GetFromJsonAsync<List<ProductResponse>>($"gateway/inventory");

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

        public async Task<List<ProductResponse>> GetTestAuthorizationAsync()
        {
            List<ProductResponse> productsAuthorization = [
                new ProductResponse { Id = 111, Name = "Product A TestAuthorization", Price = 10.99m, Stock = 100 },
                new ProductResponse { Id = 222, Name = "Product B TestAuthorization", Price = 15.99m, Stock = 50 },
                ];

            return productsAuthorization is not null ? productsAuthorization : null!;
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

            var orderCreatedEvent = new OrderCreatedEvent(
                new Random().Next(1, 5), 
                request.ProductId, 
                request.Quantity
             );


            await _rabbitMqPublisher.Publish(orderCreatedEvent);

            return order;
        }
    }
}

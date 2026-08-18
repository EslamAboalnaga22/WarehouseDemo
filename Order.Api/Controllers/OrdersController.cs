using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.Api.Requests;
using Order.Api.Services;

namespace Order.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController(IOrderService orderService) : ControllerBase
    {
        private readonly IOrderService _orderService = orderService;

        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _orderService.GetProductAsync();

            if (products is null)
                return NotFound("Products Request not found");

            return Ok(products);
        }

        [HttpGet("alternative")]
        public async Task<IActionResult> GetAlternativeProducts()
        {
            var products = await _orderService.GetTestAlternativeProductAsync();

            if (products is null)
                return NotFound("Alternative products not found");

            return Ok(products);
        }

        [HttpGet("test-rate-limit")]
        public async Task<IActionResult> TestRateLimitBurst()
        {
            using var client = new HttpClient();

            var tasks = Enumerable.Range(1, 5)
                .Select(async i =>
                {
                    var response = await client.GetAsync(
                        "http://localhost:5002/api/Orders");

                    return new
                    {
                        Request = i,
                        StatusCode = (int)response.StatusCode
                    };
                });

            var results = await Task.WhenAll(tasks);

            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderRequest request)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            await _orderService.CreateOrderAsync(request);

            return Ok("Order created successfully.");

        }
    }
}

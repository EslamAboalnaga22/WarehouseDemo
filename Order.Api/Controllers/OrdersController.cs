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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shipping.Api.Services;

namespace Shipping.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShippingController(TestHedjing testHedjing) : ControllerBase
    {
        private readonly TestHedjing _testHedjing = testHedjing;

        [HttpGet]
        public async Task<IActionResult> GetTestAlternativeProductAsync()
        {
            var products = await _testHedjing.GetProductsAsync();

            return Ok(products);
        }
    }
}

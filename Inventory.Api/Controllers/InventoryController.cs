
using Inventory.Api.Models;
using Inventory.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Inventory.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController(IProductServices productService) : ControllerBase
    {
        private readonly IProductServices _productService = productService;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _productService.GetAllAsync();

            if (products is null)
                return NotFound("Products Request not found");

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product is null)
                return NotFound("Product Request not found");

            return Ok(product);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string searchTerm)
        {
            var product = await _productService.GetByAsync(x => x.Name.Contains(searchTerm));

            if (product is null)
                return NotFound("Product Request not found");

            return Ok(product);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.CreateAsync(product);

            return response.Flag is true ? CreatedAtAction(nameof(GetById), new { id = product.Id }, product) : BadRequest(response);
        }

        [HttpPost("Stock")]
        public async Task<IActionResult> UpdateStock([FromBody] StockRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.UpdateStockAsync(request);

            return response.Flag is true ? Ok(response) : BadRequest(response);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Product product)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var response = await _productService.UpdateAsync(product);

            return response.Flag is true ? CreatedAtAction(nameof(GetById), new { id = product.Id }, product) : BadRequest(response);
        }



        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _productService.DeleteAsync(id);

            return response.Flag is true ? Ok(response) : BadRequest(response);
        }
    }
}

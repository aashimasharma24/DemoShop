using DemoShop.Core.DataObjects;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
            Log.Information("ProductController initialized.");
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetAllProducts()
        {
            var products = _productService.GetAll();
            return Ok(products);
        }

        [HttpGet("{guid}")]
        [Authorize]
        public IActionResult GetProductById(String guid)
        {
            var product = _productService.GetByGUID(guid);
            if (product == null)
                return NotFound(new { message = "Product not found" });
            return Ok(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddProduct([FromBody] Product product)
        {
            _productService.Add(product);
            return Ok(product);
        }

        [HttpPut("{guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateProduct(String guid, [FromBody] Product product)
        {
            _productService.Update(guid, product);
            return Ok(new { message = "Product updated successfully" });
        }

        [HttpDelete("{guid}")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteProduct(String guid)
        {
            _productService.Delete(guid);
            return Ok(new { message = "Product deleted successfully" });
        }
    }
}

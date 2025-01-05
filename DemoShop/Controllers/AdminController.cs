using Asp.Versioning;
using DemoShop.Manager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoShop.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProductRepository _productRepository;

        public AdminController(
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _userRepository = userRepository;
            _productRepository = productRepository;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> GetAllOrders()
        {
            var orders = await _orderRepository.GetAllAsync();
            return Ok(orders);
        }

        [HttpPut("orders/{orderId}/status")]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, [FromQuery] string status)
        {
            await _orderRepository.UpdateStatusAsync(orderId, status);
            return Ok("Order status updated.");
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();
            return Ok(users);
        }

        [HttpGet("inventory")]
        public async Task<IActionResult> GetInventory()
        {
            var products = await _productRepository.GetAllAsync();
            return Ok(products);
        }

        [HttpPut("inventory/{productId}/stock")]
        public async Task<IActionResult> UpdateStock(int productId, [FromQuery] int stockCount)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return NotFound();

            product.StockCount = stockCount;
            await _productRepository.UpdateAsync(product);

            return Ok("Stock count updated.");
        }

        [HttpPut("promote/{userId}")]
        public async Task<IActionResult> PromoteUser(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound("User not found.");

            user.Role = "Admin";
            await _userRepository.UpdateAsync(user);
            return Ok("User promoted to Admin.");
        }
    }
}
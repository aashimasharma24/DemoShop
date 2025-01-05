using Asp.Versioning;
using DemoShop.Core.DataObjects;
using DemoShop.Manager.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userId = GetUserId();
            var items = await _cartRepository.GetCartItemsAsync(userId);
            return Ok(items);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(int productId, int quantity)
        {
            var userId = GetUserId();
            await _cartRepository.AddOrUpdateCartItemAsync(userId, productId, quantity);
            return Ok("Item added to cart.");
        }

        [HttpDelete("{cartItemId}")]
        public async Task<IActionResult> Remove(int cartItemId)
        {
            await _cartRepository.RemoveCartItemAsync(cartItemId);
            return Ok("Item removed from cart.");
        }

        [HttpDelete("clear")]
        public async Task<IActionResult> Clear()
        {
            var userId = GetUserId();
            await _cartRepository.ClearCartAsync(userId);
            return Ok("Cart cleared.");
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst("userId")!.Value);
        }
    }
}

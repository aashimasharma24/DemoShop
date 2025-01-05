using DemoShop.Core.DataObjects;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ShoppingCartController : ControllerBase
    {
        private readonly IShoppingCartService _cartService;

        public ShoppingCartController(IShoppingCartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        public IActionResult GetCartItems()
        {
            var userId = User.FindFirst("sub")?.Value;
            Log.Information("Fetching cart items for user: {UserId}", userId);
            var items = _cartService.GetCartItems(userId);
            return Ok(items);
        }

        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            try
            {
                item.UserId = User.FindFirst("sub")?.Value;
                Log.Information("Adding item to cart for user: {UserId}", item.UserId);
                _cartService.AddToCart(item);
                return Ok(new { message = "Item added to cart" });
            }
            catch (ArgumentException ex)
            {
                Log.Warning("Validation error: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize(Roles = "User")]
        [HttpPut("{id}")]
        public IActionResult UpdateCartItem([FromBody] CartItem item)
        {
            Log.Information("Updating cart item GUID: {GUID}", item.Guid);
            _cartService.UpdateCartItem(item);
            return Ok(new { message = "Cart item updated" });
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{id}")]
        public IActionResult RemoveFromCart(String guid)
        {
            Log.Information("Removing item from cart GUID: {GUID}", guid);
            _cartService.RemoveFromCart(guid);
            return Ok(new { message = "Item removed from cart" });
        }

        [Authorize(Roles = "User")]
        [HttpDelete("clear")]
        public IActionResult ClearCart()
        {
            var userId = User.FindFirst("sub")?.Value;
            Log.Information("Clearing cart for user: {UserId}", userId);
            _cartService.ClearCart(userId);
            return Ok(new { message = "Cart cleared successfully" });
        }

        [Authorize(Roles = "User")]
        [HttpGet("total")]
        public IActionResult GetCartTotal()
        {
            var userId = User.FindFirst("sub")?.Value;
            var total = _cartService.CalculateCartTotal(userId);
            Log.Information("Cart total for user {UserId}: {Total}", userId, total);
            return Ok(new { total });
        }
    }
}

using DemoShop.Core.DataObjects;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class OrderController : ControllerBase 
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
            Log.Information("OrderController initialized.");
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public IActionResult PlaceOrder([FromBody] Order order)
        {
            if (order == null || !order.Items.Any())
            {
                Log.Warning("Order validation failed: Empty order.");
                return BadRequest(new { message = "Order cannot be empty." });
            }

            try
            {
                Log.Information("Placing order for user: {UserId}", User.FindFirst("sub")?.Value);
                _orderService.Add(order);
                return Ok(new { message = "Order placed successfully." });
            }
            catch (Exception ex)
            {
                Log.Error("Error placing order: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while placing the order." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            try
            {
                Log.Information("Fetching all orders (Admin).");
                var orders = _orderService.GetAll();
                return Ok(orders);
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching orders: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while fetching orders." });
            }
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{guid}")]
        public IActionResult GetOrderById(String guid)
        {
            try
            {
                Log.Information("Fetching order with ID: {GUId}", guid);
                var order = _orderService.GetByGUID(guid);
                if (order == null)
                {
                    Log.Warning("Order not found with ID: {GUId}", guid);
                    return NotFound(new { message = "Order not found." });
                }
                return Ok(order);
            }
            catch (Exception ex)
            {
                Log.Error("Error fetching order by GUID: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while fetching the order." });
            }
        }

        [Authorize(Roles = "User")]
        [HttpDelete("{guid}")]
        public IActionResult CancelOrder(String guid)
        {
            try
            {
                Log.Information("Cancelling order with GUID: {GUID}", guid);
                _orderService.Delete(guid);
                return Ok(new { message = "Order cancelled successfully." });
            }
            catch (Exception ex)
            {
                Log.Error("Error cancelling order: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while cancelling the order." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{guid}/status")]
        public IActionResult UpdateOrderStatus(String guid, [FromBody] string status)
        {
            try
            {
                Log.Information("Updating order status for ID: {GUID}", guid);
                _orderService.Update(guid, status);
                return Ok(new { message = "Order status updated successfully." });
            }
            catch (Exception ex)
            {
                Log.Error("Error updating order status: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while updating order status." });
            }
        }
    }
}

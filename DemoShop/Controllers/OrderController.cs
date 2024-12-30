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
            Log.Information("Placing new order for user.");
            _orderService.Add(order);
            return Ok(new { message = "Order placed successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            Log.Information("Fetching all orders (Admin).");
            var orders = _orderService.GetAll();
            return Ok(orders);
        }

        [Authorize(Roles = "Admin,User")]
        [HttpGet("{id}")]
        public IActionResult GetOrderById(String guid)
        {
            Log.Information("Fetching order with GUID: {GUID}", guid);
            var order = _orderService.GetByGUID(guid);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            return Ok(order);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public IActionResult UpdateOrder(String guid, [FromBody] Order order)
        {
            Log.Information("Updating order with ID: {GUID}", guid);
            _orderService.Update(guid, order);
            return Ok(new { message = "Order updated successfully" });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteOrder(String guid)
        {
            Log.Information("Deleting order with GUID: {Id}", guid);
            var order = _orderService.GetByGUID(guid);
            if (order == null)
                return NotFound(new { message = "Order not found" });
            _orderService.Delete(guid);
            return Ok(new { message = "Order deleted successfully" });
        }
    }
}

using DemoShop.Core.DataObjects;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
            Log.Information("UserController initialized.");
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _userService.GetAll();
            return Ok(users);
        }

        [HttpGet("{guid}")]
        public IActionResult GetUserById(String guid)
        {
            var user = _userService.GetByGUID(guid);
            if (user == null)
                return NotFound(new { message = "User not found" });
            return Ok(user);
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] User user)
        {
            _userService.Add(user);
            return Ok(user);
        }

        [HttpPut("{guid}")]
        public IActionResult UpdateUser(String guid, [FromBody] User user)
        {
            _userService.Update(guid, user);
            return Ok(new { message = "User updated successfully" });
        }

        [HttpDelete("{guid}")]
        public IActionResult DeleteUser(String guid)
        {
            _userService.Delete(guid);
            return Ok(new { message = "User deleted successfully" });
        }

    }
}

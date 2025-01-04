using DemoShop.Core.DataObjects;
using DemoShop.Core.DTOs.Account;
using DemoShop.Manager.DBContext;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers.Account
{
    [ApiController]
    [Route("api/v1/account/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly IAuthenticateUserService _userService;
        public RegisterController(IAuthenticateUserService userService)
        {
            _userService = userService;
            Log.Information("RegisterController initialized.");
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            Log.Information("Registering new user: {Username}", request.Username);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.Password, //ToDo password hashing
                Role = "User",
                Guid = Guid.NewGuid().ToString(), //ToDo optimize
            };

            _userService.Register(user);

            return Ok(new { message = "Registration successful" });
        }
    }
}

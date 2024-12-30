using DemoShop.Core.DataObjects.Account;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers.Account
{
    [ApiController]
    [Route("api/v1/account/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthenticateUserService _userService;

        public LoginController(IAuthenticateUserService userService)
        {
            _userService = userService;
            Log.Information("LoginController initialized.");
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            Log.Information("Login attempt for user: {Username}", request.Username);
            var token = _userService.Authenticate(request.Username, request.Password);

            if (token == null)
            {
                Log.Warning("Invalid login attempt for user: {Username}", request.Username);
                return Unauthorized(new { message = "Invalid credentials" });
            }

            Log.Information("Login successful for user: {Username}", request.Username);
            return Ok(new { token });
        }

    }
}

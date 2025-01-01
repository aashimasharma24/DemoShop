using DemoShop.Core.DataObjects;
using DemoShop.Manager.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace DemoShop.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ResetPasswordController : ControllerBase
    {
        private readonly IAuthenticateUserService _authenticateUserService;

        public ResetPasswordController(IAuthenticateUserService authenticateUserService)
        {
            _authenticateUserService = authenticateUserService;
        }

        [HttpPost("request-reset")]
        public IActionResult RequestPasswordReset([FromBody] string email)
        {
            try
            {
                _authenticateUserService.RequestPasswordReset(email);
                Log.Information("Password reset requested for {Email}", email);
                return Ok(new { message = "Password reset link sent to your email." });
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning("Failed password reset request: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error during password reset request: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while requesting password reset." });
            }
        }

        [HttpPost("reset")]
        public IActionResult ResetPassword([FromBody] ResetPassword resetPassword)
        {
            try
            {
                _authenticateUserService.ResetPassword(resetPassword.Token, resetPassword.NewPassword);
                Log.Information("Password reset successfully for token {Token}", resetPassword.Token);
                return Ok(new { message = "Password reset successful." });
            }
            catch (InvalidOperationException ex)
            {
                Log.Warning("Invalid password reset attempt: {Error}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                Log.Error("Unexpected error during password reset: {Exception}", ex);
                return StatusCode(500, new { message = "An error occurred while resetting password." });
            }
        }
    }
}

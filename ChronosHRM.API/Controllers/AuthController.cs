using Chronos.Application.DTOs.Auth;
using Chronos.Application.Interfaces;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        // 1. API Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto request)
        {
            try
            {
                var result = await authService.RegisterAsync(request);
                return Ok(new { message = "Đăng ký thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 2. API Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            try
            {
                var token = await authService.LoginAsync(request);
                // Trả về Token cho Client
                return Ok(new { token = token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenDto request)
        {
            var result = await authService.RefreshTokenAsync(request);

            if (!result.Success)
            {
                return BadRequest(result); // Token sai hoặc hết hạn hẳn -> Bắt đăng nhập lại
            }

            return Ok(result);
        }
    }
}
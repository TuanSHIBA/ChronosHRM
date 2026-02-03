using System.Security.Claims;
using Chronos.Application.IServices;
using Chronos.Application.Services; // Import Service
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chronos.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MenusController(IMenuService menuService) : ControllerBase
{
    [HttpGet("my-menus")]
    public async Task<IActionResult> GetMyMenus()
    {
        // 1. Việc duy nhất của Controller: Lấy ID từ Token
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        // 2. Gọi Service làm việc
        var result = await menuService.GetMenusForUserAsync(userId);

        // 3. Trả hàng
        return Ok(result);
    }
}
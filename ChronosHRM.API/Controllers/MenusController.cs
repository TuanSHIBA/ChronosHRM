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
    [HttpGet]
    public async Task<IActionResult> GetMyMenus()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return Unauthorized();

        var result = await menuService.GetMenusForUserAsync(userId);

        return Ok(result);
    }
}
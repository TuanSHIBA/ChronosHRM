using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using Chronos.Application.DTOs.Auth;
using Chronos.Domain.Entity.Identity;

namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UsersController(UserManager<ApplicationUser> _userManager) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    Id = user.Id.ToString(),
                    FullName = user.FullName,
                    Username = user.UserName!,
                    Roles = roles.ToList()
                });
            }

            return Ok(userDtos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null) return BadRequest("Username đã tồn tại");

            var newUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(newUser, "Employee");

            return Ok("Tạo user thành công");
        }


        [HttpPut("{id}/roles")]
        public async Task<IActionResult> AssignRoles(Guid id, [FromBody] List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User không tồn tại");

            // Lấy các role hiện tại
            var currentRoles = await _userManager.GetRolesAsync(user);

            // Xóa role cũ, thêm role mới
            var removeResult = await _userManager.RemoveFromRolesAsync(user, currentRoles);
            if (!removeResult.Succeeded) return BadRequest("Lỗi khi xóa role cũ");

            var addResult = await _userManager.AddToRolesAsync(user, roles);
            if (!addResult.Succeeded) return BadRequest("Lỗi khi thêm role mới");

            return Ok("Cập nhật quyền thành công");
        }

        [HttpGet("{id}/permissions")]
        public async Task<IActionResult> GetUserPermissions(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User không tồn tại");
            var userClaims = await _userManager.GetClaimsAsync(user);

            return Ok(userClaims.Select(c => c.Value).ToList());
        }


        [HttpPut("{id}/permissions")]
        public async Task<IActionResult> UpdateUserPermissions(Guid id, [FromBody] List<string> permissionValues)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound("User không tồn tại");

            //lấy tất cả claim hiện tại của User
            var currentClaims = await _userManager.GetClaimsAsync(user);

            var claimsToRemove = currentClaims.Where(c => c.Type == "Permission").ToList();

            if (claimsToRemove.Any())
            {
                var removeResult = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
                if (!removeResult.Succeeded) return BadRequest("Lỗi khi xóa quyền cũ");
            }

            var newClaims = permissionValues.Select(value => new System.Security.Claims.Claim("Permission", value)).ToList();

            if (newClaims.Any())
            {
                var addResult = await _userManager.AddClaimsAsync(user, newClaims);
                if (!addResult.Succeeded) return BadRequest("Lỗi khi thêm quyền mới");
            }

            return Ok("Cập nhật quyền riêng cho User thành công");
        }
    }
}
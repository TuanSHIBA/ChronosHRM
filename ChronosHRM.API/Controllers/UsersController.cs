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
    [Authorize(Roles = "Admin")] // Chỉ Admin to nhất mới được vào đây
    public class UsersController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. Lấy danh sách User kèm Role
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

        // 2. Tạo User mới (Admin tạo cho nhân viên)
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto model)
        {
            var existingUser = await _userManager.FindByNameAsync(model.Username);
            if (existingUser != null) return BadRequest("Username đã tồn tại");

            var newUser = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email,
                // Link với bảng Employee nếu cần
            };

            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded) return BadRequest(result.Errors);

            // Mặc định gán role Employee
            await _userManager.AddToRoleAsync(newUser, "Employee");

            return Ok("Tạo user thành công");
        }

        // 3. Gán Role cho User (Quan trọng nhất!)
        // PUT: api/users/{id}/roles
        [HttpPut("{userId}/roles")]
        public async Task<IActionResult> AssignRoles(Guid userId, [FromBody] List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
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

        [HttpGet("{userId}/permissions")]
        public async Task<IActionResult> GetUserPermissions(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User không tồn tại");

            // Lấy các claim được gán trực tiếp cho User (không bao gồm claim từ Role)
            var userClaims = await _userManager.GetClaimsAsync(user);

            // Chỉ lấy Value (ví dụ: "Employee.Create", "Leave.Approve")
            // Giả sử Claim Type bạn quy định là "Permission"
            // Nếu bạn dùng claim type khác thì filter ở đây
            return Ok(userClaims.Select(c => c.Value).ToList());
        }

        // 5. Cập nhật Quyền riêng cho User (Gán quyền ngoại lệ)
        // PUT: api/users/{userId}/permissions
        [HttpPut("{userId}/permissions")]
        public async Task<IActionResult> UpdateUserPermissions(Guid userId, [FromBody] List<string> permissionValues)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound("User không tồn tại");

            // 1. Lấy tất cả claim hiện tại của User
            var currentClaims = await _userManager.GetClaimsAsync(user);

            // 2. Xóa sạch các claim cũ (chỉ xóa claim loại "Permission" để tránh xóa nhầm các claim khác như email, avatar...)
            // Lưu ý: Nếu hệ thống bạn dùng ClaimType là "Permission" cho mọi quyền hạn
            var claimsToRemove = currentClaims.Where(c => c.Type == "Permission").ToList();
            // Hoặc nếu bạn muốn reset hết thì xóa hết (cẩn thận):
            // var claimsToRemove = currentClaims; 

            if (claimsToRemove.Any())
            {
                var removeResult = await _userManager.RemoveClaimsAsync(user, claimsToRemove);
                if (!removeResult.Succeeded) return BadRequest("Lỗi khi xóa quyền cũ");
            }

            // 3. Thêm các claim mới
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
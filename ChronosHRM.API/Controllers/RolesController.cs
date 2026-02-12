using System.Security.Claims;
using Chronos.API.Attributes;
using Chronos.Domain.Constants;
using Chronos.Domain.Entity.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace Chronos.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [HasPermission(Permissions.System.Manage)]
    public class RolesController : ControllerBase
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        
        [HttpGet]
        public IActionResult GetRoles()
        {
            var roles = _roleManager.Roles.ToList();
            return Ok(roles);
        }

 
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            if (await _roleManager.RoleExistsAsync(roleName))
                return BadRequest("Role đã tồn tại");

            await _roleManager.CreateAsync(new ApplicationRole { Name = roleName });
            return Ok("Tạo role thành công");
        }


        [HttpGet("{roleId}/permissions")]
        public async Task<IActionResult> GetPermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();

            var claims = await _roleManager.GetClaimsAsync(role);
            return Ok(claims.Select(c => c.Value)); 
        }

        [HttpPut("{roleId}/permissions")]
        public async Task<IActionResult> UpdatePermissions(string roleId, [FromBody] List<string> permissionValues)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null) return NotFound();


            var currentClaims = await _roleManager.GetClaimsAsync(role);
            foreach (var claim in currentClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }


            foreach (var permission in permissionValues)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", permission));
            }

            return Ok("Cập nhật quyền hạn cho Role thành công");
        }
        [HttpGet("system-permissions")]
        public IActionResult GetSystemPermissions()
        {
          
            var permissions = Chronos.Domain.Constants.Permissions.GetAllPermissions();
            return Ok(permissions);
        }
    }
}
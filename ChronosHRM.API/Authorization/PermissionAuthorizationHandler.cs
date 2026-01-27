using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;
using Chronos.Domain.Entity.Identity;

namespace Chronos.API.Authorization
{
    public class PermissionAuthorizationHandler(
        IServiceScopeFactory serviceScopeFactory,
        IMemoryCache cache
        ) : AuthorizationHandler<PermissionRequirement>
    {
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            // 1. Lấy UserId từ Token
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return;

            // 2. Định nghĩa Key Cache 
            var cacheKey = $"Auth_Permissions_{userId}";

            // 3. Kiểm tra Cache xem có danh sách quyền chưa
            if (!cache.TryGetValue(cacheKey, out Dictionary<string, bool>? permissionMap))
            {
                // Nếu chưa có cache -> Tạo Scope mới để gọi Database (Vì Handler là Singleton)
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

                    var user = await userManager.FindByIdAsync(userId);
                    if (user == null) return;

                    permissionMap = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                    var roles = await userManager.GetRolesAsync(user);
                    foreach (var roleName in roles)
                    {
                        var role = await roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var roleClaims = await roleManager.GetClaimsAsync(role);
                            foreach (var claim in roleClaims)
                            {
                                    if (claim.Type == "Permission")
                                    {
                                        permissionMap[claim.Value] = true;
                                    }
                            }
                        }
                    }

                    var userClaims = await userManager.GetClaimsAsync(user);
                    foreach (var claim in userClaims)
                    {
                        if (bool.TryParse(claim.Value, out bool isAllow))
                        {
                            permissionMap[claim.Type] = isAllow;
                        }
                    }

                    cache.Set(cacheKey, permissionMap, TimeSpan.FromMinutes(30));
                }
            }

            if (permissionMap != null&& permissionMap.TryGetValue(requirement.Permission, out bool allowAccess)&& allowAccess)
            {
                context.Succeed(requirement);
            }
        }
    }
}
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Chronos.Domain.Entities; // Đảm bảo using đúng namespace chứa User entity của bạn
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

            // 2. Định nghĩa Key Cache (Mỗi user có 1 vùng nhớ riêng)
            var cacheKey = $"Auth_Permissions_{userId}";

            // 3. Kiểm tra Cache xem có danh sách quyền chưa
            if (!cache.TryGetValue(cacheKey, out Dictionary<string, bool>? permissionMap))
            {
                // Nếu chưa có cache -> Tạo Scope mới để gọi Database (Vì Handler thường là Singleton)
                using (var scope = serviceScopeFactory.CreateScope())
                {
                    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                    var user = await userManager.FindByIdAsync(userId);
                    if (user == null) return;

                    permissionMap = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

                    // --- BƯỚC A: LẤY QUYỀN TỪ ROLE (NỀN TẢNG) ---
                    var roles = await userManager.GetRolesAsync(user);
                    foreach (var roleName in roles)
                    {
                        var role = await roleManager.FindByNameAsync(roleName);
                        if (role != null)
                        {
                            var roleClaims = await roleManager.GetClaimsAsync(role);
                            foreach (var claim in roleClaims)
                            {
                                // Nếu ClaimValue là "true" -> Cho phép
                                if (claim.Type == "Permission" && bool.TryParse(claim.Value, out bool isAllow)) // Sửa lại check Type
                                {
                                    // Lưu ý: Trong DB mình lưu Type="Permission", Value="Employees.View"
                                    // Nhưng logic override mình cần Key="Employees.View", Value=true/false
                                    // Sửa lại logic một chút cho khớp với Seeder ở bước sau:

                                    // Logic chuẩn:
                                    // Trong DB bảng RoleClaims: ClaimType="Permission", ClaimValue="Permissions.Employees.View"
                                    // Ta mặc định Role có claim này là TRUE
                                    if (claim.Type == "Permission")
                                    {
                                        permissionMap[claim.Value] = true;
                                    }
                                }
                            }
                        }
                    }

                    // --- BƯỚC B: LẤY QUYỀN TỪ USER (NGOẠI LỆ) -> GHI ĐÈ ---
                    // Ví dụ bảng UserClaims: ClaimType="Permissions.Employees.View", ClaimValue="false"
                    var userClaims = await userManager.GetClaimsAsync(user);
                    foreach (var claim in userClaims)
                    {
                        // Logic ghi đè: Key là tên quyền, Value là true/false
                        if (bool.TryParse(claim.Value, out bool isAllow))
                        {
                            permissionMap[claim.Type] = isAllow;
                        }
                    }

                    // C. Lưu vào Cache (30 phút)
                    cache.Set(cacheKey, permissionMap, TimeSpan.FromMinutes(30));
                }
            }

            // 4. Kiểm tra quyền cuối cùng
            // requirement.Permission ví dụ là "Permissions.Employees.View"
            if (permissionMap != null
                && permissionMap.TryGetValue(requirement.Permission, out bool allowAccess)
                && allowAccess)
            {
                context.Succeed(requirement);
            }
        }
    }
}
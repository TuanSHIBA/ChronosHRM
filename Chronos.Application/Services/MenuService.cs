using Chronos.Application.IServices;
using Chronos.Domain.Entity.Identity;
using Chronos.Domain.Entity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Chronos.Domain.Interfaces;
using Chronos.Application.DTOs.Menu;

namespace Chronos.Application.Services
{
    public class MenuService(
     IUnitOfWork unitOfWork, // 👈 Thay DbContext bằng UnitOfWork
     UserManager<ApplicationUser> userManager,
     RoleManager<ApplicationRole> roleManager
     ) : IMenuService
    {
        public async Task<List<MenuDto>> GetMenusForUserAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user is null) return [];

            // --- LOGIC GOM CHÌA KHÓA (Copy từ Controller sang) ---
            var userPermissions = new HashSet<string>();

            // A. Từ Role
            var userRoles = await userManager.GetRolesAsync(user);
            foreach (var roleName in userRoles)
            {
                var role = await roleManager.FindByNameAsync(roleName);
                if (role is not null)
                {
                    var roleClaims = await roleManager.GetClaimsAsync(role);
                    foreach (var claim in roleClaims)
                    {
                        if (claim.Type == "Permission") userPermissions.Add(claim.Value);
                    }
                }
            }

            // B. Từ User Claims
            var userClaims = await userManager.GetClaimsAsync(user);
            foreach (var claim in userClaims)
            {
                if (claim.Type == "Permission") userPermissions.Add(claim.Value);
            }

            // --- LOGIC LỌC MENU ---
            var allMenus = await unitOfWork.Menus.GetAllAsync();

            var visibleMenus = allMenus.Where(menu =>
            {
                if (string.IsNullOrEmpty(menu.RequiredPermission)) return true;
                return userPermissions.Contains(menu.RequiredPermission);
            }).ToList();

            // Xếp cây thư mục
            return BuildMenuTree(visibleMenus, null);
        }

        // Hàm private để xếp cây
        private static List<MenuDto> BuildMenuTree(List<AppMenu> source, int? parentId)
        {
            return source
                .Where(m => m.ParentId == parentId)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Title = m.Title,
                    Path = m.Path,
                    Icon = m.Icon,
                    Children = BuildMenuTree(source, m.Id)
                })
                .ToList();
        }
    }

}

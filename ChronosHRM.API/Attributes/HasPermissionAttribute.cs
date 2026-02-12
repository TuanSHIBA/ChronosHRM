using Microsoft.AspNetCore.Authorization;

namespace Chronos.API.Attributes
{
    public class HasPermissionAttribute : AuthorizeAttribute
    {
        // Cách dùng: [HasPermission("Permissions.Employees", "View")]
        // Hoặc dùng constant: [HasPermission(Permissions.Employees.View)]
        public HasPermissionAttribute(string permission)
        {
            Policy = permission;
        }
        public HasPermissionAttribute(string resource, string action)
        {
            Policy = $"Permissions.{resource}.{action}";
        }
    }
}
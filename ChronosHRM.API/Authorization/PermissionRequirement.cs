using Microsoft.AspNetCore.Authorization;

namespace Chronos.API.Authorization
{
    public class PermissionRequirement(string permission) : IAuthorizationRequirement
    {
        public string Permission { get; } = permission;
    }
}
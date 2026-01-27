using Microsoft.AspNetCore.Authorization;

using Microsoft.Extensions.Options;

namespace Chronos.API.Authorization

{
    public class PermissionPolicyProvider(IOptions<AuthorizationOptions> options) : DefaultAuthorizationPolicyProvider(options)
    {
        public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            // Kiểm tra xem policy đã tồn tại chưa (Ví dụ policy mặc định "AdminOnly")
            var policy = await base.GetPolicyAsync(policyName);
            if (policy == null)
            {
                // Nếu chưa có, tự động tạo Policy mới với Requirement là tên quyền
                policy = new AuthorizationPolicyBuilder().AddRequirements(new PermissionRequirement(policyName)).Build();
            }
            return policy;

        }

    }

}
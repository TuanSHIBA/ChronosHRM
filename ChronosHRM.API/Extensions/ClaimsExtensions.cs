using System.Security.Claims;

namespace Chronos.API.Extensions 
{
    public static class ClaimsExtensions
    {
        public static Guid GetEmployeeId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("EmployeeId");

            if (claim == null)
            {
                throw new UnauthorizedAccessException("Token không chứa thông tin Nhân viên (EmployeeId).");
            }

            return Guid.Parse(claim.Value);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Auth
{

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public Guid? DepartmentId { get; set; } // Nếu muốn link với nhân viên luôn
    }
    public class UserDto
    {
        public string? Id { get; set; }
        public required string Username { get; set; }
        public string? FullName { get; set; }
        public List<string>? Roles { get; set; }
        public List<Claim>? Claims { get; set; }
    }
}

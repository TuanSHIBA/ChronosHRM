using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Auth
{
    public class TokenDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
    public class UserDto
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public List<string> Roles { get; set; }
        public List<Claim> Claims { get; set; }
    }
}
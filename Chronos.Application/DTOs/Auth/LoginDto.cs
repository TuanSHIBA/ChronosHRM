using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Auth
{
    public class LoginDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
    public class LoginResponseDto
    {
        public TokenDto Token { get; set; }
        
        public UserDto User { get; set; } // <--- Thêm cái này
    }
}

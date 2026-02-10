using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IAuthService
    {
        Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginDto request);
        Task<ServiceResponse<string>> RegisterAsync(RegisterDto request);
        Task<ServiceResponse<TokenDto>> RefreshTokenAsync(TokenDto request);
    }
}

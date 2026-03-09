using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.Common.Settings;
using Chronos.Application.DTOs.Auth;
using Chronos.Application.IServices;
using Chronos.Domain.Entity.Identity;
using Chronos.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Chronos.Application.Services
{
    public class AuthService(UserManager<ApplicationUser> _userManager,
            SignInManager<ApplicationUser> _signInManager,
            JwtSettings _jwtSettings, IUnitOfWork _unitOfWork) : IAuthService
    {
   
        public async Task<ServiceResponse<LoginResponseDto>> LoginAsync(LoginDto request)
        {
            // 1. Kiểm tra User & Password (Giữ nguyên)
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return ServiceResponse<LoginResponseDto>.ErrorResponse("Tài khoản không tồn tại.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return ServiceResponse<LoginResponseDto>.ErrorResponse("Mật khẩu hoặc tài khoản không đúng.");

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);


            var accessToken = await GenerateAccessTokenAsync(user, userRoles, userClaims);
            var refreshToken = GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.DurationInMinutes);
            await _userManager.UpdateAsync(user);

            // 5. Tạo UserDto (Dùng lại biến userRoles và userClaims)
            var userDto = new UserDto
            {
                Username = user.UserName!,
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Roles = userRoles.ToList(),
                Claims = userClaims.ToList()
            };

            // 6. Trả về kết quả
            return ServiceResponse<LoginResponseDto>.SuccessResponse(new LoginResponseDto
            {
                Token = new TokenDto
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                },
                User = userDto
            }, "Đăng nhập thành công!");
        }

        // ==================== 2. REGISTER (Đã làm sạch) ====================
        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto request)
        {
            // 1. Check trùng
            var userExists = await _userManager.FindByEmailAsync(request.Email!);
            if (userExists != null)
                return ServiceResponse<string>.ErrorResponse("Email này đã được sử dụng.");

            // 2. Tạo User Object
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName!,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // 3. Lưu xuống DB (Chỉ tạo user, KHÔNG gán quyền)
            var result = await _userManager.CreateAsync(user, request.Password!);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResponse<string>.ErrorResponse("Đăng ký thất bại");
            }

            // Trả về ID
            return ServiceResponse<string>.SuccessResponse(user.Id.ToString(), "Đăng ký thành công!");
        }

        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user, IList<string> roles, IList<Claim> claims)
        {
            // 1. Tạo các Claim cơ bản
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("fullName", user.FullName ?? "")
            };
            var employee = (await _unitOfWork.Employees.GetByAppUserIdAsync(user.Id));
            if (employee != null)
            {
                claims.Add(new Claim("EmployeeId", employee.Id.ToString()));
            }
            authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            authClaims.AddRange(claims);
            
            // 3. Ký Token (Giữ nguyên logic cũ của bạn)
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<ServiceResponse<TokenDto>> RefreshTokenAsync(TokenDto request)
        {
            var accessToken = request.AccessToken;
            var refreshToken = request.RefreshToken;

            // 1. Trích xuất thông tin từ Token hết hạn (Giữ nguyên)
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
                return ServiceResponse<TokenDto>.ErrorResponse("Invalid Access Token.");

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier); // Hoặc JwtRegisteredClaimNames.Sub
            if (userId == null)
                return ServiceResponse<TokenDto>.ErrorResponse("Token không chứa User ID.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ServiceResponse<TokenDto>.ErrorResponse("Invalid Refresh Token.");
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var userClaims = await _userManager.GetClaimsAsync(user);

            // 3. Sinh Access Token mới (Truyền roles và claims vào hàm GenerateAccessToken mới)
            var newAccessToken = await GenerateAccessTokenAsync(user, userRoles, userClaims);

            // 4. Sinh Refresh Token mới
            var newRefreshToken = GenerateRefreshToken();

            // 5. Cập nhật xuống DB
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.DurationInMinutes); // Nhớ update cả hạn dùng
            await _userManager.UpdateAsync(user);

            return ServiceResponse<TokenDto>.SuccessResponse(new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }, "Làm mới Token thành công!");
        }


        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
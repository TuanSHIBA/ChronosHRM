using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Chronos.Application.Common.Settings;
using Chronos.Application.DTOs.Auth;
using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.Interfaces.IServices;
using Chronos.Domain.Entity.Identity;

namespace Chronos.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtSettings jwtSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
        }

        // ==================== 1. LOGIN (Giữ nguyên logic Refresh Token) ====================
        public async Task<ServiceResponse<TokenDto>> LoginAsync(LoginDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null)
                return ServiceResponse<TokenDto>.ErrorResponse("Tài khoản không tồn tại.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
                return ServiceResponse<TokenDto>.ErrorResponse("Mật khẩu hoặc tài khoản không đúng.");

            // Sinh Token (Lúc này Token sẽ chỉ chứa thông tin cơ bản, chưa có Role nếu Admin chưa cấp)
            var accessToken = await GenerateAccessTokenAsync(user);
            var refreshToken = GenerateRefreshToken();

            // Update Refresh Token
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.DurationInMinutes);
            await _userManager.UpdateAsync(user);

            return ServiceResponse<TokenDto>.SuccessResponse(new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            }, "Đăng nhập thành công!");
        }

        // ==================== 2. REGISTER (Đã làm sạch) ====================
        public async Task<ServiceResponse<string>> RegisterAsync(RegisterDto request)
        {
            // 1. Check trùng
            var userExists = await _userManager.FindByEmailAsync(request.Email);
            if (userExists != null)
                return ServiceResponse<string>.ErrorResponse("Email này đã được sử dụng.");

            // 2. Tạo User Object
            var user = new ApplicationUser
            {
                UserName = request.Username,
                Email = request.Email,
                FullName = request.FullName,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            // 3. Lưu xuống DB (Chỉ tạo user, KHÔNG gán quyền)
            var result = await _userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return ServiceResponse<string>.ErrorResponse("Đăng ký thất bại");
            }

            // Trả về ID
            return ServiceResponse<string>.SuccessResponse(user.Id.ToString(), "Đăng ký thành công!");
        }

        // ==================== HELPER METHODS ====================

        private async Task<string> GenerateAccessTokenAsync(ApplicationUser user)
        {
            // Lấy Role từ DB (Nếu user mới tạo thì list này sẽ rỗng -> Token sạch)
            var userRoles = await _userManager.GetRolesAsync(user);

            // Lấy Claims từ DB (Nếu chưa gán thì list rỗng)
            var userClaims = await _userManager.GetClaimsAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("fullName", user.FullName)
            };

            // Tự động nhét Role và Claim vào Token NẾU CÓ
            authClaims.AddRange(userRoles.Select(role => new Claim(ClaimTypes.Role, role)));
            authClaims.AddRange(userClaims);

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

            // 1. Trích xuất thông tin User từ Access Token đã hết hạn
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return ServiceResponse<TokenDto>.ErrorResponse("Invalid Access Token (Token không hợp lệ).");
            }

            // Lấy UserName từ trong Token cũ

            var userId =  principal.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return ServiceResponse<TokenDto>.ErrorResponse("Token không chứa thông tin User ID.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return ServiceResponse<TokenDto>.ErrorResponse("Invalid Refresh Token (Token làm mới không hợp lệ hoặc đã hết hạn).");
            }

            // 2. Nếu mọi thứ OK -> Sinh cặp token mới
            var newAccessToken = await GenerateAccessTokenAsync(user);
            var newRefreshToken = GenerateRefreshToken();

            // 3. Cập nhật xuống DB
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            return ServiceResponse<TokenDto>.SuccessResponse(new TokenDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            }, "Làm mới Token thành công!");
        }

        // ==================== HELPER: ĐỌC TOKEN HẾT HẠN ====================
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
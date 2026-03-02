using Chronos.API.Authorization;
using Chronos.Application.Common.Settings;
using Chronos.Application.DTOs.Employee;
using Chronos.Application.IServices;
using Chronos.Application.Services;
using Chronos.Domain.Entity.Identity;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using Chronos.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json; 

namespace ChronosHRM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddMemoryCache();
            // 1. DB Context
            builder.Services.AddDbContext<ChronosDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // 3. DI Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmploymentContractService, EmploymentContractService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<ILeaveTypeService, LeaveTypeService>();
            builder.Services.AddScoped<ILeaveRequestService, LeaveRequestService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();
            builder.Services.AddScoped<IPositionService, PositionService>();
            builder.Services.AddScoped<IMenuService, MenuService>();
            builder.Services.AddScoped<IEmployeeTransferService, EmployeeTransferService>();
            // tài liệu ASP.NET Core Custom Authorization Policy Providers
            builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    document.Info = new OpenApiInfo
                    {
                        Title = "Chronos HRM API",
                        Version = "v1",
                        Description = "API hệ thống quản lý nhân sự"
                    };

                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes = new Dictionary<string, OpenApiSecurityScheme>
                    {
                        ["Bearer"] = new OpenApiSecurityScheme
                        {
                            Type = SecuritySchemeType.Http,
                            Scheme = "bearer",
                            BearerFormat = "JWT",
                            In = ParameterLocation.Header,
                            Description = "Nhập JWT Token vào đây (Không cần chữ Bearer)"
                        }
                    };

                    // 2. Yêu cầu bảo mật toàn cục (Áp dụng cho tất cả API)
                    document.SecurityRequirements = new List<OpenApiSecurityRequirement>
        {
            new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            }
        };

                    return Task.CompletedTask;
                });
            });

            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(typeof(IEmployeeService).Assembly);

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.AddSingleton(jwtSettings!); 

            // 2. Cấu hình Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false; 
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6; 
            })
            .AddEntityFrameworkStores<ChronosDbContext>()
            .AddDefaultTokenProviders();

            // 3. Cấu hình xác thực JWT (Authentication)
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings?.Issuer,
                    ValidAudience = jwtSettings?.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Key)),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    // Khi Authentication thất bại (Hết hạn, sai token...)
                    OnChallenge = context =>
                    {
                        // Bỏ qua behavior mặc định
                        context.HandleResponse();

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";

                        // Kiểm tra xem lỗi cụ thể là gì
                        var message = "Bạn chưa đăng nhập hoặc Token không hợp lệ.";

                        if (context.AuthenticateFailure != null &&
                            context.AuthenticateFailure.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            message = "Token đã hết hạn. Vui lòng Refresh Token.";
                        }

                        // Trả về JSON tùy chỉnh
                        var result = JsonSerializer.Serialize(new
                        {
                            message = message,
                            statusCode = 401
                        });

                        return context.Response.WriteAsync(result);
                    }
                };
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

      
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Tạo ra file JSON tại /openapi/v1.json
                app.MapScalarApiReference(); // Tạo giao diện Web tại /scalar/v1
            }
         
            app.UseHttpsRedirection();
            app.UseCors("AllowAllOrigins");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
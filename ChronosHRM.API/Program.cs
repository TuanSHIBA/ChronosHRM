using Chronos.Application.DTOs.Employee;
using Chronos.Application.Interfaces;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Mappings;
using Chronos.Application.Services;
using Chronos.Domain.Entity.Identity;
using Chronos.Persistence.Context;
using Chronos.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text; // 👈 Nhớ using cái này

namespace ChronosHRM.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // 1. DB Context
            builder.Services.AddDbContext<ChronosDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 2. AutoMapper
            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // 3. DI Services
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddControllers();

            // ==========================================
            // 👇 CHUẨN .NET 9: Dùng Native OpenAPI
            // ==========================================
            builder.Services.AddOpenApi();

            // 4. FluentValidation
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddValidatorsFromAssembly(typeof(IEmployeeService).Assembly);

            var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
            builder.Services.AddSingleton(jwtSettings); // Đăng ký để tiêm vào Service sau này

            // 2. Cấu hình Identity (User/Role/Pass)
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false; // Demo cho dễ, thực tế nên để true
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6; // Pass tối thiểu 6 ký tự
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

                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
                };
            });




            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Tạo ra file JSON tại /openapi/v1.json
                app.MapScalarApiReference(); // Tạo giao diện Web tại /scalar/v1
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
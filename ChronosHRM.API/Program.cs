using Chronos.Application.DTOs.Employee;
using Chronos.Application.Interfaces;
using Chronos.Application.Interfaces.IServices;
using Chronos.Application.Mappings;
using Chronos.Application.Services;
using Chronos.Persistence.Context;
using Chronos.Persistence.Repositories;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore; // 👈 Nhớ using cái này

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
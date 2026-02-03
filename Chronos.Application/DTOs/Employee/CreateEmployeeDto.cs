using Chronos.Domain.Enums;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Employee
{
    public class CreateEmployeeDto
    {
        // --- 1. THÔNG TIN CƠ BẢN ---
        public required string EmployeeCode { get; set; } // NV001
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; } // Email công ty
        public string? PersonalEmail { get; set; }     // Email cá nhân
        public required string PhoneNumber { get; set; }
        public string? AvatarUrl { get; set; }

        // --- 2. THÔNG TIN CÁ NHÂN ---
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; } = Gender.Male; // Enum (0, 1, 2)
        public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single; // Enum

        public string? PlaceOfBirth { get; set; }
        public string? Hometown { get; set; }
        public string? Ethnicity { get; set; } // Dân tộc
        public string? Religion { get; set; }  // Tôn giáo
        public string? Nationality { get; set; } = "Việt Nam";

        public required string Address { get; set; }       // Thường trú
        public string? CurrentAddress { get; set; }        // Tạm trú

        // --- 3. THÔNG TIN PHÁP LÝ (CMND/Thuế) ---
        public string? IdentityCardNumber { get; set; }
        public DateTime? IdentityCardDate { get; set; }
        public string? IdentityCardPlace { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }

        // --- 4. NGÂN HÀNG (Trả lương) ---
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }

        // --- 5. CÔNG VIỆC ---
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public string? JobTitle { get; set; }
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Probation; // Enum

        // Foreign Keys
        public Guid DepartmentId { get; set; } // Bắt buộc phải thuộc phòng ban nào đó
        public Guid? ManagerId { get; set; }   // Có thể chưa có quản lý
    }

    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.Email).NotEmpty().EmailAddress();

            RuleFor(x => x.PhoneNumber).Matches(@"^\d{10}$").WithMessage("Số điện thoại không hợp lệ (10 số).");

            RuleFor(x => x.DateOfBirth).Must(BeAtLeast18YearsOld).WithMessage("Nhân viên phải đủ 18 tuổi.");

            RuleFor(x => x.DepartmentId).NotEmpty().WithMessage("Vui lòng chọn phòng ban.");
        }

        // Hàm phụ trợ để tính tuổi
        private bool BeAtLeast18YearsOld(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            // Nếu chưa đến sinh nhật năm nay thì trừ đi 1 tuổi
            if (dateOfBirth.Date > today.AddYears(-age)) age--;

            return age >= 18;
        }
    }
}

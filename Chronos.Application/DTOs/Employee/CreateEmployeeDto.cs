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
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string? Address { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
        public int Status { get; set; }
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

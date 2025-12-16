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
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; }
        public Guid DepartmentId { get; set; }
        public Guid? ManagerId { get; set; }
    }

    public class CreateEmployeeValidator : AbstractValidator<CreateEmployeeDto>
    {
        public CreateEmployeeValidator()
        {
            // Check Mã NV
            RuleFor(x => x.EmployeeCode)
                .NotEmpty().WithMessage("Mã nhân viên không được để trống.")
                .MaximumLength(10).WithMessage("Mã nhân viên tối đa 10 ký tự.");

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

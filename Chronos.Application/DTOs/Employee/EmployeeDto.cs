using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Employee
{
    public class EmployeeDto
    {
        public Guid Id { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty; // Đã gộp First + Last
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        // Liên hệ
        public string WorkEmail { get; set; } = string.Empty;
        public string? PersonalEmail { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? CurrentAddress { get; set; }

        // Cá nhân
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; }               // Frontend sẽ tự map 0->Nam, 1->Nữ
        public MaritalStatus MaritalStatus { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Hometown { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }

        // Pháp lý & Bank
        public string? IdentityCardNumber { get; set; }
        public DateTime? IdentityCardDate { get; set; }
        public string? IdentityCardPlace { get; set; }
        public string? TaxCode { get; set; }
        public string? SocialInsuranceNumber { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }

        // Công việc
        public DateTime JoinDate { get; set; }
        public Guid? PositionId { get; set; } // Cho phép null (lúc mới tạo chưa gán)
        public Position? Position { get; set; } // Link sang bảng Position
        public EmployeeStatus Status { get; set; }

        // Relationship Data (Dữ liệu đã Join bảng)
        public Guid DepartmentId { get; set; }
        public string? DepartmentName { get; set; } // Để hiện tên phòng ban trên bảng

        public Guid? ManagerId { get; set; }
        public string? ManagerName { get; set; }    // Để hiện tên Sếp quản lý (nếu có)
    }
}

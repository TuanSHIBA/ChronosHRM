using Chronos.Domain.Common;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Employee : BaseEntity
    {
        [MaxLength(20)]
        public string EmployeeCode { get; set; } = string.Empty; 

        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public string FullName => $"{LastName} {FirstName}";

        public string? AvatarUrl { get; set; }

        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneNumber { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public Gender Gender { get; set; } = Gender.Male;

        public MaritalStatus MaritalStatus { get; set; } = MaritalStatus.Single;

        [MaxLength(100)]
        public string? PlaceOfBirth { get; set; } // Nơi sinh (Tỉnh/TP)

        [MaxLength(100)]
        public string? Hometown { get; set; } // Nguyên quán (Quê quán)

        [MaxLength(50)]
        public string? Ethnicity { get; set; } // Dân tộc (Kinh, Tày...)

        [MaxLength(50)]
        public string? Religion { get; set; } // Tôn giáo

        [MaxLength(50)]
        public string? Nationality { get; set; } = "Việt Nam"; // Quốc tịch

        [MaxLength(255)]
        public string Address { get; set; } = string.Empty; // Địa chỉ thường trú

        [MaxLength(255)]
        public string? CurrentAddress { get; set; } // Chỗ ở hiện nay (Tạm trú)
        [MaxLength(20)]
        public string? IdentityCardNumber { get; set; } // CMND / CCCD
        public DateTime? IdentityCardDate { get; set; } // Ngày cấp
        [MaxLength(100)]
        public string? IdentityCardPlace { get; set; } // Nơi cấp
        [MaxLength(20)]
        public string? TaxCode { get; set; } // Mã số thuế cá nhân (MST)
        [MaxLength(20)]
        public string? SocialInsuranceNumber { get; set; } // Số sổ bảo hiểm xã hội
        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }
        [MaxLength(100)]
        public string? BankName { get; set; } // VD: Techcombank, Vietcombank
        [MaxLength(100)]
        public string? BankBranch { get; set; } // Chi nhánh
        public DateTime JoinDate { get; set; } = DateTime.Now;
        public EmployeeStatus Status { get; set; } = EmployeeStatus.Probation;
        public Guid? PositionId { get; set; }
        public Position? Position { get; set; } 
        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }
        public Guid? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public Guid? AppUserId { get; set; } // Link tới tài khoản đăng nhập
        public ICollection<EmploymentContract> Contracts { get; set; } = new List<EmploymentContract>();

    }
}

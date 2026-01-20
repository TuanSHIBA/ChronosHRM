using Chronos.Domain.Common;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Employee : BaseEntity
    {
        public string EmployeeCode { get; set; } = string.Empty; // Mã NV: NV001
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => $"{LastName} {FirstName}"; // Computed Property

        public DateTime DateOfBirth { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Probation;

        // Foreign Key: Phòng ban
        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }

        // Self-Referencing: Quản lý trực tiếp (Manager)
        public Guid? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        // Quan hệ 1-N: Một nhân viên có nhiều hợp đồng (Lịch sử lương)
        public ICollection<EmploymentContract> Contracts { get; set; } = new List<EmploymentContract>();
    }
}

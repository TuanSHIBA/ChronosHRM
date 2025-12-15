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
        public string EmployeeCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string DepartmentName { get; set; } // Flatten dữ liệu: Lấy tên phòng ban luôn
        public string Status { get; set; } // Trả về string cho Frontend dễ hiển thị
        public string? AvatarUrl { get; set; }
    }
}

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
        public string DepartmentId { get; set; } 
        public string DepartmentName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public DateTime DateOfBirth { get; set; }
        public string Address { get; set; } 
        public string Status { get; set; } 
        public string? AvatarUrl { get; set; }
    }
}

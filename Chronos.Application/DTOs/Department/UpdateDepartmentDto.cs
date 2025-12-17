using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Department
{
    public class UpdateDepartmentDto
    {
        public Guid Id { get; set; } // ID cần sửa
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}

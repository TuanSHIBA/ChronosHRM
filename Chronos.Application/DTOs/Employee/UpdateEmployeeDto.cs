using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Employee
{
    public class UpdateEmployeeDto : CreateEmployeeDto // Có thể kế thừa từ Create cho nhanh
    {
        public Guid Id { get; set; } // 👈 Bắt buộc phải có cái này
    }
}

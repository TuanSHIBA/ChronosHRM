using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Employee
{
    public class UpdateEmployeeDto : CreateEmployeeDto 
    {
        public Guid Id { get; set; } 
    }
}

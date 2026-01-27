using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmploymentContract
{
    public class UpdateEmploymentContractDto : CreateEmploymentContractDto
    {
        public Guid Id { get; set; } 
    }
}

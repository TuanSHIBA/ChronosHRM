using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmploymentContract
{
    public class CreateContractDto
    {
        public Guid EmployeeId { get; set; }
        public string ContractCode { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
        public SalaryType SalaryType { get; set; }
        public decimal? InsuranceSalary { get; set; }
    }
}

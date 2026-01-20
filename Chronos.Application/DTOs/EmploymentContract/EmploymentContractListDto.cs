using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmploymentContract
{
    public class EmploymentContractListDto
    {
        public Guid Id { get; set; }
        public string? ContractCode { get; set; }

        public Guid EmployeeId { get; set; }
        public string? FullName { get; set; }
        public string?  EmployeeCode { get; set; }

        public ContractType ContractType { get; set; }
        public ContractStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal BaseSalary { get; set; }
    }
}

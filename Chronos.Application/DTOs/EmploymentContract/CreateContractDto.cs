using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmploymentContract
{
    public class CreateEmploymentContractDto
    {
        public Guid EmployeeId { get; set; }
        public string ContractCode { get; set; } = "hahahhaa";
        public DateTime SignDate { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public Guid? PositionId { get; set; }    
        public string? WorkingLocation { get; set; } 

        public decimal BaseSalary { get; set; }
        public decimal InsuranceSalary { get; set; }
        public SalaryType SalaryType { get; set; }
        public ContractType ContractType { get; set; }
        public ContractStatus Status { get; set; }
        public decimal MealAllowance { get; set; } = 0;  
        public decimal TravelAllowance { get; set; } = 0; 
        public decimal OtherAllowance { get; set; } = 0; 

        public string? Note { get; set; }
        public string? AttachmentUrl { get; set; } 
    }
}

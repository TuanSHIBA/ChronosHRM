using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmploymentContract
{
    public class EmploymentContractDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }

        // 1. THÔNG TIN CHUNG
        public string ContractCode { get; set; }
        public ContractType ContractType { get; set; }
        public ContractStatus Status { get; set; }
        public DateTime SignDate { get; set; } // [MỚI]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 2. CHI TIẾT CÔNG VIỆC [MỚI]
        public string? JobTitle { get; set; }
        public string? WorkingLocation { get; set; }

        // 3. THU NHẬP (LƯƠNG + PHỤ CẤP)
        public decimal BaseSalary { get; set; }
        public SalaryType SalaryType { get; set; }    // [BỔ SUNG]
        public decimal InsuranceSalary { get; set; }  // [BỔ SUNG]

        // [MỚI] Nhóm phụ cấp
        public decimal MealAllowance { get; set; }
        public decimal TravelAllowance { get; set; }
        public decimal OtherAllowance { get; set; }

        // 4. KHÁC [MỚI]
        public string? Note { get; set; }
        public string? AttachmentUrl { get; set; }
    }
}

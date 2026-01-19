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
        // 1. LIÊN KẾT
        public Guid EmployeeId { get; set; }

        // 2. PHÁP LÝ & ĐỊNH DANH

        public DateTime SignDate { get; set; } // [MỚI] Ngày ký hợp đồng
        public int ContractType { get; set; }
        public int Status { get; set; }
        public int SalaryType { get; set; }
        // 3. THỜI GIAN
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 4. CÔNG VIỆC [MỚI]
        public string? JobTitle { get; set; }        // Chức danh (VD: Senior Dev)
        public string? WorkingLocation { get; set; } // Địa điểm làm việc

        // 5. LƯƠNG & BHXH
        public decimal BaseSalary { get; set; }
        public decimal? InsuranceSalary { get; set; } // Lương đóng bảo hiểm (Nullable)

        // 6. PHỤ CẤP [MỚI] (Mặc định là 0 nếu không nhập)
        public decimal MealAllowance { get; set; } = 0;   // Ăn trưa
        public decimal TravelAllowance { get; set; } = 0; // Xăng xe
        public decimal OtherAllowance { get; set; } = 0;  // Khác

        // 7. KHÁC [MỚI]
        public string? Note { get; set; }
        public string? AttachmentUrl { get; set; } // Link file PDF
    }
}

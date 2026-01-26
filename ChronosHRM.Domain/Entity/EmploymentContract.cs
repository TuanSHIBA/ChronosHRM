using Chronos.Domain.Common;
using Chronos.Domain.Enums;

namespace Chronos.Domain.Entity
{
    public class EmploymentContract : BaseEntity
    {
        // 1. LIÊN KẾT
        public Guid EmployeeId { get; set; }
        public string ContractCode { get; set; } = string.Empty; // Mã HĐ
        public ContractType ContractType { get; set; }           // Loại
        public ContractStatus Status { get; set; }               // Trạng thái

        // 3. THỜI GIAN
        public DateTime SignDate { get; set; }  // Ngày ký (Mới thêm)
        public DateTime StartDate { get; set; } // Ngày hiệu lực
        public DateTime? EndDate { get; set; }  // Ngày hết hạn

        // 4. LƯƠNG & CHẾ ĐỘ (Dùng số thực tế, không dùng Ngạch/Bậc)
        public decimal BaseSalary { get; set; }      // Lương cứng
        public SalaryType SalaryType { get; set; }   // Gross/Net
        public decimal InsuranceSalary { get; set; } // Lương đóng BHXH

        // 5. PHỤ CẤP CỐ ĐỊNH (3 loại phổ biến nhất)
        public decimal MealAllowance { get; set; } = 0;   // Ăn trưa
        public decimal TravelAllowance { get; set; } = 0; // Xăng xe
        public decimal OtherAllowance { get; set; } = 0;  // Khác (Trách nhiệm, độc hại...)

        // 6. CÔNG VIỆC & LƯU TRỮ
        public string? JobTitle { get; set; }        // Chức danh (VD: Kế toán trưởng)
        public string? WorkingLocation { get; set; } // Địa điểm
        public string? AttachmentUrl { get; set; }   // Link file scan
        public string? Note { get; set; }            // Ghi chú
        public required Employee Employee { get; set; }
    }
}
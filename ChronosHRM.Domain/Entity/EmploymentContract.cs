using Chronos.Domain.Common;
using Chronos.Domain.Entity;
using Chronos.Domain.Enums;

namespace Chronos.Domain.Entities
{
    public class EmploymentContract : BaseEntity
    {
        public string ContractCode { get; set; } = string.Empty;
        public ContractType ContractType { get; set; }
        public ContractStatus Status { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? SignedDate { get; set; }

        // === 👇 PHẦN LƯƠNG THƯỞNG (Đã sửa lại) ===

        // 1. Lương thỏa thuận (Ví dụ: 20 triệu)
        public decimal BaseSalary { get; set; }

        // 2. Kiểu lương (Gross hay Net) -> Quan trọng để tính thuế sau này
        public SalaryType SalaryType { get; set; }

        // 3. Lương đóng bảo hiểm (Ví dụ: Chỉ đóng trên mức 5 triệu)
        // Nếu null thì mặc định lấy BaseSalary để đóng
        public decimal? InsuranceSalary { get; set; }

        // 4. Các loại phụ cấp theo hợp đồng (Ăn trưa, Xăng xe, Điện thoại...)
        // Tạm thời mình lưu tổng, sau này xịn hơn thì tách bảng Allowance riêng
        public decimal AllowanceAmount { get; set; } = 0;

        // Lưu ý: Thuế sẽ được tính động bằng công thức (Calculator Service)
        // chứ không lưu cứng vào đây.

        // === 👆 HẾT PHẦN SỬA ===

        public string? AttachmentUrl { get; set; }

        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
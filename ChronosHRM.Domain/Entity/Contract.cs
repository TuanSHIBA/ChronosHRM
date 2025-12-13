using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Contract : BaseEntity
    {
        public string ContractCode { get; set; } = string.Empty; // HĐ-2024-001

        // Loại hợp đồng: Xác định thời hạn, Không xác định thời hạn...
        public string ContractType { get; set; } = string.Empty;

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; } // Null nếu là hợp đồng vô thời hạn

        public decimal BaseSalary { get; set; } // Lương cơ bản
        public decimal TaxPercentage { get; set; } // % Thuế (Ví dụ: 10.5)

        public bool IsActive { get; set; } = true; // Hợp đồng đang hiệu lực

        // FK
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}

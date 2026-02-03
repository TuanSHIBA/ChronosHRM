using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Position : BaseEntity
    {
        [MaxLength(50)]
        public required string Code { get; set; } // Mã vị trí: POS001, DEV, HR_MGR...

        [MaxLength(100)]
        public required string Title { get; set; } // Tên vị trí: Giám đốc, Kế toán trưởng...

        [MaxLength(500)]
        public string? Description { get; set; } // Mô tả công việc (Job Description)

        // Có thể thêm dải lương gợi ý cho vị trí này (Optional)
        public decimal? BaseSalaryRangeMin { get; set; }
        public decimal? BaseSalaryRangeMax { get; set; }

        // Quan hệ: Một vị trí có nhiều nhân viên nắm giữ
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

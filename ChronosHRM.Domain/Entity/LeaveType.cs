using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class LeaveType : BaseEntity
    {

        public required string Name { get; set; } // VD: "Nghỉ phép năm", "Nghỉ ốm"

        public string? Description { get; set; }

        public int DefaultDays { get; set; } // Số ngày mặc định được cấp (VD: 12 ngày/năm)

        public bool IsPaid { get; set; } // True = Có hưởng lương, False = Trừ lương
    }
}

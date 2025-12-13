using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum EmployeeStatus
    {
        Working = 1,    // Đang làm việc
        Resigned = 2,   // Đã nghỉ việc
        OnLeave = 3,    // Nghỉ thai sản/không lương dài hạn
        Probation = 4   // Thử việc
    }
}

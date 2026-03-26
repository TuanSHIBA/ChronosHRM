using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum EmployeeStatus
    {
        Probation = 0,      // Thử việc
        Official = 1,       // Chính thức
        Resigned = 2,       // Đã nghỉ việc
        Terminated = 3,     // Bị sa thải
        MaternityLeave = 4, // Nghỉ thai sản (Vẫn tính là nhân viên nhưng tạm vắng)
        UnpaidLeave = 5     // Nghỉ không lương dài hạn
    }
}

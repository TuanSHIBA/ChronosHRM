using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum LeaveStatus
    {
        Pending = 0,    // Chờ duyệt
        Approved = 1,   // Đã duyệt
        Rejected = 2,   // Từ chối
        Cancelled = 3   // Hủy (Nhân viên tự hủy đơn)
    }
}

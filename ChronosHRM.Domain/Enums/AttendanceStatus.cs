using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum AttendanceStatus
    {
        Pending = 0,   // Chờ duyệt (Dành cho chấm công qua Web/App)
        Approved = 1,  // Đã duyệt (Hợp lệ, được tính lương)
        Rejected = 2   // Bị từ chối (Không tính công)
    }
    public enum AttendanceSource
    {
        Machine = 0,   // Dữ liệu từ Máy chấm công (Tin tưởng tuyệt đối)
        Web = 1,       // Dữ liệu từ Web Portal (Cần duyệt)
        Mobile = 2     // (Dự phòng) Dữ liệu từ Mobile App
    }
}

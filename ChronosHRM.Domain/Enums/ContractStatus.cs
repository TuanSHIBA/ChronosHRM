using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum ContractStatus
    {
        Draft = 0,      // Nháp (Lưu tạm)
        Active = 1,     // Đang chạy (Đã ký)
        Expired = 2,    // Đã hết hạn (Hợp đồng cũ)
        Terminated = 3, // Bị hủy bỏ/Thôi việc giữa chừng
        Cancelled = 4   // Hủy (Viết sai nên hủy)
    }
}

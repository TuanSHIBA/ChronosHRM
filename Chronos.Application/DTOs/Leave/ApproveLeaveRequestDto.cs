using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Leave
{
    public class ApproveLeaveRequestDto
    {
        public Guid RequestId { get; set; }
        public bool IsApproved { get; set; } // True = Duyệt, False = Từ chối
        public string? ManagerNote { get; set; }
    }
}

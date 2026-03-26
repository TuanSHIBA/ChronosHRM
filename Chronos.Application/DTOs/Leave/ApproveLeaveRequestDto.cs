using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Leave
{
    public class ApproveLeaveRequestDto
    {
        public bool IsApproved { get; set; }
        public string? ManagerNote { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Leave
{
    public class CreateLeaveRequestDto
    {
        public required Guid LeaveTypeId { get; set; } 
        public required DateTime FromDate { get; set; }
        public required DateTime ToDate { get; set; }
        public required string Reason { get; set; }
    }
}

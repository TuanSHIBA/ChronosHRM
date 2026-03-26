using Chronos.Domain.Enums;
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
        public Guid LeaveTypeId { get; set; }
        public string? Reason { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public LeaveSession Session { get; set; } = LeaveSession.AllDay;
    }


}

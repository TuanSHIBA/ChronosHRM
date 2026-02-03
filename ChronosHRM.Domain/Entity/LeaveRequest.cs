using Chronos.Domain.Common;
using Chronos.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronos.Domain.Entity
{
    public class LeaveRequest : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee? Employee { get; set; }
        public Guid LeaveTypeId { get; set; }
        [ForeignKey("LeaveTypeId")]
        public virtual LeaveType? LeaveType { get; set; }

        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public double TotalDays { get; set; }

        public string? Reason { get; set; } 

        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;

        public Guid? ApprovedById { get; set; }
        public string? ManagerNote { get; set; }
        public LeaveSession Session { get; set; }
    }
}

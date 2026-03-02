using Chronos.Domain.Common;
using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class EmployeeTransfer : BaseEntity
    {
        public Guid EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public Guid? OldDepartmentId { get; set; }
        public Guid? OldPositionId { get; set; }
        public Guid? OldManagerId { get; set; }

        public Guid NewDepartmentId { get; set; }
        public Guid NewPositionId { get; set; }
        public Guid? NewManagerId { get; set; }

        public DateTime EffectiveDate { get; set; } 
        public string Reason { get; set; } = string.Empty; 

        public TransferStatus Status { get; set; } = TransferStatus.Pending;
        public TransferType Type { get; set; } = TransferType.Transfer;
    }
}

using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class DepartmentPosition : BaseEntity
    {
      
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;

        public Guid PositionId { get; set; }
        public virtual Position Position { get; set; } = null!;
        public int? MaxHeadcount { get; set; }
    }
}

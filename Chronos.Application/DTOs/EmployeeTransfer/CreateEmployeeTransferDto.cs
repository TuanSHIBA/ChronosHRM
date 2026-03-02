using Chronos.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.EmployeeTransfer
{
    public class CreateEmployeeTransferDto
    {
       
        public required Guid EmployeeId { get; set; }
        public required Guid NewDepartmentId { get; set; }
        public required Guid NewPositionId { get; set; }
        public Guid? NewManagerId { get; set; }
        public required DateTime EffectiveDate { get; set; }
        public required string Reason { get; set; } = string.Empty;
        public required TransferType Type { get; set; }
    }
}

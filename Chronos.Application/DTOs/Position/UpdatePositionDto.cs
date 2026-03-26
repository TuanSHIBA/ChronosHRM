using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Position
{
    public class UpdatePositionDto
    {
        public Guid DepartmentId { get; set; }
        public string PositionName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int Level { get; set; }
    }
}

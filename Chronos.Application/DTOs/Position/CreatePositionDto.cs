using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Position
{
    public class CreatePositionDto
    {
        public Guid DepartmentId { get; set; }
        public string Code { get; set; } = string.Empty; 
        public string PositionName { get; set; } = string.Empty; 
        public string? Description { get; set; }
        public int Level { get; set; }

    }
}

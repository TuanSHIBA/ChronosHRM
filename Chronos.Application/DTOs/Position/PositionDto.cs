using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Position
{
    public class PositionDto
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string PositionName { get; set; }  = string.Empty;
        public string? Description { get; set; }
        public decimal? BaseSalaryRangeMin { get; set; }
        public decimal? BaseSalaryRangeMax { get; set; }

    }
}

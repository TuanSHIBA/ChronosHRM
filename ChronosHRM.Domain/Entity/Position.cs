using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Position : BaseEntity
    {
        [MaxLength(50)]
        public required string Code { get; set; } 
        [MaxLength(100)]
        public required string PositionName { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; } 
        public decimal? BaseSalaryRangeMin { get; set; }
        public decimal? BaseSalaryRangeMax { get; set; }
        public int Level { get; set; }
        public Guid DepartmentId { get; set; }
        public virtual Department Department { get; set; } = null!;

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

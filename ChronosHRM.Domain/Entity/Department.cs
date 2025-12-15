using Chronos.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class Department : BaseEntity
    {
        public string Name { get; set; } = string.Empty; 
        public string Code { get; set; } = string.Empty; 

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}

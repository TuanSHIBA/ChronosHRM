using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Interfaces
{
    public interface IDepartmentPositionRepository : IGenericRepository<DepartmentPosition>
    { 
        Task<IEnumerable<DepartmentPosition>> GetByDepartmentIdAsync(Guid departmentId);
    }
}

using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Interfaces
{
    public interface ILeaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<List<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId);
        Task<List<LeaveRequest>> GetPendingListAsync();
    }
}

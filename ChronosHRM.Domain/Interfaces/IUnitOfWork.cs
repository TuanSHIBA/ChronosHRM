using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IEmployeeRepository Employees { get; }
        IDepartmentRepository Departments { get; }
        IEmploymentContractRepository Contracts { get; }
        IAttendanceRepository  Attendance { get; }
        ILeaveTypeRepository LeaveType { get; }
        Task<int> SaveChangesAsync();
    }
}

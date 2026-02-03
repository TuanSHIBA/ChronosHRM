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
        IEmploymentContractRepository EmploymentContracts { get; }
        IAttendanceRepository  Attendance { get; }
        ILeaveTypeRepository LeaveType { get; }
        ILeaveRequestRepository LeaveRequest { get; }
        IMenuRepository Menus { get; }
        Task<int> SaveChangesAsync();
    }
}

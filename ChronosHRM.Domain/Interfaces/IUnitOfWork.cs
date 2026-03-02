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
        IPositionRepository Position { get; }  
        IMenuRepository Menus { get; }
        IEmployeeTransferRepository EmployeeTransfers { get; }
        Task<int> SaveChangesAsync();
    }
}

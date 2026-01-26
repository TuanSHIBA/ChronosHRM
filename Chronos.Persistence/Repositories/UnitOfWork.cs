using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChronosDbContext _context;

        // Khai báo biến cache cho các Repository cụ thể
        private IEmployeeRepository? _employeeRepository;
        private IDepartmentRepository? _departmentRepository;
        private IEmploymentContractRepository? _contractRepository;
        private IAttendanceRepository? _attendanceRepository;
        private ILeaveTypeRepository? _LeaveType;
        public UnitOfWork(ChronosDbContext context)
        {
            _context = context;
        }

        public IEmployeeRepository Employees
        {
            get { return _employeeRepository ??= new EmployeeRepository(_context); }
        }

        public IDepartmentRepository Departments
        {
            get { return _departmentRepository ??= new DepartmentRepository(_context); }
        }

        public IEmploymentContractRepository Contracts
        {
            get { return _contractRepository ??= new EmploymentContractRepository(_context); }
        }

        public IAttendanceRepository Attendance
        {
            get { return _attendanceRepository ??= new AttendanceRepository(_context); }
        }
        public ILeaveTypeRepository LeaveType
        {
            get { return _LeaveType ??= new LeaveTypeRepository(_context); }
        }


        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

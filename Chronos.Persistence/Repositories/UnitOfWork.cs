using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;
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
        private IEmployeeRepository? _employee;
        private IDepartmentRepository? _department;
        private IEmploymentContractRepository? _employmentContracts;
        private IAttendanceRepository? _attendance;
        private ILeaveTypeRepository? _leaveType;
        private ILeaveRequestRepository? _leaveRequest;
        private IMenuRepository? _menus;
        private IPositionRepository? _positions;
        private IEmployeeTransferRepository? _employeeTransfer;
        private IContractAnnexRepository? _contractAnnex;
        public UnitOfWork(ChronosDbContext context)
        {
            _context = context;
        }

        public IEmployeeRepository Employees
        {
            get { return _employee ??= new EmployeeRepository(_context); }
        }

        public IDepartmentRepository Departments
        {
            get { return _department ??= new DepartmentRepository(_context); }
        }

        public IEmploymentContractRepository EmploymentContracts
        {
            get { return _employmentContracts ??= new EmploymentContractRepository(_context); }
        }

        public IAttendanceRepository Attendance
        {
            get { return _attendance ??= new AttendanceRepository(_context); }
        }
        public ILeaveTypeRepository LeaveType
        {
            get { return _leaveType ??= new LeaveTypeRepository(_context); }
        }
        public ILeaveRequestRepository LeaveRequest
        {
            get { return _leaveRequest ??= new LeaveRequestRepository(_context); }
        }
        public IMenuRepository Menus
        {
            get { return _menus ??= new MenuRepository(_context); }
        }
        public IPositionRepository Position
        {
            get { return _positions ??= new PositionRepository(_context); }
        }
        public IEmployeeTransferRepository EmployeeTransfers
        {
            get { return _employeeTransfer ??= new EmployeeTransferRepository(_context); }
        } 
        public IContractAnnexRepository ContractAnnexes
        {
            get { return _contractAnnex ??= new ContractAnnexRepository(_context); }
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

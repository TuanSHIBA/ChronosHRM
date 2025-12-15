using Chronos.Application.Interfaces;
using Chronos.Domain.Entities;
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

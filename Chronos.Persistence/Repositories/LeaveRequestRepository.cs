using Chronos.Domain.Entity;
using Chronos.Domain.Enums;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, ILeaveRequestRepository
    {
        public LeaveRequestRepository(ChronosDbContext context) : base(context)
        {
        }

        public async Task<List<LeaveRequest>> GetByEmployeeIdAsync(Guid employeeId)
        {
            return await _context.LeaveRequests
                .Include(q => q.LeaveType) // Kèm loại nghỉ
                .Where(q => q.EmployeeId == employeeId)
                .OrderByDescending(q => q.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetPendingListAsync()
        {
            return await _context.LeaveRequests
                .Include(q => q.Employee)  // Kèm tên nhân viên
                .Include(q => q.LeaveType) // Kèm tên loại nghỉ
                .Where(q => q.Status == LeaveStatus.Pending)
                .OrderBy(q => q.CreatedAt) // Ai xin trước hiện trước
                .ToListAsync();
        }
    }
}

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
    // nếu giờ chỉ implement IAttendanceRepository, thì sẽ phải implement tất cả các method trong interface đó, 
    // nên phải implement GenericRepository để tái sử dụng code, vì GenericRepository đã implement IGenericRepository rồi, interface  IAttendanceRepository đã kế thừa IGenericRepository rồi 
    internal class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
    {
        public AttendanceRepository(ChronosDbContext context) : base(context)
        {
        }

        public async Task<List<Attendance>> GetPendingListAsync()
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Status == AttendanceStatus.Pending && a.Source == AttendanceSource.Web)
                .OrderByDescending(a => a.Date) // Mới nhất lên đầu
                .ToListAsync();
        }
        public async Task<Attendance?> GetByDateAsync(Guid employeeId, DateTime date)
        {
            // Chỉ lấy bản ghi của nhân viên đó trong ngày hôm nay
            // EF Core sẽ sinh ra SQL: SELECT TOP 1 ... WHERE EmployeeId = ... AND Date = ...
            return await _context.Attendances
                .FirstOrDefaultAsync(a => a.EmployeeId == employeeId && a.Date.Date == date.Date);
        }
        // AttendanceRepository.cs
        public async Task<List<Attendance>> GetMyAttendanceHistoryAsync(Guid employeeId, int month, int year)
        {
            return await _context.Attendances
                .Where(x => x.EmployeeId == employeeId &&
                            x.Date.Month == month &&
                            x.Date.Year == year)
                .OrderByDescending(x => x.Date) // Ngày mới nhất lên đầu cho dễ nhìn
                .ToListAsync();
        }
    }
}

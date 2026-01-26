using Chronos.Domain.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Interfaces
{
    public interface IAttendanceRepository : IGenericRepository<Attendance>
    {
        Task<List<Attendance>> GetPendingListAsync();
        Task<Attendance?> GetByDateAsync(Guid employeeId, DateTime date);
        Task<List<Attendance>> GetMyAttendanceHistoryAsync(Guid employeeId, int month, int year);
    }
}

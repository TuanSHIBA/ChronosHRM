using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using Chronos.Persistence.Context;
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
    }
}

using Chronos.Domain.Entity;
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
    public class DepartmentPositionRepository : GenericRepository<DepartmentPosition>, IDepartmentPositionRepository
    {
        public DepartmentPositionRepository(ChronosDbContext context) : base(context)
        {
        }

        // Thực thi hàm lấy danh sách vị trí theo phòng ban
        public async Task<IEnumerable<DepartmentPosition>> GetByDepartmentIdAsync(Guid departmentId)
        {
            // Dùng .Include() để JOIN bảng Position vào, tránh lỗi N+1 Query
            return await _dbSet
                .Include(dp => dp.Position)
                .Where(dp => dp.DepartmentId == departmentId && !dp.IsDeleted && !dp.Position.IsDeleted)
                .ToListAsync();
        }
    }
}

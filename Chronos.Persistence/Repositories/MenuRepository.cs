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
    public class MenuRepository(ChronosDbContext context) : IMenuRepository
    {
        public async Task<List<AppMenu>> GetAllAsync()
        {
            // Code truy vấn DB nằm gọn ở đây
            return await context.AppMenus.OrderBy(x=>x.OrderIndex).ToListAsync();
                           
        }
    }
}

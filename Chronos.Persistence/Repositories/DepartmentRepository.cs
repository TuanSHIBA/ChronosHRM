using Chronos.Application.Interfaces;
using Chronos.Domain.Entity;
using Chronos.Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Persistence.Repositories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {
        public DepartmentRepository(ChronosDbContext context) : base(context)
        {
        }
    }
}

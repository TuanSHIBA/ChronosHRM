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
    internal class PositionRepository : GenericRepository<Position>, IPositionRepository
    {
        public PositionRepository(ChronosDbContext context) : base(context)
        {
        }
    }
}

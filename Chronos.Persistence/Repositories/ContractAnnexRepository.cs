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
    public  class ContractAnnexRepository : GenericRepository<ContractAnnex>, IContractAnnexRepository
    {
        public ContractAnnexRepository(ChronosDbContext context) : base(context)
        {
        }
    }
}

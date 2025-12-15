using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum ContractType
    {
        Probation = 0,      // Thử việc
        DefiniteTerm = 1,   // Xác định thời hạn (12 tháng, 24 tháng...)
        IndefiniteTerm = 2, // Không xác định thời hạn
    }
}

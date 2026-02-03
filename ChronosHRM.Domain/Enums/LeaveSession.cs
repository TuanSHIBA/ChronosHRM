using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Enums
{
    public enum LeaveSession
    {
        AllDay = 0,    // Cả ngày (1.0)
        Morning = 1,   // Sáng (0.5)
        Afternoon = 2  // Chiều (0.5)
    }
}

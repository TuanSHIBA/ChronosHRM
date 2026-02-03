using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Dashboard
{
    public class DashboardSummaryDto
    {
        // 1. Thống kê Nhân sự
        public int TotalEmployees { get; set; }       
        public int NewEmployees { get; set; }         

        // 2. Thống kê Nghỉ phép
        public int OnLeaveToday { get; set; }         
        public int PendingLeaveRequests { get; set; } 

        // 3. Thống kê Hợp đồng
        public int ContractsExpiringSoon { get; set; } 
    }
}

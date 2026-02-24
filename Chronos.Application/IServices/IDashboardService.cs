using Chronos.Application.Common.Models.Chronos.Application.Common.Models;
using Chronos.Application.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.IServices
{
    public interface IDashboardService
    {
        Task<ServiceResponse<DashboardSummaryDto>> GetSummaryAsync();
        Task<ServiceResponse<EmployeeDashboardDto>> GetEmployeeSummaryAsync(Guid employeeId);
    }
}

namespace Chronos.Application.DTOs.EmployeeTransfer
{
    public class EmployeeTransferDto
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;

        public Guid? OldDepartmentId { get; set; }
        public string? OldDepartmentName { get; set; }

        public Guid NewDepartmentId { get; set; }
        public string? NewDepartmentName { get; set; }

        public Guid? OldPositionId { get; set; }
        public string? OldPositionName { get; set; }

        public Guid NewPositionId { get; set; }
        public string? NewPositionName { get; set; }
        public decimal? NewBaseSalary { get; set; }
        public decimal? NewAllowanceAmount { get; set; }
        public DateTime EffectiveDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public int Status { get; set; }
        public int Type { get; set; }
        public string TypeName { get; set; } = string.Empty;
    }
}

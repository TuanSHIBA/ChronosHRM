using Chronos.Domain.Common;
using Chronos.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Chronos.Domain.Entity
{
    public class EmploymentContract : BaseEntity
    {
        public Guid EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; } 

        [MaxLength(50)]
        public required string ContractCode { get; set; } 

        public ContractType ContractType { get; set; }    

        public ContractStatus Status { get; set; }      

        public DateTime SignDate { get; set; }  
        public DateTime StartDate { get; set; } 
        public DateTime? EndDate { get; set; }  

        public Guid? PositionId { get; set; }

        [ForeignKey(nameof(PositionId))]
        public Position? Position { get; set; }

        [MaxLength(255)]
        public string? WorkingLocation { get; set; }

        public decimal BaseSalary { get; set; }     

        public SalaryType SalaryType { get; set; } 

        [Column(TypeName = "decimal(18, 2)")]
        public decimal InsuranceSalary { get; set; }  // Lương đóng BHXH (Thường thấp hơn BaseSalary)

        [Column(TypeName = "decimal(18, 2)")]
        public decimal MealAllowance { get; set; } = 0;   // Ăn trưa

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TravelAllowance { get; set; } = 0; // Xăng xe

        [Column(TypeName = "decimal(18, 2)")]
        public decimal OtherAllowance { get; set; } = 0;  // Khác

        public string? AttachmentUrl { get; set; }    // File scan hợp đồng

        [MaxLength(500)]
        public string? Note { get; set; }
    }
}
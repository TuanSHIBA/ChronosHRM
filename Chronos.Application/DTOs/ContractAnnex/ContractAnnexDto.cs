using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.ContractAnnex
{
    public class ContractAnnexDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string AnnexCode { get; set; } = string.Empty;
        public DateTime EffectiveDate { get; set; }
        public DateTime SignDate { get; set; }
        public string Content { get; set; } = string.Empty;
        public decimal BaseSalary { get; set; }
        public decimal OtherAllowance { get; set; }
    }
}

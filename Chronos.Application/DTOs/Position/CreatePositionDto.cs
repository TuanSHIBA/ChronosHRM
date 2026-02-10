using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Position
{
    public class CreatePositionDto
    {
        [Required(ErrorMessage = "Vui lòng nhập mã chức vụ")]
        [MaxLength(20, ErrorMessage = "Mã chức vụ tối đa 20 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_-]+$", ErrorMessage = "Mã chức vụ chỉ được chứa chữ, số, gạch ngang hoặc gạch dưới")]
        public string Code { get; set; } = string.Empty; 
        // 2. Tên chức vụ (Bắt buộc)
        [Required(ErrorMessage = "Vui lòng nhập tên chức vụ")]
        [MaxLength(100, ErrorMessage = "Tên chức vụ tối đa 100 ký tự")]
        public string PositionName { get; set; } = string.Empty; 

        [MaxLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
        public string? Description { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương tối thiểu không được âm")]
        public decimal? BaseSalaryRangeMin { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Lương tối đa không được âm")]
        public decimal? BaseSalaryRangeMax { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Application.DTOs.Leave
{
    public class CreateLeaveTypeDto
    {
        [Required(ErrorMessage = "Tên loại nghỉ không được để trống")]
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int DefaultDays { get; set; }
        public bool IsPaid { get; set; }
    }
}

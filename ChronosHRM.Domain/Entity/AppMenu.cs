using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Domain.Entity
{
    public class AppMenu
    {
 
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty; 

        [MaxLength(200)]
        public string? Path { get; set; } 

        [MaxLength(50)]
        public string? Icon { get; set; } 

        public int? ParentId { get; set; } 

        public int OrderIndex { get; set; } 

        [MaxLength(100)]
        public string? RequiredPermission { get; set; }

        [ForeignKey("ParentId")]
        public virtual AppMenu? Parent { get; set; }
        public virtual ICollection<AppMenu> Children { get; set; } = new List<AppMenu>();
    }
}

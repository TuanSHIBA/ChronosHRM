using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chronos.Domain.Entity
{
    [Table("Attendances")]
    public class Attendance
    {
        [Key]
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public DateTime Date { get; set; } 

        public TimeSpan? CheckInTime { get; set; } 

        public TimeSpan? CheckOutTime { get; set; }

        public double WorkingHours { get; set; } 

        public Employee Employee { get; set; }
    }
}
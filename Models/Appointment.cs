using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace fitnessCenter.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        public int CotchId { get; set; }
        [ForeignKey("CotchId")]
        public virtual Cotch? Cotch { get; set; }

        public int ExerciseId { get; set; }
        [ForeignKey("ExerciseId")]
        public virtual Exercise? Exercise { get; set; }

        public string? AppointmentDate { get; set; }
        
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected
    }
}

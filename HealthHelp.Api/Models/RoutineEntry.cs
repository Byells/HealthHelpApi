using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthHelp.Api.Models
{
    public class RoutineEntry
    {
        [Key] 
        public int Id { get; set; }

        [Required] 
        public string Category { get; set; } 
        
        public string? Description { get; set; }

        [Required]
        public decimal Hours { get; set; } 

        [Required]
        public DateOnly EntryDate { get; set; } 

        [Required]
        public string ApplicationUserId { get; set; } 

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser User { get; set; }
    }
}
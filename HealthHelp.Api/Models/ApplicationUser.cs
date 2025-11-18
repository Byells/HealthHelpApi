using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HealthHelp.Api.Models
{
    public class ApplicationUser : IdentityUser
    {

        [Required] 
        [StringLength(100)]
        public string Name { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public virtual ICollection<RoutineEntry> RoutineEntries { get; set; }
    }
}
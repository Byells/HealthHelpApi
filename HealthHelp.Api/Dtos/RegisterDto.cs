using System.ComponentModel.DataAnnotations;

namespace HealthHelp.Api.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public DateOnly DateOfBirth { get; set; }
    }
}
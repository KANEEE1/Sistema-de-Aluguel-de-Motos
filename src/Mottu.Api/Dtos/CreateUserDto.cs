using System.ComponentModel.DataAnnotations;

namespace Mottu.Api.Dtos
{
    public class CreateUserDto
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Admin|DeliveryPerson)$", ErrorMessage = "Role must be Admin or DeliveryPerson")]
        public string Role { get; set; } = string.Empty;
    }
}

using System.ComponentModel.DataAnnotations;

namespace PokemonWebAPI.Models
{
    public class RegisterDto
    {
        [Required]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}

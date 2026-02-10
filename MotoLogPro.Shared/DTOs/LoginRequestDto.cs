using System.ComponentModel.DataAnnotations;

namespace MotoLogPro.Shared.DTOs
{
    public class LoginRequestDto
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        public string Password { get; set; } = string.Empty;
    }
}
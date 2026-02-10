using System.ComponentModel.DataAnnotations;

namespace MotoLogPro.Shared.DTOs
{
    public class RegisterRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Il nome deve avere almeno 3 caratteri")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email non valida")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La password deve essere di almeno 6 caratteri")]
        // Regex per obbligare numeri, maiuscole e minuscole
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "La password deve contenere maiuscole, minuscole e numeri")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
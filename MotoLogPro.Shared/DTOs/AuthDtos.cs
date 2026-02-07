using System.ComponentModel.DataAnnotations;

namespace MotoLogPro.Shared.DTOs
{
    // DTO per la richiesta di Login
    public record LoginRequestDto
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Inserisci un indirizzo email valido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La password è obbligatoria")]
        public string Password { get; set; } = string.Empty;
    }

    // DTO per la risposta del Login (cosa riceve l'app)
    public record LoginResponseDto
    {
        public string Token { get; set; } = string.Empty; // Il JWT
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;  // "Admin", "Meccanico", "Cliente"
        public DateTime Expiration { get; set; }
    }

    // DTO per la Registrazione
    public record RegisterRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Il nome deve avere almeno 3 caratteri")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "La password deve essere di almeno 6 caratteri")]
        // Malizia: Regex per obbligare numeri e lettere (opzionale ma consigliato)
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{6,}$",
            ErrorMessage = "La password deve contenere maiuscole, minuscole e numeri")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Le password non coincidono")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
namespace MotoLogPro.Shared.DTOs
{
    public class LoginResponseDto
    {
        // CAMBIAMENTO FONDAMENTALE: L'API restituisce "accessToken", non "Token"
        public string AccessToken { get; set; } = string.Empty;

        public string TokenType { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public string RefreshToken { get; set; } = string.Empty;

        // Nota: L'API standard /login NON restituisce Email e Ruolo di default.
        // Li togliamo per ora per evitare confusione (sarebbero sempre null).
        // Li recupereremo decodificando il token o con una chiamata /me successiva.
    }
}
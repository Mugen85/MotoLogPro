using System.Net.Http.Json;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public class AuthService(HttpClient httpClient) : IAuthService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<bool> LoginAsync(string email, string password)
        {
            try
            {
                var loginDto = new LoginRequestDto { Email = email, Password = password };

                // Chiamata all'API
                var response = await _httpClient.PostAsJsonAsync("/login", loginDto);

                if (response.IsSuccessStatusCode)
                {
                    // Ora LoginResponseDto esiste e Visual Studio lo riconoscerà (usa CTRL+. per aggiungere il using se serve)
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    // NOTA BENE: La proprietà corretta è .AccessToken (come nel DTO che abbiamo appena creato)
                    if (result is not null && !string.IsNullOrEmpty(result.AccessToken))
                    {
                        await SecureStorage.SetAsync("auth_token", result.AccessToken);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                // AGGIUNGI QUESTA RIGA PER VEDERE L'ERRORE NELLA CONSOLE DI OUTPUT
                System.Diagnostics.Debug.WriteLine($"[ERRORE LOGIN]: {ex.Message}");

                // Se vuoi vedere l'errore a schermo (solo per debug estremo):
                // await Shell.Current.DisplayAlert("Debug Error", ex.Message, "OK");

                return false;
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                // 1. Recupero il refresh token salvato
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                // 2. Chiedo al backend un nuovo access token
                var dto = new RefreshTokenRequestDto { RefreshToken = refreshToken };
                var response = await _httpClient.PostAsJsonAsync("/refresh", dto);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                    if (result is not null && !string.IsNullOrEmpty(result.AccessToken))
                    {
                        // 3. Salvo i nuovi token (sia access che refresh — rotation)
                        await SecureStorage.SetAsync("auth_token", result.AccessToken);
                        await SecureStorage.SetAsync("refresh_token", result.RefreshToken);
                        return true;
                    }
                }

                // Se il refresh fallisce, forziamo il logout
                await LogoutAsync();
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERRORE REFRESH]: {ex.Message}");
                await LogoutAsync();
                return false;
            }
        }

        public async Task<bool> RegisterAsync(string fullName, string email, string password, string confirmPassword)
        {
            var registerDto = new RegisterRequestDto
            {
                FullName = fullName,
                Email = email,
                Password = password,
                ConfirmPassword = confirmPassword
            };

            var response = await _httpClient.PostAsJsonAsync("/register", registerDto);
            return response.IsSuccessStatusCode;
        }

        public Task LogoutAsync()
        {
            SecureStorage.Remove("auth_token");
            SecureStorage.Remove("refresh_token");
            return Task.CompletedTask;
        }

        public async Task<bool> IsUserLoggedIn()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }
    }
}
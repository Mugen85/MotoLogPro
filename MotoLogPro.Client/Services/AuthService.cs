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
                System.Diagnostics.Debug.WriteLine($"Login error: {ex.Message}");
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
            return Task.CompletedTask;
        }

        public async Task<bool> IsUserLoggedIn()
        {
            var token = await SecureStorage.GetAsync("auth_token");
            return !string.IsNullOrEmpty(token);
        }
    }
}
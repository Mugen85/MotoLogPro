using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public class VehicleService(HttpClient httpClient, IAuthService authService) : IVehicleService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IAuthService _authService = authService;

        public async Task<List<VehicleDto>> GetVehiclesAsync()
        {
            // Prima chiamata con il token attuale
            var response = await SendAuthenticatedRequest(
                () => _httpClient.GetAsync("api/vehicles")
            );

            // Se il token è scaduto (401), proviamo a rinnovarlo e ripetere
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                var refreshed = await _authService.RefreshTokenAsync();
                if (!refreshed) return []; // Refresh fallito → logout già gestito in AuthService

                // Seconda chiamata con il token nuovo
                response = await SendAuthenticatedRequest(
                    () => _httpClient.GetAsync("api/vehicles")
                );
            }

            if (!response.IsSuccessStatusCode) return [];

            var result = await response.Content.ReadFromJsonAsync<List<VehicleDto>>();
            return result ?? [];
        }

        // Metodo helper: attacca il token JWT ad ogni richiesta
        private async Task<HttpResponseMessage> SendAuthenticatedRequest(
            Func<Task<HttpResponseMessage>> request)
        {
            var token = await SecureStorage.GetAsync("auth_token");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token ?? string.Empty);

            return await request();
        }
    }
}
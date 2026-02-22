using System.Net.Http.Headers;
using System.Net.Http.Json;
using MotoLogPro.Shared.DTOs;

namespace MotoLogPro.Client.Services
{
    public class VehicleService(HttpClient httpClient) : IVehicleService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<List<VehicleDto>> GetVehiclesAsync()
        {
            // 1. Recuperiamo il Token salvato al Login
            var token = await SecureStorage.GetAsync("auth_token");

            if (string.IsNullOrEmpty(token))
                return []; // O gestisci l'errore (logout forzato)

            // 2. Attacchiamo il Token alla richiesta (Header: Authorization Bearer eyJ...)
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            try
            {
                // 3. Chiamiamo l'API
                // Assumiamo che l'endpoint sia /api/vehicles (controlla il tuo Controller API!)
                var response = await _httpClient.GetFromJsonAsync<List<VehicleDto>>("api/motorcycles");
                return response ?? [];
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ERRORE API] {ex.Message}");
                return []; // Per ora ritorniamo vuoto in caso di errore
            }
        }
    }
}
using MotoLogPro.Shared.DTOs;
using System.Net.Http.Json;
using System.Text.Json; // <-- Importante per leggere il JSON del Middleware

namespace MotoLogPro.Client.Services
{
    public class VehicleService(HttpClient httpClient, IAuthService authService) : IVehicleService
    {
        private readonly HttpClient _httpClient = httpClient;
        private readonly IAuthService _authService = authService;

        public async Task<IEnumerable<VehicleDto>> GetVehiclesAsync()
        {
            await SetAuthorizationHeader();

            var response = await _httpClient.GetAsync("api/vehicles");

            if (response.IsSuccessStatusCode)
            {
                var vehicles = await response.Content.ReadFromJsonAsync<IEnumerable<VehicleDto>>();
                return vehicles ?? [];
            }

            // Se arriviamo qui, c'è un problema (es. 401 Unauthorized, 500 Server Error)
            throw new Exception($"Errore nel recupero dei veicoli: {response.StatusCode}");
        }

        public async Task<VehicleDto?> CreateVehicleAsync(CreateMotorcycleDto dto)
        {
            await SetAuthorizationHeader();

            var response = await _httpClient.PostAsJsonAsync("api/vehicles", dto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<VehicleDto>();
            }

            // ----------------------------------------------------------------------
            // ESTRAZIONE DEL PROBLEMDETAILS (Gestione 409 Conflict)
            // ----------------------------------------------------------------------
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                var errorContent = await response.Content.ReadAsStringAsync();

                try
                {
                    // Leggiamo il JSON senza creare DTO superflui
                    using var doc = JsonDocument.Parse(errorContent);

                    // Cerchiamo la proprietà "title" (CamelCase dal tuo Middleware)
                    // Nota: Se JsonNamingPolicy è CamelCase, il campo sarà "title" o "Title".
                    // Per sicurezza cerchiamo case-insensitive usando RootElement.
                    foreach (var prop in doc.RootElement.EnumerateObject())
                    {
                        if (prop.NameEquals("title") || prop.NameEquals("Title"))
                        {
                            throw new Exception(prop.Value.GetString());
                        }
                    }
                }
                catch (JsonException)
                {
                    // Fallback silenzioso: se non è JSON valido, ignoriamo il parsing
                }

                // Se non riusciamo a estrarre il "title" o non è JSON, mostriamo l'errore grezzo
                throw new Exception(errorContent);
            }

            // Altri errori
            response.EnsureSuccessStatusCode();
            return null;
        }

        public async Task<bool> UpdateVehicleAsync(int id, CreateMotorcycleDto dto)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.PutAsJsonAsync($"api/vehicles/{id}", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteVehicleAsync(int id)
        {
            await SetAuthorizationHeader();
            var response = await _httpClient.DeleteAsync($"api/vehicles/{id}");
            return response.IsSuccessStatusCode;
        }

        /// <summary>
        /// Aggiunge il Token JWT nell'header di ogni richiesta HTTP.
        /// </summary>
        private async Task SetAuthorizationHeader()
        {
            var token = await _authService.GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
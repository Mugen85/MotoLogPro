using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoLogPro.Client.Services;
using MotoLogPro.Shared.DTOs;
using System.Collections.ObjectModel;

namespace MotoLogPro.Client.ViewModels
{
    public partial class DashboardViewModel(IVehicleService vehicleService) : ObservableObject
    {
        private readonly IVehicleService _vehicleService = vehicleService;

        // La lista che la UI osserva. ObservableCollection aggiorna la UI automaticamente quando aggiungi/rimuovi item.
        public ObservableCollection<VehicleDto> Vehicles { get; } = [];

        [ObservableProperty]
        bool isBusy;

        [RelayCommand]
        async Task LoadData()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Proviamo a chiamare l'API
                var list = await _vehicleService.GetVehiclesAsync();

                Vehicles.Clear();

                if (list.Count > 0)
                {
                    foreach (var v in list) Vehicles.Add(v);
                }
                else
                {
                    // --- DATI MOCK (FINTI) PER TESTARE LA GRAFICA ---
                    // Togli questo blocco quando avrai il DB pieno!
                    Vehicles.Add(new VehicleDto { Brand = "Yamaha", Model = "XT1200Z Super Ténéré", LicensePlate = "AA123BB", Year = 2018, OwnerName = "Io" });
                    Vehicles.Add(new VehicleDto { Brand = "Honda", Model = "Africa Twin 1100", LicensePlate = "CC456DD", Year = 2022, OwnerName = "Cliente Test" });
                    Vehicles.Add(new VehicleDto { Brand = "Ducati", Model = "Multistrada V4", LicensePlate = "EE789FF", Year = 2023, OwnerName = "Mario Rossi" });
                    Vehicles.Add(new VehicleDto { Brand = "Moto Guzzi", Model = "V85 TT", LicensePlate = "GG101HH", Year = 2021, OwnerName = "Luigi Verdi" });
                    // -----------------------------------------------
                }
            }
            catch (Exception ex)
            {
                // In caso di errore API, mostriamo comunque i dati finti per ora
                await Shell.Current.DisplayAlert("Info", "Impossibile contattare server, carico dati test.", "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Logout()
        {
            SecureStorage.Remove("auth_token");
            // Torniamo al Login (gestiremo la navigazione tra poco)
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoLogPro.Client.Services;
using MotoLogPro.Shared.DTOs;
using System.Collections.ObjectModel;

namespace MotoLogPro.Client.ViewModels
{
    public partial class DashboardViewModel(
        IVehicleService vehicleService,
        IAuthService authService) : ObservableObject
    {
        private readonly IVehicleService _vehicleService = vehicleService;
        private readonly IAuthService _authService = authService;

        public ObservableCollection<VehicleDto> Vehicles { get; } = [];

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        bool isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        bool hasError;

        [ObservableProperty]
        string errorMessage = string.Empty;

        public bool IsNotBusy => !IsBusy;

        // Mostra l'empty state solo se non stiamo caricando, non c'è errore e la lista è vuota
        public bool ShowEmptyState => !IsBusy && !HasError && Vehicles.Count == 0;

        [RelayCommand]
        async Task LoadData()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;

            try
            {
                var list = await _vehicleService.GetVehiclesAsync();

                Vehicles.Clear();
                foreach (var v in list)
                    Vehicles.Add(v);

                // Notifica ShowEmptyState dopo aver popolato la lista
                OnPropertyChanged(nameof(ShowEmptyState));
            }
            catch (HttpRequestException ex)
            {
                HasError = true;
                ErrorMessage = "Impossibile contattare il server. Verifica la connessione.";
                System.Diagnostics.Debug.WriteLine($"[ERRORE RETE]: {ex.Message}");
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = "Si è verificato un errore imprevisto.";
                System.Diagnostics.Debug.WriteLine($"[ERRORE]: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        async Task Logout()
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
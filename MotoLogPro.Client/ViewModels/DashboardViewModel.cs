
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
        private bool isBusy;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(ShowEmptyState))]
        private bool hasError;
        [ObservableProperty]
        private string errorMessage = string.Empty;

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

        [RelayCommand]
        async Task AddNewVehicle()
        {
            // Naviga verso la pagina di dettaglio
            await Shell.Current.GoToAsync(nameof(Pages.VehicleDetailPage));
        }

        [RelayCommand]
        private async Task DeleteMotorcycleAsync(VehicleDto? moto)
        {
            // 1. FAIL-FAST & KISS: Se il binding dello XAML ha fallito o l'ID è invalido, 
            // usciamo all'istante. Niente eccezioni che fanno esplodere l'app, semplicemente non facciamo nulla.
            if (moto is null || moto.Id <= 0)
                return;

            // 2. Da qui in poi sappiamo che "moto" è valida. Procediamo col flusso UI.
            bool confirm = await Shell.Current.DisplayAlertAsync(
                "Rottamazione",
                $"Sei sicuro di voler eliminare la {moto.Brand} {moto.Model}?",
                "Elimina", "Annulla");

            if (!confirm)
                return; // Altro Fail-Fast/Early Return: l'utente ha annullato, usciamo subito.

            // 3. Optimistic UI Update (Feedback immediato)
            int originalIndex = Vehicles.IndexOf(moto);
            Vehicles.Remove(moto);
            OnPropertyChanged(nameof(ShowEmptyState));

            try
            {
                // 4. Chiamata di rete
                var success = await _vehicleService.DeleteVehicleAsync(moto.Id);

                if (!success)
                {
                    // Rollback
                    Vehicles.Insert(originalIndex, moto);
                    OnPropertyChanged(nameof(ShowEmptyState));
                    await Shell.Current.DisplayAlertAsync("Errore", "Impossibile eliminare il veicolo dal server. Riprova.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Rollback
                Vehicles.Insert(originalIndex, moto);
                OnPropertyChanged(nameof(ShowEmptyState));
                await Shell.Current.DisplayAlertAsync("Errore", $"Errore di comunicazione: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task EditMotorcycleAsync(VehicleDto moto)
        {
            if (moto is null) return;

            // Passiamo la moto selezionata alla pagina di dettaglio tramite il dizionario di navigazione
            var navigationParameter = new Dictionary<string, object>
    {
        { "VehicleToEdit", moto }
    };

            await Shell.Current.GoToAsync(nameof(Pages.VehicleDetailPage), navigationParameter);
        }
    }
}
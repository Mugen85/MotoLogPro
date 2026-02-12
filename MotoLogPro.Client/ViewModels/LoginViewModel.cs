using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoLogPro.Client.Services;

namespace MotoLogPro.Client.ViewModels
{
    public partial class LoginViewModel(IAuthService authService) : ObservableObject
    {
        private readonly IAuthService _authService = authService;

        // --- PROPRIETÀ ---

        private string? _email;
        public string? Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        private string? _password;
        public string? Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }

        private bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (SetProperty(ref _isBusy, value))
                {
                    OnPropertyChanged(nameof(IsNotBusy));
                }
            }
        }

        public bool IsNotBusy => !IsBusy;

        // --- COMANDI ---

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            // 1. Validazione Input
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlert("Errore", "Inserisci email e password", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                // 2. Tentativo di Login tramite API
                bool success = await _authService.LoginAsync(Email, Password);

                if (success)
                {
                    // 3. NAVIGAZIONE (Il pezzo mancante!)
                    // Le "//" resettano lo stack di navigazione. 
                    // Significa che l'utente non potrà tornare al Login premendo "Indietro".
                    await Shell.Current.GoToAsync("//DashboardPage");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Errore", "Credenziali non valide", "Riprova");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Errore Tecnico", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
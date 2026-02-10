using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoLogPro.Client.Services;

namespace MotoLogPro.Client.ViewModels
{
    public partial class LoginViewModel(IAuthService authService) : ObservableObject
    {
        private readonly IAuthService _authService = authService;

        // --- CORREZIONE 1: Usa proprietà standard per evitare MVVMTK0041 ---

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

        // --- CORREZIONE 2: Usa bool normale (non nullable) ---

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

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy) return;

            // Validazione
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                await Shell.Current.DisplayAlertAsync("Errore", "Inserisci email e password", "OK");
                return;
            }

            try
            {
                IsBusy = true;

                bool success = await _authService.LoginAsync(Email, Password);

                if (success)
                {
                    await Shell.Current.DisplayAlertAsync("Successo", "Benvenuto in MotoLogPro!", "Gas a martello ✊");
                    // Qui metteremo la navigazione in futuro
                }
                else
                {
                    await Shell.Current.DisplayAlertAsync("Errore", "Credenziali non valide", "Riprova");
                }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Errore Tecnico", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
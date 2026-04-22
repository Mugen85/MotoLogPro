using MotoLogPro.Client.Services;
using MotoLogPro.Shared.DTOs;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MotoLogPro.Client.ViewModels
{
    public class VehicleDetailViewModel : INotifyPropertyChanged
    {
        private readonly IVehicleService _vehicleService;

        private string _brand = string.Empty;
        private string _model = string.Empty;
        private string _year = string.Empty; // Uso stringa per facilitare l'input dell'utente
        private string _vin = string.Empty;

        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public event PropertyChangedEventHandler? PropertyChanged;

        public VehicleDetailViewModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);
        }

        public ICommand SaveCommand { get; }

        // --- Proprietà di Binding ---

        public string Brand
        {
            get => _brand;
            set { _brand = value; OnPropertyChanged(); }
        }

        public string Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged(); }
        }

        public string Year
        {
            get => _year;
            set { _year = value; OnPropertyChanged(); }
        }

        public string Vin
        {
            get => _vin;
            set { _vin = value; OnPropertyChanged(); }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                OnPropertyChanged();
                ((Command)SaveCommand).ChangeCanExecute(); // Disabilita il bottone mentre salva
            }
        }

        // --- Logica di Salvataggio ---

        private async Task SaveAsync()
        {
            if (IsBusy) return;

            ErrorMessage = string.Empty;

            // 1. Validazione base da officina (non facciamo i pignoli, ma le basi servono)
            if (string.IsNullOrWhiteSpace(Brand) || string.IsNullOrWhiteSpace(Model) || string.IsNullOrWhiteSpace(Vin))
            {
                ErrorMessage = "Marca, Modello e Telaio (VIN) sono obbligatori.";
                return;
            }

            if (!int.TryParse(Year, out int yearParsed) || yearParsed < 1900 || yearParsed > DateTime.Now.Year + 1)
            {
                ErrorMessage = "Inserisci un anno di produzione valido.";
                return;
            }

            IsBusy = true;

            try
            {
                var dto = new CreateMotorcycleDto
                {
                    Brand = this.Brand.Trim(),
                    Model = this.Model.Trim(),
                    Year = yearParsed,
                    Vin = this.Vin.Trim().ToUpper() // Il telaio si salva sempre in maiuscolo
                };

                // Chiamata all'API tramite il nostro "cavo dell'acceleratore"
                var result = await _vehicleService.CreateVehicleAsync(dto);

                if (result != null)
                {
                    // Successo! Chiudiamo la pagina e torniamo indietro
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
                // Qui peschiamo esattamente il messaggio del 409 Conflict ("Il VIN inserito è già presente...")
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
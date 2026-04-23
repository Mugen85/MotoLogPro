using MotoLogPro.Client.Services;
using MotoLogPro.Shared.DTOs;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace MotoLogPro.Client.ViewModels
{
    public class VehicleDetailViewModel : INotifyPropertyChanged
    {
        private readonly IVehicleService _vehicleService;
        private readonly Services.ICatalogService _catalogService;

        private BrandDto? _selectedBrand;
        private BikeModelDto? _selectedModel;
        private string _year = string.Empty;
        private string _vin = string.Empty;

        private string _errorMessage = string.Empty;
        private bool _isBusy;

        public event PropertyChangedEventHandler? PropertyChanged;

        public ObservableCollection<BrandDto> Brands { get; } = new();
        public ObservableCollection<BikeModelDto> Models { get; } = new();

        public VehicleDetailViewModel(IVehicleService vehicleService, Services.ICatalogService catalogService)
        {
            _vehicleService = vehicleService;
            _catalogService = catalogService;
            SaveCommand = new Command(async () => await SaveAsync(), () => !IsBusy);

            // Carichiamo le marche appena il ViewModel viene istanziato
            _ = LoadBrandsAsync();
        }

        public ICommand SaveCommand { get; }

        // --- Proprietà a Cascata ---

        public BrandDto? SelectedBrand
        {
            get => _selectedBrand;
            set
            {
                if (_selectedBrand != value)
                {
                    _selectedBrand = value;
                    OnPropertyChanged();

                    // Se l'utente cambia marca, azzeriamo il modello e ricarichiamo la lista
                    SelectedModel = null;
                    _ = LoadModelsAsync();
                }
            }
        }

        public BikeModelDto? SelectedModel
        {
            get => _selectedModel;
            set { _selectedModel = value; OnPropertyChanged(); }
        }

        // --- Altre Proprietà ---

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
                ((Command)SaveCommand).ChangeCanExecute();
            }
        }

        // --- Logica Dati ---

        private async Task LoadBrandsAsync()
        {
            IsBusy = true;
            try
            {
                var brands = await _catalogService.GetBrandsAsync();
                Brands.Clear();
                foreach (var brand in brands)
                {
                    Brands.Add(brand);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Impossibile caricare le marche.";
                System.Diagnostics.Debug.WriteLine($"Errore: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task LoadModelsAsync()
        {
            if (SelectedBrand == null)
            {
                Models.Clear();
                return;
            }

            IsBusy = true;
            try
            {
                var models = await _catalogService.GetModelsByBrandAsync(SelectedBrand.Id);
                Models.Clear();
                foreach (var model in models)
                {
                    Models.Add(model);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Impossibile caricare i modelli.";
                System.Diagnostics.Debug.WriteLine($"Errore: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task SaveAsync()
        {
            if (IsBusy) return;
            ErrorMessage = string.Empty;

            if (SelectedBrand == null || SelectedModel == null || string.IsNullOrWhiteSpace(Vin))
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
                    Brand = SelectedBrand.Name, // Preleviamo il nome dall'oggetto selezionato!
                    Model = SelectedModel.Name,
                    Year = yearParsed,
                    Vin = this.Vin.Trim().ToUpper()
                };

                var result = await _vehicleService.CreateVehicleAsync(dto);

                if (result != null)
                {
                    await Shell.Current.GoToAsync("..");
                }
            }
            catch (Exception ex)
            {
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
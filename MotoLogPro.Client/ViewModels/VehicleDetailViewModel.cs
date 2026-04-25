using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MotoLogPro.Shared.DTOs;
using System.Collections.ObjectModel;

// ATTENZIONE: Assicurati che non ci siano "using Java..." o "using Android..." qui sopra!

namespace MotoLogPro.Client.ViewModels;

[QueryProperty(nameof(VehicleToEdit), "VehicleToEdit")]
public partial class VehicleDetailViewModel : ObservableObject
{
    // Usiamo i percorsi assoluti per evitare qualsiasi ambiguità (Errore CS0104 risolto)
    private readonly MotoLogPro.Client.Services.IVehicleService _vehicleService;
    private readonly MotoLogPro.Client.Services.ICatalogService _catalogService;

    public ObservableCollection<BrandDto> Brands { get; } = [];
    public ObservableCollection<BikeModelDto> Models { get; } = [];

    // --- Variabili di stato generate dal Toolkit ---
    [ObservableProperty] private VehicleDto? _vehicleToEdit;
    [ObservableProperty] private BrandDto? _selectedBrand;
    [ObservableProperty] private BikeModelDto? _selectedModel;
    [ObservableProperty] private string _year = string.Empty;
    [ObservableProperty] private string _vin = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string _errorMessage = string.Empty;

    [ObservableProperty] private bool _isBusy;

    [ObservableProperty] private string _pageTitle = "Nuovo Veicolo";
    [ObservableProperty] private string _saveButtonText = "SALVA MOTO NEL GARAGE";

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Costruttore classico per massima stabilità con l'iniezione delle dipendenze
    public VehicleDetailViewModel(
        MotoLogPro.Client.Services.IVehicleService vehicleService,
        MotoLogPro.Client.Services.ICatalogService catalogService)
    {
        _vehicleService = vehicleService;
        _catalogService = catalogService;
    }

    // --- Inizializzazione ---

    [RelayCommand]
    public async Task LoadInitialDataAsync()
    {
        if (Brands.Count > 0) return;

        IsBusy = true;
        try
        {
            var brands = await _catalogService.GetBrandsAsync();
            Brands.Clear();
            foreach (var brand in brands) Brands.Add(brand);
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

    // --- Logica a Cascata ---

    partial void OnSelectedBrandChanged(BrandDto? value)
    {
        Models.Clear();
        SelectedModel = null;

        if (value is null) return;

        _ = LoadModelsForBrandAsync(value.Id);
    }

    private async Task LoadModelsForBrandAsync(int brandId)
    {
        IsBusy = true;
        try
        {
            var models = await _catalogService.GetModelsByBrandAsync(brandId);
            Models.Clear();
            foreach (var model in models) Models.Add(model);
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

    // --- Logica di Modifica (Rehydration) ---

    partial void OnVehicleToEditChanged(VehicleDto? value)
    {
        if (value is null) return;

        PageTitle = "Modifica Veicolo";
        SaveButtonText = "AGGIORNA DATI MOTO";
        Year = value.Year.ToString();
        Vin = value.Vin;

        _ = RehydrateDropdownsAsync(value);
    }

    private async Task RehydrateDropdownsAsync(VehicleDto moto)
    {
        IsBusy = true;

        if (Brands.Count == 0) await LoadInitialDataAsync();

        SelectedBrand = Brands.FirstOrDefault(b => b.Name == moto.Brand);

        if (SelectedBrand != null)
        {
            await LoadModelsForBrandAsync(SelectedBrand.Id);
            SelectedModel = Models.FirstOrDefault(m => m.Name == moto.Model);
        }

        IsBusy = false;
    }

    // --- Salvataggio ---

    [RelayCommand]
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
                Brand = SelectedBrand.Name,
                Model = SelectedModel.Name,
                Year = yearParsed,
                Vin = Vin.Trim().ToUpper()
            };

            if (VehicleToEdit is null)
            {
                await _vehicleService.CreateVehicleAsync(dto);
            }
            else
            {
                await _vehicleService.UpdateVehicleAsync(VehicleToEdit.Id, dto);
            }

            await Shell.Current.GoToAsync("..");
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
}
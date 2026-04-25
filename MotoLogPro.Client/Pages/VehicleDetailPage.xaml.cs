using MotoLogPro.Client.ViewModels;

namespace MotoLogPro.Client.Pages
{
    public partial class VehicleDetailPage : ContentPage
    {
        public VehicleDetailPage(VehicleDetailViewModel viewModel)
        {
            InitializeComponent();

            // Agganciamo la View al ViewModel
            BindingContext = viewModel;
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Ci assicuriamo che il ViewModel sia collegato
            if (BindingContext is VehicleDetailViewModel vm)
            {
                // Eseguiamo il comando in sicurezza.
                // Essendo OnAppearing, Android sa che l'interfaccia è pronta.
                vm.LoadInitialDataCommand.Execute(null);
            }
        }
    }
}
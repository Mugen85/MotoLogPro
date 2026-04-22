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
    }
}
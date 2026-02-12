using MotoLogPro.Client.ViewModels;

namespace MotoLogPro.Client.Pages;

public partial class DashboardPage : ContentPage
{
    private readonly DashboardViewModel _viewModel;

    public DashboardPage(DashboardViewModel vm)
    {
        InitializeComponent();
        BindingContext = _viewModel = vm;
    }

    // Carichiamo i dati ogni volta che la pagina appare
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.LoadDataCommand.ExecuteAsync(null);
    }
}
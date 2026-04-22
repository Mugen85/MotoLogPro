namespace MotoLogPro.Client
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            Routing.RegisterRoute(nameof(Pages.VehicleDetailPage), typeof(Pages.VehicleDetailPage));
            InitializeComponent();
        }
    }
}

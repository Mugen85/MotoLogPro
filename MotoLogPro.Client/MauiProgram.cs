using Microsoft.Extensions.Logging;
using MotoLogPro.Client.ViewModels;
using MotoLogPro.Client.Pages;
using MotoLogPro.Client.Services;

namespace MotoLogPro.Client
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
#if DEBUG
            builder.Logging.AddDebug();
#endif

            string apiUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7035"
                : "https://localhost:7035";

            // DRY: Definiamo il bypass SSL una sola volta per tutto l'impianto
            HttpMessageHandler GetInsecureHandler()
            {
                var handler = new HttpClientHandler();
#if DEBUG
                // In sviluppo, accettiamo qualsiasi certificato (Bypass)
                handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
#endif
                return handler;
            }

            // AuthService
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(GetInsecureHandler);

            // VehicleService
            builder.Services.AddHttpClient<IVehicleService, VehicleService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(GetInsecureHandler);

            // CatalogService - SPINOTTO RIATTACCATO
            builder.Services.AddHttpClient<ICatalogService, CatalogService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(GetInsecureHandler);

            // Pagine e ViewModel
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardPage>();
            builder.Services.AddTransient<VehicleDetailViewModel>();
            builder.Services.AddTransient<VehicleDetailPage>();

            return builder.Build();
        }
    }
}
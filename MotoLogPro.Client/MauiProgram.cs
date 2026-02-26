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

            // AuthService — con SSL bypass per sviluppo
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
#endif
                return handler;
            });

            // VehicleService — con SSL bypass per sviluppo
            builder.Services.AddHttpClient<IVehicleService, VehicleService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                var handler = new HttpClientHandler();
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = (m, c, ch, e) => true;
#endif
                return handler;
            });

            // Pagine e ViewModel
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<DashboardViewModel>();
            builder.Services.AddTransient<DashboardPage>();

            return builder.Build();
        }
    }
}
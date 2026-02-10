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
            // Registrazione delle Pagine e dei ViewModel
            // Transient = Ne crea uno nuovo ogni volta che apro la pagina
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();


            // Configurazione URL API (Usa la porta corretta del tuo progetto API!)
            string apiUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "http://10.0.2.2:5141"   // Controlla la porta HTTP nel launchSettings.json dell'API
                : "https://localhost:7035"; // Controlla la porta HTTPS

            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            });
            return builder.Build();
        }
    }
}

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


            // Configurazione URL API
            string apiUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7035"   // <--- TORNALIMO A USARE HTTPS (Porta 7035)
                : "https://localhost:7035"; // Windows HTTPS

            // Registriamo l'HttpClient con un "Handler" personalizzato che bypassa il controllo SSL
            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            })
            .ConfigurePrimaryHttpMessageHandler(() =>
            {
                // Creiamo un handler che gestisce le richieste HTTP
                var handler = new HttpClientHandler();

                // SOLO PER SVILUPPO: Ignora gli errori di certificato SSL (perché sono self-signed)
                // Se siamo in Debug, restituiamo sempre "true" (tutto ok)
#if DEBUG
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
                return handler;
            });

            builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
            {
                client.BaseAddress = new Uri(apiUrl);
            });
            return builder.Build();
        }
    }
}

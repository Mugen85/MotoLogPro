using MotoLogPro.Client.ViewModels;

namespace MotoLogPro.Client.Pages
{
    public partial class LoginPage : ContentPage
    {
        // Dependency Injection: Il ViewModel arriva già pronto dal costruttore!
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm; // Questo è il collegamento magico
        }
    }
}
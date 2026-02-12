namespace MotoLogPro.Client.Services
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(string email, string password);
        Task<bool> RegisterAsync(string fullName, string email, string password, string confirmPassword);
        Task LogoutAsync();
        Task<bool> IsUserLoggedIn();
    }
}
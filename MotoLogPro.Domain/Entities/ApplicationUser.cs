using Microsoft.AspNetCore.Identity;

namespace MotoLogPro.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        // Relazione 1:N -> Un utente ha molte moto
        public virtual ICollection<Motorcycle> Motorcycles { get; set; } = [];
    }
}
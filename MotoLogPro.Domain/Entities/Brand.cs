namespace MotoLogPro.Domain.Entities
{
    public class Brand
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Navigation property per Entity Framework:
        // Una marca ha tanti modelli
        public ICollection<BikeModel> Models { get; set; } = [];
    }
}
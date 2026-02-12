namespace MotoLogPro.Shared.DTOs
{
    public class VehicleDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty; // Es: Yamaha
        public string Model { get; set; } = string.Empty; // Es: XT1200Z
        public string LicensePlate { get; set; } = string.Empty;
        public int Year { get; set; }
        public string OwnerName { get; set; } = string.Empty; // Utile per l'officina
    }
}
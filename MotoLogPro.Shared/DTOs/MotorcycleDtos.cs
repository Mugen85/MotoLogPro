using System.ComponentModel.DataAnnotations;

namespace MotoLogPro.Shared.DTOs
{
    // DTO per mostrare la lista delle moto (Lettura)
    public record MotorcycleDto
    {
        public int Id { get; set; }
        public string Brand { get; set; } = string.Empty; // Es. Yamaha
        public string Model { get; set; } = string.Empty; // Es. XT1200Z
        public string PlateNumber { get; set; } = string.Empty; // Targa (opzionale in visualizzazione)
        public int Year { get; set; }
        public string OwnerName { get; set; } = string.Empty; // Utile per il meccanico
    }

    // DTO per creare o modificare una moto (Scrittura)
    public record CreateMotorcycleDto
    {
        [Required]
        [StringLength(50)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Model { get; set; } = string.Empty;

        [Required]
        [Range(1900, 2100, ErrorMessage = "Anno non valido")]
        public int Year { get; set; }

        [Required]
        [StringLength(17, MinimumLength = 17, ErrorMessage = "Il VIN deve essere di 17 caratteri")]
        public string Vin { get; set; } = string.Empty; // Numero telaio per API esterne

        [StringLength(10)]
        public string PlateNumber { get; set; } = string.Empty;
    }
}
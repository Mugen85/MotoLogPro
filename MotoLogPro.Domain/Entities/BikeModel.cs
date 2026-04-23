namespace MotoLogPro.Domain.Entities
{
    public class BikeModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }

        // Chiave esterna verso la Marca
        public int BrandId { get; set; }
        public Brand? Brand { get; set; }
    }
}
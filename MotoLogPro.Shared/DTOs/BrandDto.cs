namespace MotoLogPro.Shared.DTOs
{
    public class BrandDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
    }

    public class BikeModelDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int BrandId { get; set; }
    }
}
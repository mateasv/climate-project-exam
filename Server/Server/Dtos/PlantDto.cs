
namespace Server.Dtos
{
    public class PlantDto
    {
        public int PlantId { get; set; }
        public int? DataloggerId { get; set; }
        public int? PlantTypeId { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public float? Price { get; set; }
    }
}

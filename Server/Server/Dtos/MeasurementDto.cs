// https://code-maze.com/automapper-net-core/

namespace Server.Dtos
{
    public class MeasurementDto
    {
        public int MeasurementId { get; set; }
        public int DataloggerId { get; set; }
        public int PlantId { get; set; }
        public float SoilHumidity { get; set; }
        public float AirHumidity { get; set; }
        public float AirTemerature { get; set; }
        public bool SoilIsDry { get; set; }
    }
}

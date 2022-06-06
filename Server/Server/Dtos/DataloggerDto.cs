namespace Server.Dtos
{
    public class DataloggerDto
    {
        public int DataloggerId { get; set; }
        public float? MinAirHumidity { get; set; }
        public float? MaxAirHumidity { get; set; }
        public float? MinAirTemperature { get; set; }
        public float? MaxAirTemperature { get; set; }
    }
}

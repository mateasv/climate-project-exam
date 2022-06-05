using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Measurement
    {
        public int MeasurementId { get; set; }
        public int? DataloggerId { get; set; }
        public int? PlantId { get; set; }
        public float AirHumidity { get; set; }
        public float AirTemperature { get; set; }
        public bool SoilIsDry { get; set; }
        public DateTime? MeasurementDate { get; set; }

        public virtual Datalogger? Datalogger { get; set; }
        public virtual Plant? Plant { get; set; }
    }
}

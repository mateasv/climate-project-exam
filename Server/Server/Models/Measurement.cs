using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Measurement
    {
        public int MeasurementId { get; set; }
        public int? PlantId { get; set; }
        public float? SoilHumidity { get; set; }
        public float? AirHumidity { get; set; }
        public float? AirTemerature { get; set; }

        public virtual Plant? Plant { get; set; }
    }
}

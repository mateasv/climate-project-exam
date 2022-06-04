using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Datalogger
    {
        public Datalogger()
        {
            Measurements = new HashSet<Measurement>();
            Plants = new HashSet<Plant>();
        }

        public int DataloggerId { get; set; }
        public float? MinAirHumidity { get; set; }
        public float? MaxAirHumidity { get; set; }
        public float? MinAirTemperature { get; set; }
        public float? MaxAirTemperature { get; set; }

        public virtual ICollection<Measurement> Measurements { get; set; }
        public virtual ICollection<Plant> Plants { get; set; }
    }
}

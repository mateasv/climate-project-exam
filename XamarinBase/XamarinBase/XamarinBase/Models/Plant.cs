using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Plant
    {
        public Plant()
        {
            Dataloggers = new HashSet<Datalogger>();
            Measurements = new HashSet<Measurement>();
        }

        public int PlantId { get; set; }
        public int? PlantTypeId { get; set; }
        public DateTime? WarrantyStartDate { get; set; }
        public float? Price { get; set; }
        public byte[] Image { get; set; }

        public virtual PlantType PlantType { get; set; }
        public virtual ICollection<Datalogger> Dataloggers { get; set; }
        public virtual ICollection<Measurement> Measurements { get; set; }
    }
}

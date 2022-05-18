using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class Datalogger
    {
        public int DataloggerId { get; set; }
        public int? PlantId { get; set; }

        public virtual Plant? Plant { get; set; }
    }
}

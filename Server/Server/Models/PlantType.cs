using System;
using System.Collections.Generic;

namespace Server.Models
{
    public partial class PlantType
    {
        public PlantType()
        {
            Plants = new HashSet<Plant>();
        }

        public int PlantTypeId { get; set; }
        public string? PlantTypeName { get; set; }

        public virtual ICollection<Plant> Plants { get; set; }
    }
}

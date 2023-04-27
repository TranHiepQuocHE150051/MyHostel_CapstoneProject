using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Facility
    {
        public Facility()
        {
            NearbyFacilities = new HashSet<NearbyFacility>();
        }

        public int Id { get; set; }
        public string UtilityName { get; set; } = null!;
        public string Code { get; set; } = null!;

        public virtual ICollection<NearbyFacility> NearbyFacilities { get; set; }
    }
}

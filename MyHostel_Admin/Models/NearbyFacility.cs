using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class NearbyFacility
    {
        public int Id { get; set; }
        public int UltilityId { get; set; }
        public int HostelId { get; set; }
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
        public string Name { get; set; } = null!;

        public virtual Hostel Hostel { get; set; } = null!;
        public virtual Facility Ultility { get; set; } = null!;
    }
}

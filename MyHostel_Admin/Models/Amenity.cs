using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class Amenity
    {
        public Amenity()
        {
            HostelAmenities = new HashSet<HostelAmenity>();
        }

        public int Id { get; set; }
        public string AmenitiyName { get; set; } = null!;
        public string? Icon { get; set; }
        public bool? Checked { get; set; }

        public virtual ICollection<HostelAmenity> HostelAmenities { get; set; }
    }
}

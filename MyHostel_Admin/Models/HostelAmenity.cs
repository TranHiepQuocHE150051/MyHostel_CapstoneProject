using System;
using System.Collections.Generic;

namespace MyHostel_Admin.Models
{
    public partial class HostelAmenity
    {
        public int HostedId { get; set; }
        public int AmenitiesId { get; set; }
        public int Id { get; set; }

        public virtual Amenity Amenities { get; set; } = null!;
        public virtual Hostel Hosted { get; set; } = null!;
    }
}

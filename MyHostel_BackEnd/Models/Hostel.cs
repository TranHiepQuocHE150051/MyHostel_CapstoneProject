using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Hostel
    {
        public Hostel()
        {
            Chats = new HashSet<Chat>();
            NearbyFacilities = new HashSet<NearbyFacility>();
            Residents = new HashSet<Resident>();
            Amenities = new HashSet<Amenity>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string GoogleLocationLat { get; set; } = null!;
        public string GoogleLocationLnd { get; set; } = null!;
        public byte[] CreatedAt { get; set; } = null!;
        public string WardsCode { get; set; } = null!;

        public virtual Ward WardsCodeNavigation { get; set; } = null!;
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<NearbyFacility> NearbyFacilities { get; set; }
        public virtual ICollection<Resident> Residents { get; set; }

        public virtual ICollection<Amenity> Amenities { get; set; }
    }
}

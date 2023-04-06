using System;
using System.Collections.Generic;

namespace MyHostel_BackEnd.Models
{
    public partial class Hostel
    {
        public Hostel()
        {
            Chats = new HashSet<Chat>();
            HostelAmenities = new HashSet<HostelAmenity>();
            HostelImages = new HashSet<HostelImage>();
            NearbyFacilities = new HashSet<NearbyFacility>();
            Residents = new HashSet<Resident>();
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? GoogleLocationLat { get; set; }
        public string? GoogleLocationLnd { get; set; }
        public DateTime CreatedAt { get; set; }
        public string WardsCode { get; set; } = null!;
        public decimal Price { get; set; }
        public string Capacity { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public int LandlordId { get; set; }
        public string Description { get; set; } = null!;
        public string RoomArea { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public decimal Electricity { get; set; }
        public decimal Water { get; set; }
        public decimal Internet { get; set; }
        public int Status { get; set; }

        public virtual Member Landlord { get; set; } = null!;
        public virtual Ward WardsCodeNavigation { get; set; } = null!;
        public virtual ICollection<Chat> Chats { get; set; }
        public virtual ICollection<HostelAmenity> HostelAmenities { get; set; }
        public virtual ICollection<HostelImage> HostelImages { get; set; }
        public virtual ICollection<NearbyFacility> NearbyFacilities { get; set; }
        public virtual ICollection<Resident> Residents { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}

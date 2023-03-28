namespace MyHostel_BackEnd.DTOs
{
    public class GetHostelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string[] ImgURL { get; set; } = null!;
        public NearbyFacilitiesGetHostelDTO[] NearbyFacilities { get; set; } = null!;
        public int[] Amenities { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string WardName { get; set; } = null!;
        public string RoomArea { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Price { get; set; } = null!;
        public string Electricity { get; set; } = null!;
        public string Water { get; set; } = null!;
        public string Internet { get; set; } = null!;
        public LandlordGetHostelDTO Landlord { get; set; } = null!;
        public ReviewGetHostelDTO Review { get; set; } = null!;
    }
}

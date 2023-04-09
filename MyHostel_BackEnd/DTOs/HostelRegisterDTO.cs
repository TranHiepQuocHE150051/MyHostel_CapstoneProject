namespace MyHostel_BackEnd.DTOs
{
    public class HostelRegisterDTO
    {
        public string Name { get; set; } = null!;
        public string Price { get; set; }
        public string rooms { get; set; }
        public string Capacity { get; set; }
        public string DetailLocation { get; set; } = null!;
        public string LocationLat { get; set; } = null!;
        public string LocationLng { get; set; } = null!;
        
        public string WardsCode { get; set; } = null!;
        public string Amenities { get; set; }
        public string[] imagesUrl { get; set; }
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string RoomArea { get; set; } = null!;

        public string Electricity { get; set; }
        public string Water { get; set; }
        public string Internet { get; set; }
    }
}

namespace MyHostel_BackEnd.DTOs
{
    public class HostelRegisterDTO
    {
        public string Name { get; set; } = null!;
        public Decimal Price { get; set; }
        public int rooms { get; set; }
        public string Capacity { get; set; }
        public string GoogleLocationLat { get; set; } = null!;
        public string GoogleLocationLnd { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string WardsCode { get; set; } = null!; 
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string RoomArea { get; set; } = null!;
        public string[] imagesUrl { get; set; }
    }
}

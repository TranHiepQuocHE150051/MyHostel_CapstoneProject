namespace MyHostel_BackEnd.DTOs
{
    public class UpdateHostelDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public string Capacity { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string WardsCode { get; set; } = null!;
        public string Amenities { get; set; }
        public ImagesUrl_UpdateHostelDTO ImagesUrl { get; set; }
        public string Phone { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string RoomArea { get; set; } = null!;

        public decimal Electricity { get; set; }
        public decimal Water { get; set; }
        public decimal Internet { get; set; }
    }
}

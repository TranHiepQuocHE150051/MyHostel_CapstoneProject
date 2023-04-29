namespace MyHostel_BackEnd.DTOs
{
    public class SuggestHostelDTO
    {
        public int HostelId { get; set; }
        public string Name { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string WardName { get; set; } = null!;
        public string DistrictName { get; set; } = null!;
        public string? Price { get; set; }
        public string ImgUrl { get; set; } = null!;
        public AmenitiesGetHostelDTO[] Amenities { get; set; } = null!;
        public double Rate { get; set; }
        public int NoRate { get; set; }
    }
}

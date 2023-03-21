namespace MyHostel_BackEnd.DTOs
{
    public class NearbyFacilitiesGetHostelDTO
    {
        public string Name { get; set; } = null!;
        public decimal Distance { get; set; }
        public decimal Duration { get; set; }
    }
}

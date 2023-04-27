namespace MyHostel_BackEnd.DTOs
{
    public class NearbyFacilitiesGetHostelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public List<object> Places { get; set; }
    }
}

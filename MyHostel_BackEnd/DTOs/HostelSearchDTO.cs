namespace MyHostel_BackEnd.DTOs
{
    public class HostelSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public string Price { get; set; }
        public string imgUrl { get; set; } = null!;
        public List<object> Amenities { get; set; }
        public object Review { get; set; }
    }
}

namespace MyHostel_BackEnd.DTOs
{
    public class HostelSearchDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string DetailLocation { get; set; } = null!;
        public decimal Price { get; set; }
        public string imgUrl { get; set; } = null!;
    }
}

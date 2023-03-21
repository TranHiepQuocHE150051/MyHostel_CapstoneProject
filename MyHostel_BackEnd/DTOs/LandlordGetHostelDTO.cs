namespace MyHostel_BackEnd.DTOs
{
    public class LandlordGetHostelDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Avatar { get; set; } = null!;
    }
}

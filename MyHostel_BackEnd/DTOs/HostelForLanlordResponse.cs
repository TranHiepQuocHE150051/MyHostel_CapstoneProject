namespace MyHostel_BackEnd.DTOs
{
    public class HostelForLanlordResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int RoomNo { get; set; }
        public string DetailLocation { get; set; } = null!;
        public string ImgUrl { get; set; } = null!;
        public int Status { get; set; }
        public int AvailableRooms { get; set; }
        public int ResidentNo { get; set; }
    }
}

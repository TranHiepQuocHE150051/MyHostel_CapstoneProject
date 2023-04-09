namespace MyHostel_BackEnd.DTOs
{
    public class UpdateRoomDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int RoomArea { get; set; }
    }
}

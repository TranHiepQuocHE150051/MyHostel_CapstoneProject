namespace MyHostel_BackEnd.DTOs
{
    public class AddNewRoomDTO
    {
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int RoomArea { get; set; }
    }
}

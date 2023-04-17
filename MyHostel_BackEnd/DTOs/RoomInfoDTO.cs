namespace MyHostel_BackEnd.DTOs
{
    public class RoomInfoDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; }
        public string Price { get; set; }
        public int Area { get; set; }
        public List<object> Roommate { get; set; }
        public object Hostel { get; set; }
    }
}

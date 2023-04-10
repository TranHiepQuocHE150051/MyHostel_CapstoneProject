namespace MyHostel_BackEnd.DTOs
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public int RoomArea { get; set; }
        public string ConvertPrice {get; set;}= null!;

        public List<ResidentDTO> Residents { get; set; } 
    }
}

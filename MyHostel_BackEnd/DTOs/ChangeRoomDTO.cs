namespace MyHostel_BackEnd.DTOs
{
    public class ChangeRoomDTO
    {
        public int MemberId { get; set; }
        public int FromRoomId { get; set; }
        public int ToRoomId { get; set; }
    }
}

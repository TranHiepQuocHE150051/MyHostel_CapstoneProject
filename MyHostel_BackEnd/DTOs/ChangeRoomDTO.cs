namespace MyHostel_BackEnd.DTOs
{
    public class ChangeRoomDTO
    {
        public string InviteCode { get; set; }
        public int FromRoomId { get; set; }
        public int ToRoomId { get; set; }
    }
}

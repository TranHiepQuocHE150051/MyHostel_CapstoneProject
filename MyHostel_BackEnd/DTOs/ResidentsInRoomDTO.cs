namespace MyHostel_BackEnd.DTOs
{
    public class ResidentsInRoomDTO
    {
        public int MemberId { get; set; }
        public string FullName { get; set; }
        public string Avatar { get; set; }
        public string JoinedAt { get; set; }
        public string InviteToken { get; set; }
    }
}

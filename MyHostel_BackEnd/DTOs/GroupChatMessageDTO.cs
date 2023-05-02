namespace MyHostel_BackEnd.DTOs
{
    public class GroupChatMessageDTO
    {
        public byte AnonymousFlg { get; set; }
        public string MsgText { get; set; } = null!;
        public string? ParticipantName { get; set; }
        public string? CreatedAt { get; set; }
    }
}

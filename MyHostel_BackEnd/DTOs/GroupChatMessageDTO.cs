namespace MyHostel_BackEnd.DTOs
{
    public class GroupChatMessageDTO
    {
        public byte AnonymousFlg { get; set; }
        public string MsgText { get; set; } = null!;
        public string? CreatedAt { get; set; }
    }
}

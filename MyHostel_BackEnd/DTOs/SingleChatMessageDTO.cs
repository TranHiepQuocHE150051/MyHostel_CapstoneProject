namespace MyHostel_BackEnd.DTOs
{
    public class SingleChatMessageDTO
    {
        public byte AnonymousFlg { get; set; }
        public int MemberId { get; set; }
        public string? AvatarUrl { get; set; }
        public string MsgText { get; set; } = null!;
        public string? CreatedAt { get; set; }
    }
}

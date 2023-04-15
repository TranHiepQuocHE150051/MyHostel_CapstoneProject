namespace MyHostel_BackEnd.DTOs
{
    public class MessageDTO
    {
        public int MessageId { get; set; }
        public byte AnonymousFlg { get; set; }
        public string MsgText { get; set; } = null!;
        public List<string> Img { get; set; }
        public string? CreatedAt { get; set; }
        
        public ParentMessageDTO? ParentMsg { get; set; }
        public MemberInMessageDTO? Member { get; set; }
    }
}

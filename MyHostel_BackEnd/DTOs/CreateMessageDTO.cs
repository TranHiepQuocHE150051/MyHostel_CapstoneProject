namespace MyHostel_BackEnd.DTOs
{
    public class CreateMessageDTO
    {
        public int ChatId { get; set; }
        public string MsgText { get; set; }
        public int ParentMsgId { get; set; }
        public int AnonymousFlg { get; set; }
        public int AnonymousCode { get; set; }
        public int MemberId { get; set; }
        public string[] Img { get; set; }
    }
}

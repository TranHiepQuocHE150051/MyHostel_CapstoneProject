namespace MyHostel_BackEnd.DTOs
{
    public class GetListChatDTO
    {
        public int MemberId { get; set; }
        public ChatDTO[] Chats { get; set; }
    }
}

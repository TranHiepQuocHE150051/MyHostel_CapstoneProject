using Swashbuckle.AspNetCore.SwaggerGen;

namespace MyHostel_BackEnd.DTOs
{
    public class GetListChatDTO
    {
        public int MemberId { get; set; }
        public List<object> Chats { get; set; }
    }
}
